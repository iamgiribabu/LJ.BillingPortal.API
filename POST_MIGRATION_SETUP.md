# Post-Migration Setup Guide

## ✅ Migration Complete - Next Steps

The LJ.BillingPortal.API has been **successfully migrated to .NET 9 Web API Core**. Follow these steps to get it running in your environment.

---

## 📋 Pre-Flight Checklist

Before starting, verify you have:

- [ ] .NET 9 SDK installed
- [ ] SQL Server 2019 or later running
- [ ] Visual Studio 2022 or VS Code
- [ ] Git (for version control)
- [ ] Network access to SQL Server

Check versions:
```bash
dotnet --version
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
```

---

## 🔧 Step 1: Environment Setup

### 1.1 Clone/Navigate to Project
```bash
cd "d:\LJ lifters files\Billingprotal\LJ.BillingPortal.API"
```

### 1.2 Review Configuration
```bash
# Check appsettings.json
cat appsettings.json

# Default configuration:
# - Server: localhost
# - Database: InvoiceDB
# - User: Admin_Giribabu
# - Port: 5001
```

### 1.3 Verify SQL Server Connection
```bash
# Test connection using sqlcmd
sqlcmd -S localhost -U Admin_Giribabu -P "$LJLpass01" -Q "SELECT 1"

# If successful, you'll see: (1 row affected)
# If error, fix SQL Server configuration
```

---

## 📦 Step 2: Restore Dependencies

### 2.1 Restore NuGet Packages
```bash
dotnet restore

# Output should show:
# Restoring LJ.BillingPortal.API.csproj
# ...
# Restore completed
```

### 2.2 Verify Packages
```bash
dotnet list package

# Shows all NuGet packages and versions
```

---

## 🗄️ Step 3: Database Setup

### 3.1 Create Database
```bash
dotnet ef database update

# Output should show:
# Build started...
# Build completed...
# Applying migration '20260306000001_InitialCreate'
# Done. To undo this action, use 'ef migrations remove'
```

### 3.2 Verify Database Creation
```bash
# Connect to SQL Server and check
sqlcmd -S localhost -U Admin_Giribabu -P "$LJLpass01"

# In sqlcmd prompt:
> SELECT name FROM sys.databases WHERE name = 'InvoiceDB'
> GO

# Should return: InvoiceDB
```

### 3.3 Check Tables Created
```bash
# In SQL Server:
USE InvoiceDB;
SELECT name FROM sys.tables;

# Should show:
# ClientDetails
# InvoiceDetails
# InvoiceParticulars
```

---

## 🚀 Step 4: Run the Application

### 4.1 Start in Development Mode
```bash
dotnet run

# Output should show:
# info: Microsoft.Hosting.Lifetime[14]
#     Now listening on: http://localhost:5001
#     Application started. Press Ctrl+C to exit.
```

### 4.2 Access Swagger UI
Open browser and navigate to: **http://localhost:5001**

You should see:
- Swagger UI with all endpoints listed
- Green "Authorize" button (for future JWT implementation)
- All invoke buttons for testing endpoints

### 4.3 Test Health Check
```bash
curl http://localhost:5001/health

# Response should be:
# {"status":"Healthy","checks":{"EntityFrameworkCore":"Healthy"}}
```

---

## ✨ Step 5: Verify All Endpoints

### 5.1 Test via Swagger UI

1. Click "GET /api/invoices/all"
2. Click "Try it out"
3. Click "Execute"
4. Should return: `[]` (empty array, no invoices yet)

**Repeat for other endpoints:**
- GET /api/invoices/next-number → Returns: `{"nextInvoiceNumber":"1001"}`
- GET /api/invoices/clients → Returns: `[]`
- GET /health → Returns: `{"status":"Healthy",...}`

### 5.2 Test Invoice Creation

```bash
# Create test invoice
curl -X POST "http://localhost:5001/api/invoices/create" \
  -H "Content-Type: application/json" \
  -d '{
    "clientDetails": {
      "billedToName": "Test Company",
      "addressLine1": "123 Main Street",
      "addressLine2": "Suite 100",
      "addressLine3": "City, State 12345",
      "gstin": "27AABCU9603R1Z0",
      "state": "Telangana",
      "stateCode": "TS"
    },
    "invoiceDetails": {
      "invoiceDate": "2024-01-15",
      "placeOfSupply": "Hyderabad",
      "poNumber": "PO001",
      "craneReg": "CR001",
      "totalAmountBeforeTax": 10000,
      "cgst": 900,
      "sgst": 900,
      "igst": 0,
      "netAmountAfterTax": 11800
    },
    "invoiceParticulars": [
      {
        "description": "Test Service",
        "hsnSac": "9966",
        "quantity": 1,
        "rate": 10000,
        "taxableValue": 10000
      }
    ]
  }'

# Response: 201 Created with invoice details
```

---

## 📚 Step 6: Review Documentation

Read these files in order:

1. **README_DOTNET.md** - Comprehensive API documentation
2. **MIGRATION_REPORT.md** - Detailed migration information  
3. **QUICK_REFERENCE.md** - Developer quick reference
4. **ARCHITECTURE.md** - Design patterns and architecture

---

## 🔒 Step 7: Security Configuration

### 7.1 Update appsettings.json for Production
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=PROD_SERVER;Database=InvoiceDB;User Id=PROD_USER;Password=PROD_PASSWORD;Encrypt=True;TrustServerCertificate=False;"
  },
  "AllowedOrigins": [
    "https://yourdomain.com"
  ],
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

### 7.2 Enable HTTPS
```bash
# Generate HTTPS certificate (development)
dotnet dev-certs https --trust

# In appsettings.json, update URLs for HTTPS
```

### 7.3 Implement Authentication (Optional but Recommended)
```csharp
// Add to Program.cs:
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        // Configure JWT validation
    });
```

---

## 🧪 Step 8: Development Workflow

### 8.1 Making Code Changes

**Change service logic:**
```bash
# Edit Services/InvoiceService.cs
# Save file → Automatic hot reload (if using watch mode)
```

**Hot reload with watch:**
```bash
dotnet watch run

# Changes automatically reload the app
# Press Ctrl+C to stop
```

### 8.2 Adding New Migrations

**When you modify models:**
```bash
# 1. Update your model class
# 2. Create migration
dotnet ef migrations add YourMigrationName

# 3. Review generated migration file in Migrations/
# 4. Apply migration
dotnet ef database update
```

### 8.3 Debugging

**Debug in Visual Studio:**
1. Open project in Visual Studio 2022
2. Press F5 or click "Start Debugging"
3. Set breakpoints in code
4. Test via Swagger UI

**Debug in VS Code:**
1. Install C# extension
2. Create .vscode/launch.json
3. Press F5 to start debugging

---

## 📊 Step 9: Monitoring & Logs

### 9.1 View Application Logs
```bash
# Log file location
ls logs/api-*.txt

# View latest logs
tail -f logs/api-*.txt
```

### 9.2 Check Debug Output
```bash
# In Visual Studio: View → Output (Debug)
# Look for information, warnings, and errors
```

### 9.3 Monitor Database
```sql
-- Check active connections
SELECT * FROM sys.dm_exec_sessions WHERE database_id = DB_ID('InvoiceDB')

-- View recent queries
SELECT * FROM sys.dm_exec_requests

-- Check table sizes
EXEC sp_spaceused 'InvoiceDetails'
```

---

## 🚢 Step 10: Deployment Preparation

### 10.1 Build for Release
```bash
dotnet build -c Release
```

### 10.2 Create Publish Package
```bash
dotnet publish -c Release -o ./publish
```

### 10.3 Create Docker Container (Optional)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY ./publish .

ENTRYPOINT ["dotnet", "LJ.BillingPortal.API.dll"]
```

**Build Docker image:**
```bash
docker build -t lj-billingportal-api:1.0 .

# Run container
docker run -p 5001:5001 -e "ConnectionStrings__DefaultConnection=..." lj-billingportal-api:1.0
```

---

## 🆘 Troubleshooting

### Issue: Migrations Don't Apply
```bash
# Solution
dotnet ef database update --verbose
# Shows detailed migration execution
```

### Issue: SQL Server Connection Fails
```bash
# Check connection string format
# Verify SQL Server is running: net start MSSQL$SQLEXPRESS (for Express)
# Check firewall ports (default: 1433)
```

### Issue: Swagger UI Not Loading
```bash
# Clear cache
# Hard refresh browser (Ctrl+Shift+R)
# Check console for JavaScript errors
```

### Issue: Port 5001 Already in Use
```bash
# Kill the process on port 5001
# Windows PowerShell:
Stop-Process -Id (Get-NetTCPConnection -LocalPort 5001).OwningProcess -Force

# Or change port in appsettings.json:
"Port": 8080
```

---

## ✅ Verification Checklist

After setup, verify everything works:

- [ ] Project builds without errors (`dotnet build`)
- [ ] Database created and contains tables
- [ ] Application starts (`dotnet run`)
- [ ] Swagger UI loads at http://localhost:5001
- [ ] Health check returns Healthy
- [ ] GET /api/invoices/all returns [] (or data if existing)
- [ ] Can create invoice via POST /api/invoices/create
- [ ] Logs are being written to logs/ folder
- [ ] Configuration loaded from appsettings.json
- [ ] No errors in debug output

---

## 🎯 Next Development Tasks

### Phase 1: Core Features
- [ ] Add unit tests for services
- [ ] Add integration tests for controllers
- [ ] Implement input sanitization
- [ ] Add pagination for list endpoints

### Phase 2: Security & Performance
- [ ] Implement JWT authentication
- [ ] Add role-based access control
- [ ] Add request throttling/rate limiting
- [ ] Implement output caching

### Phase 3: Advanced Features
- [ ] Add invoice search/filtering
- [ ] Implement audit logging
- [ ] Add batch invoice operations
- [ ] Implement webhook notifications

### Phase 4: DevOps
- [ ] Setup CI/CD pipeline (Azure DevOps/GitHub Actions)
- [ ] Configure database backup strategy
- [ ] Setup monitoring and alerting
- [ ] Document deployment procedures

---

## 📞 Support & Resources

**Documentation Files:**
- README_DOTNET.md - Full API documentation
- MIGRATION_REPORT.md - Migration details
- QUICK_REFERENCE.md - Command quick reference
- This file - Setup guide

**External Resources:**
- [.NET 9 Docs](https://learn.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Docs](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [SQL Server Docs](https://learn.microsoft.com/en-us/sql/sql-server/)

---

## 🎉 Success!

Once you've completed all steps and verified everything is working:

1. ✅ Application is running locally
2. ✅ Database is created with all tables
3. ✅ All endpoints are functional
4. ✅ Documentation is reviewed

**You're ready to start development! 🚀**

For any questions, refer to:
- MIGRATION_REPORT.md (why things changed)
- ARCHITECTURE.md (how things are organized)
- QUICK_REFERENCE.md (how to do common tasks)

---

**Happy coding!**
