# Quick Start Guide - LJ.BillingPortal.API

## 🚀 Starting the Application

### Prerequisites
✅ Ensure SQL Server LocalDB is running:
```powershell
sqllocaldb start MSSQLLocalDB
```

### Option 1: Using Visual Studio

1. **Open the Solution**
   - File → Open → Solution
   - Navigate to `LJ.BillingPortal.API.sln`

2. **Build Solution**
   - Build → Build Solution (or Ctrl+Shift+B)
   - ✅ Verify "Build succeeded"

3. **Start Debugging**
   - Press `F5` or click the green ▶ button
   - Select `LJ.BillingPortal.API` as startup project

4. **Access the API**
   - Browser will open to: `http://localhost:5001`
   - Swagger UI will load automatically
   - API is ready to use

### Option 2: Using Command Line (PowerShell)

```powershell
# Navigate to project directory
cd "D:\LJ lifters files\Billingprotal\LJ.BillingPortal.API"

# Start the application
dotnet run

# Output will show:
# info: LJ.BillingPortal.API.Program[0]
#       Application starting on port 5001
```

### Option 3: Using Command Line (Terminal)

```bash
cd "D:\LJ lifters files\Billingprotal\LJ.BillingPortal.API"
dotnet run
```

---

## 🌐 Accessing the API

### Swagger UI (API Documentation)
- **URL**: `http://localhost:5001`
- **Features**:
  - ✅ View all endpoints
  - ✅ Try endpoints directly
  - ✅ See request/response schemas
  - ✅ Authentication testing

### Health Check Endpoint
- **URL**: `http://localhost:5001/health`
- **Purpose**: Verify API is running and database is connected

### API Base URL
- **URL**: `http://localhost:5001/api`
- **Endpoints**:
  - `GET /api/invoices/all` - Get all invoices
  - `GET /api/invoices/next-number` - Get next invoice number
  - `POST /api/invoices/create` - Create new invoice
  - `POST /api/invoices/generate-pdf` - Generate invoice PDF
  - `GET /api/invoices/clients` - Get all clients
  - `PUT /api/invoices/clients` - Update client
  - `PUT /api/invoices/details` - Update invoice
  - `PUT /api/invoices/particulars` - Update particular

---

## ⚙️ Configuration

### appsettings.json
```json
{
  "Port": 5001,                           // ← API Port
  "ConnectionStrings": {
    "BillingPortalDBConnection": "Data Source=(localdb)\\MSSQLLocalDB;..."
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"            // ← Log Level
    }
  }
}
```

### Common Configuration Changes

**Change Port:**
```json
"Port": 5002  // Change to desired port
```

**Change Log Level (More/Less Verbose):**
```json
"LogLevel": {
  "Default": "Debug"      // More verbose
  // or
  "Default": "Warning"    // Less verbose
}
```

---

## 🧪 Testing the API

### Using Swagger UI (Easiest)

1. Navigate to `http://localhost:5001`
2. Click on an endpoint (e.g., `GET /api/invoices/all`)
3. Click **"Try it out"**
4. Click **"Execute"**
5. View the response

### Using PowerShell

```powershell
# Get all invoices
$response = Invoke-RestMethod -Uri "http://localhost:5001/api/invoices/all" -Method Get
$response | ConvertTo-Json

# Create invoice
$body = @{
    clientDetails = @{
        billedToName = "Test Client"
        addressLine1 = "123 Main St"
        # ... more fields
    }
    invoiceDetails = @{
        # ... invoice details
    }
    invoiceParticulars = @()
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:5001/api/invoices/create" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body
```

### Using cURL

```bash
# Get all invoices
curl -X GET "http://localhost:5001/api/invoices/all" \
  -H "Content-Type: application/json"

# Get health status
curl -X GET "http://localhost:5001/health"
```

---

## 🔧 Troubleshooting

### Problem: "Port 5001 already in use"

**Solution:**
```powershell
# Option 1: Kill process using the port
netstat -ano | findstr :5001
taskkill /PID <PID> /F

# Option 2: Use different port (change in appsettings.json)
"Port": 5002
```

### Problem: "SQL Server connection failed"

**Solution:**
```powershell
# Start SQL Server LocalDB
sqllocaldb start MSSQLLocalDB

# Verify it's running
sqllocaldb info MSSQLLocalDB
```

### Problem: "Database tables not found"

**Solution:**
```powershell
# Database is created on first run
# If issues, manually run migrations:
cd "D:\LJ lifters files\Billingprotal\LJ.BillingPortal.API"
dotnet ef database update
```

### Problem: "Swagger UI not loading"

**Solution:**
- Ensure `app.Environment.IsDevelopment()` is true
- Check `launchSettings.json`:
  ```json
  "ASPNETCORE_ENVIRONMENT": "Development"
  ```

---

## 📊 Logs and Debugging

### View Application Logs

**Console Output:**
```
info: LJ.BillingPortal.API.Program[0]
      Starting LJ.BillingPortal.API Web API Core
info: Microsoft.EntityFrameworkCore.Database.Connection[20000]
      Opened connection to database
info: LJ.BillingPortal.API.Services.InvoiceService[0]
      Fetching all invoices
info: LJ.BillingPortal.API.Program[0]
      Application starting on port 5001
```

**File Logs:**
- Location: `logs/api-<date>.txt`
- Rolling daily
- Contains all operations and errors

### Increase Log Verbosity

**appsettings.Development.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "LJ.BillingPortal.API": "Debug",
      "Microsoft.EntityFrameworkCore": "Debug"
    }
  }
}
```

---

## 🎯 Sample API Call

### Create Invoice with Parallel Processing

**Request:**
```http
POST http://localhost:5001/api/invoices/create
Content-Type: application/json

{
  "clientDetails": {
    "billedToName": "ABC Manufacturing",
    "addressLine1": "123 Industrial Park",
    "addressLine2": "Building A",
    "addressLine3": "Suite 200",
    "gstin": "29ABCDE1234F1Z0",
    "state": "Karnataka",
    "stateCode": "KA"
  },
  "invoiceDetails": {
    "invoiceDate": "2026-03-06T00:00:00",
    "placeOfSupply": "Karnataka",
    "poNumber": "PO-2024-001",
    "craneReg": "CR-2024-001",
    "totalAmountBeforeTax": 100000,
    "cgst": 9000,
    "sgst": 9000,
    "igst": 0,
    "netAmountAfterTax": 118000
  },
  "invoiceParticulars": [
    {
      "description": "Crane Rental Service",
      "hsnSac": "999314",
      "quantity": 10,
      "rate": 5000,
      "taxableValue": 50000
    },
    {
      "description": "Operator Charges",
      "hsnSac": "999314",
      "quantity": 10,
      "rate": 5000,
      "taxableValue": 50000
    }
  ]
}
```

**Response (Success):**
```json
{
  "invoiceId": 1,
  "clientDetails": {
    "clientId": 1,
    "billedToName": "ABC Manufacturing",
    ...
  },
  "invoiceDetails": {
    "invoiceNumber": "1001",
    ...
  },
  "invoiceParticulars": [
    { "serviceId": 1, ... },
    { "serviceId": 2, ... }
  ]
}
```

**Response (Failure - Automatic Rollback):**
```json
{
  "message": "Failed to add invoice particulars: Failed to insert particular for Invoice ID 1: Duplicate HSN/SAC",
  "failedOperations": ["ServiceId: 2"]
}
```

---

## ✅ Verification Checklist

- [ ] SQL Server LocalDB is running
- [ ] Build succeeds (no compilation errors)
- [ ] Application starts on port 5001
- [ ] Swagger UI loads at `http://localhost:5001`
- [ ] Health check passes at `http://localhost:5001/health`
- [ ] Can create test invoice via Swagger
- [ ] Logs appear in console and `logs/` directory
- [ ] Database connection works (no connection errors)

---

## 📚 Documentation

For more detailed information, see:
- **IMPLEMENTATION_SUMMARY.md** - Complete implementation overview
- **PARALLEL_PROCESSING_README.md** - Parallel processing details
- **PARALLEL_PROCESSING_GUIDE.md** - Technical deep dive
- **ARCHITECTURE_DIAGRAMS.md** - System architecture diagrams

---

**Ready to go!** 🚀 Start with Option 1 (Visual Studio) for the easiest experience.
