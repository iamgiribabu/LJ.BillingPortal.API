# Quick Reference Guide - .NET 9 Web API

## 🚀 Getting Started (5 Minutes)

### 1. Prerequisites Check
```bash
# Verify .NET 9 installation
dotnet --version
# Should output: 9.0.x or higher

# SQL Server should be running on localhost
# Connection: Server=localhost;Database=InvoiceDB;User Id=Admin_Giribabu;Password=$LJLpass01
```

### 2. Setup Project
```bash
# Navigate to project directory
cd "d:\LJ lifters files\Billingprotal\LJ.BillingPortal.API"

# Restore packages
dotnet restore

# Apply migrations (creates database schema)
dotnet ef database update

# Run the API
dotnet run
```

### 3. Test the API
Open browser: **http://localhost:5001**

You should see Swagger UI with all endpoints.

---

## 📚 Common Operations

### Running the API

**Development (Debug)**
```bash
dotnet run
# API on http://localhost:5001
# Hot reload enabled: Changes detected automatically
```

**Release Mode**
```bash
dotnet run --configuration Release
# Optimized performance, no debug symbols
```

**Specific Port**
```bash
dotnet run -- --urls "http://localhost:8080"
```

### Database Operations

**Create New Migration**
```bash
dotnet ef migrations add AddNewField
# Creates migration file in Migrations/ folder
```

**Apply Migrations**
```bash
dotnet ef database update
# Applies pending migrations to database
```

**View Migrations**
```bash
dotnet ef migrations list
# Shows all migrations and their status
```

**Drop Database**
```bash
dotnet ef database drop --force
# Deletes the database (WARNING: data loss)
```

**Revert to Previous Migration**
```bash
dotnet ef database update PreviousMigration
# Reverts to named migration
```

### Building & Publishing

**Build (Debug)**
```bash
dotnet build
# Creates debug binaries
```

**Build (Release)**
```bash
dotnet build -c Release
# Creates optimized release binaries
```

**Publish for Deployment**
```bash
dotnet publish -c Release -o ./publish
# Creates standalone publication in ./publish folder
```

**Run Published Version**
```bash
cd ./publish
dotnet LJ.BillingPortal.API.dll
```

---

## 🔧 Configuration

### Connection String (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=InvoiceDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=False;TrustServerCertificate=True;"
  }
}
```

### CORS Origins (appsettings.json)
```json
{
  "AllowedOrigins": [
    "http://localhost:3000",
    "http://localhost:5001",
    "https://yourdomain.com"
  ]
}
```

### Logging Level (appsettings.json)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Warning",
      "Microsoft": "Warning"
    }
  }
}
```

### Port Configuration (appsettings.json)
```json
{
  "Port": 5001
}
```

---

## 🔐 API Testing

### Using Swagger UI
1. Open: http://localhost:5001
2. Click on endpoint name to expand
3. Click "Try it out"
4. Enter request body (if needed)
5. Click "Execute"

### Using cURL

**Get All Invoices**
```bash
curl -X GET "http://localhost:5001/api/invoices/all" \
  -H "accept: application/json"
```

**Create Invoice**
```bash
curl -X POST "http://localhost:5001/api/invoices/create" \
  -H "Content-Type: application/json" \
  -d @invoice.json
```

**Generate PDF**
```bash
curl -X POST "http://localhost:5001/api/invoices/generate-pdf" \
  -H "Content-Type: application/json" \
  -d @invoice.json
```

### Using Postman
1. Import collection from Swagger: http://localhost:5001/swagger/v1/swagger.json
2. Set environment variables if needed
3. Test each endpoint

---

## 📝 Project Structure Quick Reference

```
Controllers/
  └── InvoicesController.cs        # HTTP endpoints

Services/
  ├── Interfaces/
  │   ├── IInvoiceService.cs       # Service contract
  │   └── IPdfGenerationService.cs
  ├── InvoiceService.cs            # Business logic
  └── PdfGenerationService.cs

Models/
  ├── ClientDetails.cs             # Database entities
  ├── InvoiceDetails.cs
  └── InvoiceParticular.cs

DTOs/
  ├── ClientDetailsDto.cs
  ├── InvoiceDetailsDto.cs
  ├── InvoiceParticularDto.cs
  └── InvoiceCompositeDto.cs

Data/
  └── BillingPortalDbContext.cs    # EF Core DbContext

Validators/
  ├── ClientDetailsValidator.cs
  ├── InvoiceDetailsValidator.cs
  ├── InvoiceParticularValidator.cs
  └── CompositeInvoiceValidator.cs

Middleware/
  └── GlobalExceptionHandlerMiddleware.cs

Exceptions/
  └── ApiExceptions.cs

Migrations/
  ├── 20260306000001_InitialCreate.cs
  ├── BillingPortalDbContextModelSnapshot.cs
  └── BillingPortalDbContextFactory.cs
```

---

## 🧪 Debugging

### Enable Detailed Logging

**appsettings.Development.json**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information"
    }
  },
  "DetailedErrors": true
}
```

### Common Issues

**Issue: "Connection Error"**
```
Solution: Check SQL Server is running
          Verify connection string in appsettings.json
          Check firewall settings
```

**Issue: "Migration Not Found"**
```
Solution: Run: dotnet ef migrations list
          Verify migration exists in Migrations folder
          Check DbContextFactory has correct connection string
```

**Issue: "Port Already in Use"**
```
Solution: Change port in appsettings.json
          Or use: dotnet run -- --urls "http://localhost:8080"
```

**Issue: "PDF Generation Error"**
```
Solution: Ensure SelectPdf license/lib is installed
          Check wwwroot/invoices folder permissions
          Verify SelectPdf NuGet package is restored
```

---

## 💡 Code Examples

### Create Invoice (Programmatically)
```csharp
var request = new CreateCompleteInvoiceDto
{
    ClientDetails = new CreateClientDetailsDto
    {
        BilledToName = "Company Ltd",
        AddressLine1 = "123 Main St",
        AddressLine2 = "Suite 100",
        AddressLine3 = "City, State",
        Gstin = "27AABCU9603R1Z0",
        State = "Telangana",
        StateCode = "TS"
    },
    InvoiceDetails = new CreateInvoiceDetailsDto
    {
        InvoiceDate = DateTime.Now,
        PlaceOfSupply = "Hyderabad",
        PoNumber = "PO123",
        CraneReg = "CR001",
        TotalAmountBeforeTax = 50000,
        Cgst = 4500,
        Sgst = 4500,
        Igst = 0,
        NetAmountAfterTax = 59000
    },
    InvoiceParticulars = new List<CreateInvoiceParticularDto>
    {
        new CreateInvoiceParticularDto
        {
            Description = "Service",
            HsnSac = "9966",
            Quantity = 1,
            Rate = 50000,
            TaxableValue = 50000
        }
    }
};

var result = await _invoiceService.CreateInvoiceAsync(request);
```

### Query Invoices (LINQ)
```csharp
// Get invoices for last 30 days
var invoices = await _dbContext.InvoiceDetails
    .Where(i => i.CreatedDate >= DateTime.UtcNow.AddDays(-30))
    .Include(i => i.Client)
    .Include(i => i.Particulars)
    .OrderByDescending(i => i.CreatedDate)
    .ToListAsync();
```

### Add Logging
```csharp
_logger.LogInformation("Creating invoice for client {ClientId}", clientId);
_logger.LogWarning("No invoices found for date range");
_logger.LogError(ex, "Error processing invoice {InvoiceNumber}", number);
```

### Validation in Code
```csharp
var validator = new CreateInvoiceDetailsDtoValidator();
var result = await validator.ValidateAsync(request);

if (!result.IsValid)
{
    var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
    throw new ValidationException(/* ... */);
}
```

---

## 📊 Monitoring & Health Checks

### Health Check Endpoint
```bash
curl http://localhost:5001/health

Response:
{
  "status": "Healthy",
  "checks": {
    "EntityFrameworkCore": "Healthy"
  }
}
```

### View Logs
```bash
# View last 100 lines
tail -100 logs/api-20260306.txt

# Follow logs in real-time (PowerShell)
Get-Content logs/api-*.txt -Wait -Tail 20
```

---

## 🔄 Dependency Injection Reference

### Registered Services (Program.cs)
```csharp
// Database
services.AddDbContext<BillingPortalDbContext>()

// Services
services.AddScoped<IInvoiceService, InvoiceService>()
services.AddScoped<IPdfGenerationService, PdfGenerationService>()

// Validators
services.AddValidatorsFromAssemblyContaining()

// CORS
services.AddCors()

// Endpoints Explorer & Swagger
services.AddEndpointsApiExplorer()
services.AddSwaggerGen()
```

### Inject in Controller
```csharp
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(
        IInvoiceService invoiceService,
        ILogger<InvoicesController> logger)
    {
        _invoiceService = invoiceService;
        _logger = logger;
    }
}
```

---

## 🎯 Performance Tips

1. **Use Async/Await** - All DB calls should be async
2. **Use Include() for Relationships** - Avoid N+1 queries
3. **Use Indexes** - Database has indexes on GSTIN, InvoiceNumber
4. **Use Connection Pooling** - EF Core does this automatically
5. **Cache PDF Generation** - Generate once, serve from wwwroot

---

## 📞 Support & Documentation

**Official Resources:**
- .NET Documentation: https://learn.microsoft.com/en-us/dotnet/
- Entity Framework Core: https://learn.microsoft.com/en-us/ef/core/
- FluentValidation: https://fluentvalidation.net/
- Serilog: https://serilog.net/

**Project Files:**
- README_DOTNET.md - Comprehensive guide
- MIGRATION_REPORT.md - Migration details
- appsettings.json - Configuration template

---

## 🚨 Critical Files to Remember

| File | Purpose | Edit? |
|------|---------|-------|
| Program.cs | Entry point, DI setup | ⚠️ Carefully |
| appsettings.json | Configuration | ✅ Often |
| BillingPortalDbContext.cs | Database config | ⚠️ Carefully |
| Migrations/* | Database changes | ✅ Auto-generated |
| Controllers/* | API endpoints | ✅ Modify logic |
| Services/* | Business logic | ✅ Modify logic |

---

**Happy coding! 🚀**
