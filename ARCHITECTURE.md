# Architecture & Design Patterns Documentation

## 🏗️ System Architecture Overview

The LJ.BillingPortal.API follows a **4-tier layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────────────────┐
│             API Gateway / Load Balancer             │
├─────────────────────────────────────────────────────┤
│          Presentation Layer (Controllers)           │
│    - HTTP Request Handling                          │
│    - Route Mapping                                  │
├─────────────────────────────────────────────────────┤
│          Application/Service Layer                   │
│    - Business Logic                                 │
│    - Orchestration                                  │
│    - Transformations                                │
├─────────────────────────────────────────────────────┤
│          Data Access Layer (EF Core)                │
│    - Database Queries                               │
│    - Entity Mapping                                 │
│    - Connection Management                          │
├─────────────────────────────────────────────────────┤
│              Persistence Layer                       │
│              (SQL Server Database)                   │
└─────────────────────────────────────────────────────┘
        ↓
    Validators (Parallel)
        ↓
    Middleware (Cross-cutting)
```

---

## 🎯 Design Principles (SOLID)

### 1. Single Responsibility Principle (SRP)
Each class has one reason to change:

```csharp
// ✅ GOOD: Separated concerns
InvoiceService          // Only handles invoice business logic
PdfGenerationService    // Only handles PDF generation
ClientDetailsValidator  // Only validates client details

// ❌ BAD: Multiple responsibilities
InvoiceController       // Used to have validation, logging, PDF generation
```

### 2. Open/Closed Principle (OCP)
Classes are open for extension, closed for modification:

```csharp
// ✅ GOOD: Interface-based, can extend without modifying
public interface IInvoiceService { }
public class InvoiceService : IInvoiceService { }
// Can create InvoiceServiceCached without modifying existing

// Usage allows polymorphism:
services.AddScoped<IInvoiceService>(
    provider => new InvoiceService(dbContext, logger)
);
```

### 3. Liskov Substitution Principle (LSP)
Derived classes can substitute base classes:

```csharp
// ✅ GOOD: All validators follow same contract
IValidator<TRequest> validator
// Any validator implementing IValidator can be used interchangeably
```

### 4. Interface Segregation Principle (ISP)
Clients shouldn't depend on interfaces they don't use:

```csharp
// ✅ GOOD: Small, focused interfaces
public interface IInvoiceService 
{
    Task<List<InvoiceResponseDto>> GetAllInvoicesAsync();
    Task<InvoiceResponseDto> CreateInvoiceAsync(CreateCompleteInvoiceDto request);
    // Only invoice operations
}

// ❌ BAD: Fat interface
public interface IService
{
    Task<T> GetAll<T>();
    Task<T> Create<T>(T request);
    Task<T> Update<T>(T request);
    Task Delete<T>(long id);
    // Too many responsibilities
}
```

### 5. Dependency Inversion Principle (DIP)
Depend on abstractions, not concretions:

```csharp
// ✅ GOOD: Depends on abstractions
public class InvoicesController
{
    private readonly IInvoiceService _service;  // Abstraction
    
    public InvoicesController(IInvoiceService service)
    {
        _service = service;  // Injected, not created
    }
}

// ❌ BAD: Direct dependency
public class InvoicesController
{
    private readonly InvoiceService _service = new InvoiceService();  // Concrete
}
```

---

## 📐 Design Patterns Implemented

### 1. Dependency Injection (DI)

**Purpose:** Loose coupling, testability, flexibility

**Implementation:**
```csharp
// Program.cs
services.AddScoped<IInvoiceService, InvoiceService>();
services.AddScoped<IPdfGenerationService, PdfGenerationService>();
services.AddValidatorsFromAssemblyContaining<CreateClientDetailsDtoValidator>();

// Usage in controller
public InvoicesController(
    IInvoiceService invoiceService,
    IPdfGenerationService pdfService,
    IValidator<CreateCompleteInvoiceDto> validator)
{
    // Dependencies injected by ASP.NET Core container
}
```

**Benefits:**
- Easy to mock for testing
- Can swap implementations (e.g., caching decorator)
- Lifecycle management (Singleton, Scoped, Transient)

### 2. Repository Pattern

**Purpose:** Abstract data access, enable testing

**Implementation:**
```csharp
// Entity Framework Core acts as the Repository
// DbContext abstracts SQL Server
public class InvoiceService
{
    private readonly BillingPortalDbContext _dbContext;
    
    public async Task<List<InvoiceResponseDto>> GetAllInvoicesAsync()
    {
        // LINQ query abstraction over SQL
        var invoices = await _dbContext.InvoiceDetails
            .Include(i => i.Client)
            .Include(i => i.Particulars)
            .ToListAsync();
    }
}
```

**Benefits:**
- Data access logic centralized
- Easy to change database without changing business logic
- Testable with InMemory DbContext

### 3. Data Transfer Object (DTO)

**Purpose:** API contract stability, security, performance

**Implementation:**
```csharp
// Domain Model - Internal representation
public class InvoiceDetails
{
    public int InvoiceId { get; set; }
    public int ClientId { get; set; }
    public string InvoiceNumber { get; set; }
    public List<InvoiceParticular> Particulars { get; set; }
}

// DTO - API contract
public class InvoiceDetailsDto
{
    public int InvoiceId { get; set; }
    public int ClientId { get; set; }
    public string InvoiceNumber { get; set; }
    // NO Particulars here - controlled exposure
}

// Mapping
var dto = new InvoiceDetailsDto
{
    InvoiceId = entity.InvoiceId,
    ClientId = entity.ClientId,
    InvoiceNumber = entity.InvoiceNumber
};
```

**Benefits:**
- API contract independence
- Hide internal implementation
- Version compatibility
- Security (expose only needed data)

### 4. Middleware Pipeline

**Purpose:** Cross-cutting concerns (logging, error handling, etc.)

**Implementation:**
```csharp
// Program.cs - Middleware order matters (FIFO for requests, LIFO for responses)
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();  // First
app.UseHttpsRedirection();  // Second
app.UseRouting();           // Third

// The middleware:
public class GlobalExceptionHandlerMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);  // Pass to next middleware
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
}
```

**Benefits:**
- Centralized cross-cutting concerns
- Consistent error handling
- Request/response transformation
- Authentication, logging, etc.

### 5. Factory Pattern

**Purpose:** Create DbContext for migrations without dependency injection

**Implementation:**
```csharp
public class BillingPortalDbContextFactory : IDesignTimeDbContextFactory<BillingPortalDbContext>
{
    public BillingPortalDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BillingPortalDbContext>();
        optionsBuilder.UseSqlServer("connection string");
        return new BillingPortalDbContext(optionsBuilder.Options);
    }
}
```

**Benefits:**
- Enables `dotnet ef` commands
- Called at design-time (migrations)
- Required for EF Core tooling

### 6. Async/Await Pattern

**Purpose:** Non-blocking I/O, scalability

**Implementation:**
```csharp
// All I/O operations are async
public async Task<List<InvoiceResponseDto>> GetAllInvoicesAsync()
{
    var invoices = await _dbContext.InvoiceDetails
        .Include(i => i.Client)
        .ToListAsync();  // Non-blocking database call
    
    return invoices.Select(MapToDto).ToList();
}

// Controller
public async Task<ActionResult<List<InvoiceResponseDto>>> GetAllInvoices()
{
    var result = await _invoiceService.GetAllInvoicesAsync();  // Awaits
    return Ok(result);
}
```

**Benefits:**
- Thread pool threads freed up
- High concurrency support
- Better resource utilization

### 7. Decorator Pattern

**Purpose:** Add functionality without modifying original (future caching)

**Implementation (Example):**
```csharp
// Base service
public class InvoiceService : IInvoiceService { }

// Decorator with caching
public class CachedInvoiceService : IInvoiceService
{
    private readonly IInvoiceService _innerService;
    private readonly IMemoryCache _cache;
    
    public CachedInvoiceService(IInvoiceService innerService, IMemoryCache cache)
    {
        _innerService = innerService;
        _cache = cache;
    }
    
    public async Task<List<InvoiceResponseDto>> GetAllInvoicesAsync()
    {
        return await _cache.GetOrCreateAsync("invoices", async entry =>
            await _innerService.GetAllInvoicesAsync()
        );
    }
}

// Usage in DI:
services.AddScoped<InvoiceService>();
services.AddScoped<IInvoiceService>(provider =>
    new CachedInvoiceService(provider.GetRequiredService<InvoiceService>(), cache)
);
```

### 8. Strategy Pattern

**Purpose:** Multiple validation strategies

**Implementation:**
```csharp
// Interface - Strategy contract
public interface IValidator<T>
{
    Task<ValidationResult> ValidateAsync(T instance);
}

// Concrete strategies
public class ClientDetailsValidator : AbstractValidator<CreateClientDetailsDto> { }
public class InvoiceDetailsValidator : AbstractValidator<CreateInvoiceDetailsDto> { }
public class CreateCompleteInvoiceValidator : AbstractValidator<CreateCompleteInvoiceDto> { }

// Usage - client code doesn't care which strategy
IValidator<CreateCompleteInvoiceDto> validator = new CreateCompleteInvoiceValidator();
var result = await validator.ValidateAsync(request);
```

---

## 🗂️ Project Organization

### Folder Structure Rationale

```
Controllers/
├── Purpose: Handle HTTP requests/responses
├── No business logic here
├── All endpoints REST-compliant
└── Delegates to services

Services/
├── Purpose: Business logic orchestration
├── Interfaces/ folder contains contracts
├── Implementations not directly referenced
└── All async operations

Models/
├── Purpose: EF Core entities mapping to database
├── Attributes for column mapping
├── Navigation properties for relationships
└── One entity per file for clarity

DTOs/
├── Purpose: API contracts and data transfer
├── Create/Update/Response variants
├── Request input Models
└── Response output Models

Data/
├── Purpose: Database access abstraction
├── BillingPortalDbContext.cs
├── Fluent API configuration
└── Migrations related

Validators/
├── Purpose: Input validation rules
├── FluentValidation based
├── One validator per DTO
└── Reusable across layers

Middleware/
├── Purpose: Cross-cutting concerns
├── Error handling
├── Request/response transformation
└── Shared functionality

Exceptions/
├── Purpose: Custom exception types
├── Domain-specific errors
└── Exception handling semantics

Migrations/
├── Purpose: Version control for schema
├── Automatically generated
└── Manually reviewable if needed
```

---

## 🔄 Data Flow Diagram

```
HTTP Request
    ↓
[GlobalExceptionHandlerMiddleware]
    ↓
Controllers (Route matching)
    ↓
[Validators] Run validation
    ↓
Services (Business logic)
    ↓
EF Core DbContext (Query building)
    ↓
SQL Server (Data retrieval)
    ↓
Entity → DTO Mapping
    ↓
Response JSON
    ↓
[GlobalExceptionHandlerMiddleware]
    ↓
HTTP Response
```

---

## 🗄️ Database Design Patterns

### 1. Entity-Relationship Model

```
ClientDetails (1) ───────────────── (N) InvoiceDetails
                        │
                        │ (1)
                        │
                        └─── (N) InvoiceParticulars
```

**Relationships:**
- One Client has many Invoices
- One Invoice has many Particulars
- Particulars have no direct Client reference (through Invoice)

### 2. Foreign Key Constraints
```sql
ALTER TABLE InvoiceDetails
ADD CONSTRAINT FK_Invoice_Client
FOREIGN KEY (ClientID) REFERENCES ClientDetails(ClientID)
ON DELETE RESTRICT;  -- Prevent orphan invoices

ALTER TABLE InvoiceParticulars
ADD CONSTRAINT FK_Particular_Invoice
FOREIGN KEY (InvoiceID) REFERENCES InvoiceDetails(InvoiceID)
ON DELETE CASCADE;  -- Delete particulars with invoice
```

### 3. Indexing Strategy
```sql
-- Fast lookups
CREATE INDEX IX_ClientDetails_GSTIN ON ClientDetails(GSTIN);
CREATE INDEX IX_InvoiceDetails_InvoiceNumber ON InvoiceDetails(InvoiceNumber);

-- Fast filtering
CREATE INDEX IX_InvoiceDetails_ClientID ON InvoiceDetails(ClientID);
CREATE INDEX IX_InvoiceParticulars_InvoiceID ON InvoiceParticulars(InvoiceID);
```

### 4. Audit Columns
```csharp
public class ClientDetails
{
    // ... other properties
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
    
    // Benefits:
    // - Track when records created/modified
    // - Supports soft delete (future)
    // - Audit trail for compliance
}
```

---

## 📋 Validation Architecture

### Validation Layers

```
1. ASP.NET Core Model Binding
   ↓ (Automatic binding to DTO)

2. FluentValidation (RequestValidators)
   ↓ (DTO-level validation)

3. Business Logic Validation
   ↓ (InvoiceService specific rules)

4. Database Constraints
   (SQL-level constraints as final barrier)
```

### Example Validation Flow

```csharp
[HttpPost("create")]
public async Task<ActionResult<InvoiceResponseDto>> CreateInvoice(
    [FromBody] CreateCompleteInvoiceDto request)  // Step 1: Binding
{
    // Step 2: Validate with FluentValidation
    var result = await _createInvoiceValidator.ValidateAsync(request);
    if (!result.IsValid)
    {
        throw new ValidationException(failures);
    }
    
    // Step 3: Business logic validation in service
    if (invoiceExists)
        throw new BusinessLogicException("Invoice already exists");
    
    // Step 4: Insert to database (DB constraints apply)
    var created = await _invoiceService.CreateInvoiceAsync(request);
    
    return CreatedAtAction(nameof(GetAllInvoices), created);
}
```

---

## 🔐 Security Architecture

### Security Layers

```
1. Input Validation (Prevention)
   - FluentValidation
   - Type safety (C#)

2. SQL Injection Prevention (EF Core)
   - Parameterized queries
   - No string concatenation

3. CORS Policy
   - Whitelist allowed origins
   - Prevent cross-origin attacks

4. Error Handling Middleware
   - Generic error responses
   - No sensitive info leakage

5. Logging (Audit trail)
   - Track operations
   - Compliance ready

6. Future: Authentication/Authorization
   - JWT tokens
   - Role-based access
```

---

## ⚡ Performance Considerations

### 1. Async/Await
All I/O operations non-blocking:
```csharp
// ✅ Non-blocking
var invoices = await _dbContext.InvoiceDetails.ToListAsync();

// ❌ Blocking
var invoices = _dbContext.InvoiceDetails.ToList();
```

### 2. Connection Pooling
```csharp
// EF Core automatic connection pooling
services.AddDbContext<BillingPortalDbContext>(options =>
    options.UseSqlServer(connectionString)
    // Connection pooling enabled by default
);
```

### 3. Eager Loading
```csharp
// ✅ Single query with joins
var invoice = await _dbContext.InvoiceDetails
    .Include(i => i.Client)          // Join on Client
    .Include(i => i.Particulars)     // Join on Particulars
    .FirstOrDefaultAsync(i => i.InvoiceId == id);

// ❌ N+1 queries problem
var invoice = _dbContext.InvoiceDetails.First();  // Query 1
var client = _dbContext.ClientDetails              // Query 2
    .First(c => c.ClientId == invoice.ClientId);
```

### 4. Caching (Future Enhancement)
```csharp
// Decorator pattern allows adding caching later
public class CachedInvoiceService : IInvoiceService
{
    // Cache results
}
```

### 5. Pagination (Future Enhancement)
```csharp
public async Task<PagedResult<InvoiceResponseDto>> GetInvoicesPaged(
    int page = 1, 
    int pageSize = 10)
{
    var total = await _dbContext.InvoiceDetails.CountAsync();
    var invoices = await _dbContext.InvoiceDetails
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return new PagedResult<InvoiceResponseDto>
    {
        Items = invoices,
        Total = total,
        Page = page,
        PageSize = pageSize
    };
}
```

---

## 🧪 Testability

### Service Layer Testing
```csharp
[Test]
public async Task CreateInvoice_WithValidData_ReturnsInvoice()
{
    // Arrange
    var mockDbContext = new Mock<BillingPortalDbContext>();
    var mockLogger = new Mock<ILogger<InvoiceService>>();
    var service = new InvoiceService(mockDbContext.Object, mockLogger.Object);
    var request = new CreateCompleteInvoiceDto { /* ... */ };
    
    // Act
    var result = await service.CreateInvoiceAsync(request);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("1001", result.InvoiceNumber);
    mockDbContext.Verify(m => m.SaveChangesAsync(), Times.Once);
}
```

### Controller Testing
```csharp
[Test]
public async Task CreateInvoice_WithInvalidData_Returns400()
{
    // Arrange
    var mockService = new Mock<IInvoiceService>();
    var mockValidator = new Mock<IValidator<CreateCompleteInvoiceDto>>();
    var controller = new InvoicesController(mockService.Object, mockValidator.Object);
    var request = new CreateCompleteInvoiceDto { /* invalid */ };
    
    // Act & Assert
    Assert.ThrowsAsync<ValidationException>(() => controller.CreateInvoice(request));
}
```

---

## 📈 Scalability Considerations

### Horizontal Scalability
```
Load Balancer
├── API Instance 1 ──┐
├── API Instance 2   ├─→ Shared Database
├── API Instance 3 ──┤
└── API Instance N ──┘
```

**Enabled by:**
- Stateless design
- No in-memory caching (use distributed cache)
- Connection pooling in EF Core

### Vertical Scalability
```
Single Server
├── More CPU cores
├── More RAM for connection pool
├── Faster disk for logging
└── Optimized database queries
```

**Enabled by:**
- Async/await (efficient thread usage)
- LINQ query optimization
- Strategic indexing

---

## 🔄 Evolution & Future Enhancements

### Phase 1: Current State ✅
- Core REST API
- CRUD operations
- Basic validation
- One database instance

### Phase 2: Caching & Performance
```csharp
// Decorator pattern allows:
services.AddScoped<InvoiceService>();
services.AddScoped<IInvoiceService>(sp =>
    new CachedInvoiceService(
        sp.GetRequiredService<InvoiceService>(),
        sp.GetRequiredService<IDistributedCache>()
    )
);
```

### Phase 3: Asynchronous Processing
```csharp
// Background jobs for PDF generation
services.AddHangfire();

public async Task<IActionResult> GenerateInvoicePdfBackground(int id)
{
    BackgroundJob.Enqueue(() => _pdfService.GeneratePdfAsync(id));
    return Accepted();
}
```

### Phase 4: Event-Driven Architecture
```csharp
// Domain events
public class InvoiceCreatedEvent : IDomainEvent
{
    public int InvoiceId { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Event handlers
public class InvoiceCreatedEventHandler : IEventHandler<InvoiceCreatedEvent>
{
    public async Task Handle(InvoiceCreatedEvent evt)
    {
        // Send notification, log to analytics, etc.
    }
}
```

---

## 📚 Resources & References

- [Microsoft - SOLID Principles](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles)
- [Entity Framework Core - Best Practices](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core - Dependency Injection](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [Design Patterns - Refactoring Guru](https://refactoring.guru/design-patterns)

---

**This architecture is production-ready, maintainable, and scalable!**
