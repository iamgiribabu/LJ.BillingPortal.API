# 🔧 Swagger Access - Troubleshooting Guide

## ✅ Fixed Issues

Your `launchSettings.json` has been updated with:
- ✅ Removed HTTPS (not needed for local development)
- ✅ Set port to `5001` (only port)
- ✅ Set `launchUrl` to empty string (Swagger loads at root)

---

## 🎯 Correct Access Method

### **URL You Should Use:**
```
http://localhost:5001
```

### **NOT These:**
```
❌ https://localhost:5002/swagger    (Wrong port, protocol, and path)
❌ http://localhost:5001/swagger     (Wrong path - swagger is at root)
❌ https://localhost:5001            (Wrong protocol for development)
```

---

## 🚀 How to Start Correctly

### **Method 1: Visual Studio (Best)**
1. Open **Solution Explorer**
2. Right-click project → **Set as Startup Project**
3. Select the **"http" profile** in the debug dropdown (NOT "LJ.BillingPortal.API")
4. Press **F5** (Debug) or **Ctrl+F5** (Run without debug)
5. Browser automatically opens to `http://localhost:5001`

### **Method 2: Command Line**
```powershell
# Navigate to project directory
cd "D:\LJ lifters files\Billingprotal\LJ.BillingPortal.API"

# Run the application
dotnet run

# Output will show:
# info: LJ.BillingPortal.API[0]
#       Application starting on port 5001
```

### **Method 3: Terminal/CMD**
```bash
cd "D:\LJ lifters files\Billingprotal\LJ.BillingPortal.API"
dotnet run
```

---

## ✅ What Should Happen

1. Application starts
2. Console shows: `Application starting on port 5001`
3. Browser automatically opens
4. Swagger UI loads at `http://localhost:5001`
5. You see all API endpoints listed

### **Console Output (Success):**
```
info: LJ.BillingPortal.API.Program[0]
      Starting LJ.BillingPortal.API Web API Core
...
info: LJ.BillingPortal.API.Program[0]
      Application starting on port 5001
```

---

## 🌐 Access Points After Starting

| Endpoint | URL | Purpose |
|----------|-----|---------|
| **Swagger UI** | `http://localhost:5001` | API Documentation & Testing |
| **Health Check** | `http://localhost:5001/health` | Verify API is running |
| **Get All Invoices** | `http://localhost:5001/api/invoices/all` | Retrieve invoices |
| **Create Invoice** | `http://localhost:5001/api/invoices/create` | Create new invoice |

---

## 🆘 Still Having Issues?

### **Issue: Port 5001 already in use**
```powershell
# Find what's using port 5001
netstat -ano | findstr :5001

# Kill the process (replace PID)
taskkill /PID <PID> /F

# OR change port in appsettings.json
"Port": 5002
```

### **Issue: SQL Server not connected**
```powershell
# Start LocalDB
sqllocaldb start MSSQLLocalDB

# Verify it's running
sqllocaldb info MSSQLLocalDB
```

### **Issue: Still can't access**
1. Stop the application (Ctrl+C or Stop button)
2. Wait 5 seconds
3. Press **F5** again
4. Use `http://localhost:5001` (not HTTPS)
5. If browser is stuck on old URL, **Ctrl+R** to refresh or open new tab

---

## 📋 Updated Launch Settings

Your `Properties\launchSettings.json` now has:

```json
"http": {
  "commandName": "Project",
  "dotnetRunMessages": true,
  "launchBrowser": true,
  "launchUrl": "",              // ✅ Empty = root path
  "applicationUrl": "http://localhost:5001",  // ✅ HTTP only, port 5001
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development"
  }
}
```

---

## 🎓 Remember

- **Development**: Use `http://` (not https)
- **Swagger**: Accessible at root `/` (not `/swagger`)
- **Port**: `5001` (as configured in appsettings.json)
- **Browser**: Any modern browser (Chrome, Edge, Firefox)

---

## ✅ Next Steps

1. **Start the application** with F5 or `dotnet run`
2. **Wait for it to start** (2-3 seconds)
3. **Use this URL**: `http://localhost:5001`
4. **Swagger UI appears** with all endpoints listed
5. **Click "Try it out"** on any endpoint to test

---

**You're all set! 🚀 Start the app and access `http://localhost:5001`**
