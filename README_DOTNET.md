# LJ.BillingPortal.API - .NET 9 Web API Core

A modern, enterprise-grade Billing Portal REST API built with ASP.NET Core 9 and SQL Server. **Migrated from Node.js/Express to .NET 9**.

## 🎯 Overview

Complete rewrite of LJ.BillingPortal.API from Node.js (TypeScript/Express) to **.NET 9 Web API Core** using SOLID principles and enterprise architecture patterns.

### Key Features
- ✅ RESTful API for invoice management
- ✅ Entity Framework Core with SQL Server
- ✅ Fluent Validation for input validation
- ✅ Async/await throughout
- ✅ Dependency Injection (DI) container
- ✅ Global exception handling middleware
- ✅ Structured logging with Serilog
- ✅ PDF generation capabilities
- ✅ CORS support
- ✅ Swagger/OpenAPI documentation
- ✅ Health check endpoints
- ✅ Database migrations with automatic apply on startup

## 🏗️ Architecture

### Layered Architecture Pattern
```
Controllers → Services → Data Access (EF Core) → SQL Server
                ↓
            Validators
                ↓
            Middleware
```

### Project Structure
```
LJ.BillingPortal.API/
├── Controllers/         # HTTP request handlers
├── Services/            
│   ├── Interfaces/      # Service contracts
│   ├── InvoiceService.cs
│   └── PdfGenerationService.cs
├── DTOs/               # Data Transfer Objects
├── Models/             # EF Core entities
├── Data/               # DbContext and configurations
├── Validators/         # FluentValidation rules
├── Middleware/         # Request/response middleware
├── Exceptions/         # Custom exceptions
├── Migrations/         # EF Core database migrations
├── Program.cs          # Application entry point
├── appsettings.json    # Configuration
└── LJ.BillingPortal.API.csproj
```

## 🚀 Quick Start

### Prerequisites
- .NET 9 SDK
- SQL Server 2019+
- Visual Studio 2022 or VS Code

### Setup in 5 Steps

1. **Restore NuGet packages**
```bash
dotnet restore
```

2. **Update appsettings.json** with your SQL Server connection:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=InvoiceDB;User Id=Admin_Giribabu;Password=$LJLpass01;Encrypt=False;TrustServerCertificate=True;"
}
```

3. **Apply database migrations**
```bash
dotnet ef database update
```

4. **Run the API**
```bash
dotnet run
```

5. **Access Swagger UI**
Open: http://localhost:5001

## 📚 Core API Endpoints

### Invoices
```
GET    /api/invoices/all              # Get all invoices
GET    /api/invoices/next-number      # Get next invoice number  
POST   /api/invoices/create           # Create new invoice
POST   /api/invoices/generate-pdf     # Generate invoice PDF
PUT    /api/invoices/details          # Update invoice
PUT    /api/invoices/particulars      # Update line item
```

### Clients  
```
GET    /api/invoices/clients          # Get all client addresses
PUT    /api/invoices/clients          # Update client address
```

## 🔧 Configuration

### Database Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=InvoiceDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=False;TrustServerCertificate=True;"
  }
}
```

### CORS Origins
```json
{
  "AllowedOrigins": [
    "http://localhost:3000",
    "http://localhost:5001"
  ]
}
```

### Logging
Configured via Serilog:
- Console output
- File output (daily rolling logs in `logs/` folder)
- Level: Information (configurable in appsettings)

## 🗄️ Database Schema

### Tables

**ClientDetails**
- ClientID (PK)
- CompanyName
- AddressLine1, AddressLine2, AddressLine3
- GSTIN, State, StateCode
- CreatedDate, ModifiedDate

**InvoiceDetails**
- InvoiceID (PK)
- ClientID (FK)
- InvoiceNumber
- InvoiceDate, PlaceOfSupply
- PONumber, CraneReg
- TotalAmountBeforeTax, CGST, SGST, IGST, NetAmountAfterTax
- CreatedDate, ModifiedDate

**InvoiceParticulars**
- ServiceID (PK)
- InvoiceID (FK)
- Description, HSN_SAC
- Quantity, Rate, TaxableValue
- CreatedDate

### Relationships
- ClientDetails ←1:N→ InvoiceDetails
- InvoiceDetails ←1:N→ InvoiceParticulars

### Indexes
- ClientDetails.GSTIN
- InvoiceDetails.InvoiceNumber
- InvoiceDetails.ClientID
- InvoiceParticulars.InvoiceID

## ✅ Validation (FluentValidation)

All inputs validated automatically:
- ✓ GSTIN format (Indian tax ID pattern)
- ✓ HSN/SAC numeric format (4-8 digits)
- ✓ Required fields
- ✓ Numeric ranges (non-negative amounts)
- ✓ Date validation

## 🛡️ Error Handling

Global exception middleware returns standardized JSON:
```json
{
  "message": "Error description",
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": {
    "fieldName": ["Validation error message"]
  }
}
```

HTTP Status Codes:
- `400` - Validation errors
- `404` - Not found
- `500` - Internal server error

## 🏆 Design Patterns Implemented

1. **Dependency Injection** - Built-in ASP.NET Core DI
2. **Repository Pattern** - EF Core abstracts data access
3. **DTO Pattern** - Separates API from domain models
4. **Middleware Pipeline** - Cross-cutting concerns
5. **Factory Pattern** - DbContextFactory for migrations
6. **Async/Await** - Non-blocking I/O operations
7. **Validation Pipeline** - FluentValidation framework
8. **Exception Handling** - Global middleware

## 📊 Node.js to .NET Migration Comparison

| Feature | Node.js | .NET 9 |
|---------|---------|--------|
| Framework | Express | ASP.NET Core |
| Language | TypeScript | C# |
| Type System | TypeScript | C# (strict) |
| Performance | Node.js runtime | .NET JIT |
| ORM | Raw SQL | Entity Framework Core |
| Validation | Manual | FluentValidation |
| Error Handling | Manual try-catch | Global Middleware |
| Logging | dotenv + custom | Serilog |
| DI Container | Manual | Built-in |
| API Docs | Swagger addon | Built-in OpenAPI |

## 📦 NuGet Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.EntityFrameworkCore | 9.0.0 | ORM |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.0 | SQL Server Provider |
| FluentValidation | 11.11.0 | Input Validation |
| Swashbuckle.AspNetCore | 7.1.1 | Swagger/OpenAPI |
| SelectPdf | 25.0.0 | PDF Generation |
| Serilog | 4.2.0 | Structured Logging |

## 🧪 Development Commands

**Build:**
```bash
dotnet build
```

**Run in Debug:**
```bash
dotnet run
```

**Run in Release:**
```bash
dotnet run --configuration Release
```

**Create Migration:**
```bash
dotnet ef migrations add MigrationName
```

**Apply Migrations:**
```bash
dotnet ef database update
```

**Remove Last Migration:**
```bash
dotnet ef migrations remove
```

**Publish:**
```bash
dotnet publish -c Release -o ./publish
```

**Run Published:**
```bash
dotnet ./publish/LJ.BillingPortal.API.dll
```

## 🔗 Resources

- [.NET 9 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9)
- [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [FluentValidation](https://fluentvalidation.net/)
- [Serilog](https://serilog.net/)
- [SelectPdf](https://selectpdf.com/)

## 📄 License

Copyright © 2024 LJ Lifters. All rights reserved.

---

**Modern .NET 9 Web API - Enterprise Architecture**
