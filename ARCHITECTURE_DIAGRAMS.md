# Architecture & Flow Diagrams

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    HTTP Client / Frontend                    │
└──────────────────────────┬──────────────────────────────────┘
                           │ HTTP POST /api/invoices/create
                           ↓
┌─────────────────────────────────────────────────────────────┐
│              InvoicesController (REST API)                    │
│  - Validates Request                                         │
│  - Catches InvoiceCreationException                          │
│  - Returns Response/Error                                    │
└──────────────────────────┬──────────────────────────────────┘
                           │ Call CreateInvoiceAsync()
                           ↓
┌─────────────────────────────────────────────────────────────┐
│            InvoiceService (Business Logic)                    │
│  - CreateInvoiceAsync()                                      │
│  - AddParticularsInParallelAsync()                           │
│  - Error Handling & Logging                                  │
└──────────────────────────┬──────────────────────────────────┘
                           │ Uses
                           ↓
┌─────────────────────────────────────────────────────────────┐
│         IInvoiceRepository (Data Access Interface)            │
│  - AddClientAsync()                                          │
│  - GetNextInvoiceNumberAsync()                               │
│  - AddInvoiceAsync()                                         │
│  - AddParticularAsync()                                      │
│  - BeginTransactionAsync() / CommitAsync() / RollbackAsync() │
└──────────────────────────┬──────────────────────────────────┘
                           │ Implemented by
                           ↓
┌─────────────────────────────────────────────────────────────┐
│      InvoiceRepository (Data Access Implementation)           │
│  - Wraps BillingPortalDbContext                              │
│  - Executes EF Core queries                                  │
│  - Manages transactions                                      │
└──────────────────────────┬──────────────────────────────────┘
                           │ Uses
                           ↓
┌─────────────────────────────────────────────────────────────┐
│      BillingPortalDbContext (Entity Framework Core)           │
│  - DbSet<ClientDetails>                                      │
│  - DbSet<InvoiceDetails>                                     │
│  - DbSet<InvoiceParticular>                                  │
└──────────────────────────┬──────────────────────────────────┘
                           │ Maps to
                           ↓
┌─────────────────────────────────────────────────────────────┐
│           SQL Server LocalDB (Database)                       │
│  - ClientDetails Table                                       │
│  - InvoiceDetails Table                                      │
│  - InvoiceParticulars Table                                  │
│  - __EFMigrationsHistory Table                               │
└─────────────────────────────────────────────────────────────┘
```

## Parallel Processing Flow

```
┌─────────────────────────────────────────────────────────────┐
│           CreateInvoiceAsync(CreateCompleteInvoiceDto)       │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ↓
          ┌──────────────────────────────┐
          │   BEGIN TRANSACTION          │
          └──────────────┬───────────────┘
                         │
                         ↓
      ┌──────────────────────────────────────┐
      │  PHASE 1: PARALLEL INITIALIZATION    │
      │  (Client + Invoice Number)           │
      └────────────┬─────────────────────────┘
                   │
         ┌─────────┴──────────┐
         │                    │
    TASK A              TASK B
    AddClient()         GetNextInvoiceNumber()
         │                    │
         └─────────┬──────────┘
                   ↓
         Task.WhenAll() [WAIT]
                   │
            ┌──────┴──────┐
            │             │
        ✅ SUCCESS    ❌ FAILURE
            │             │
            │          ROLLBACK
            │          Exception
            │             │
            ↓             ↓
      ┌─────────┐    ┌──────────────────┐
      │ Continue │    │ InvoiceCreation  │
      └────┬────┘    │ Exception + Info │
           │         └──────────────────┘
           ↓
      ┌─────────────────────────────────────┐
      │  PHASE 2: SEQUENTIAL               │
      │  Create Invoice                    │
      └────────────┬────────────────────────┘
                   │
            ┌──────┴──────┐
            │             │
        ✅ SUCCESS    ❌ FAILURE
            │             │
            │          ROLLBACK
            │             │
            ↓             ↓
      ┌─────────┐    Exception
      │ Continue│
      └────┬────┘
           │
           ↓
      ┌─────────────────────────────────────┐
      │  PHASE 3: PARALLEL BATCH            │
      │  Add Particulars (Batched)          │
      └────────────┬────────────────────────┘
                   │
         ┌─────────┴──────────────────┐
         │                            │
      BATCH 1                      BATCH 2
    (8 items parallel)           (8 items parallel)
         │                            │
    ┌────┴────────┐            ┌─────┴──────┐
    ↓      ↓      ↓            ↓      ↓     ↓
   ADD   ADD   ADD ADD        ADD   ADD   ADD...
    │      │      │             │      │     │
    └──┬───┴──┬───┴─────────────┴──┬───┴─────┘
       ↓      ↓                    ↓
    AWAIT                      AWAIT
    ALL                         ALL
       │                           │
       └─────────┬─────────────────┘
                 │
            ┌────┴────┐
            │          │
        ✅ SUCCESS  ❌ FAILURE
            │          │
            │      Check Results
            │      Any Failed?
            │          │
            │      ROLLBACK
            │          │
            ↓          ↓
         Continue   Exception
            │          │
            ↓          ↓
      ┌──────────┐  ┌──────────────────┐
      │ Continue │  │ InvoiceCreation  │
      └─────┬────┘  │ Exception + Info │
            │       └──────────────────┘
            ↓
      ┌──────────────────────────┐
      │  COMMIT TRANSACTION      │
      └──────────┬───────────────┘
                 │
                 ↓
      ┌──────────────────────────┐
      │  RETURN InvoiceResponseDto│
      └──────────────────────────┘
```

## Batch Processing Detail

```
Particulars: [P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, ... P100]
Batch Size: 8 (based on CPU cores)

┌─────────────────────────────────────────────────┐
│             BATCH 1 (Items 1-8)                 │
│  P1 ──┐                                         │
│  P2 ──┼──→ Task.WhenAll() ──→ Check Results    │
│  P3 ──┤                       ✅ All Success?   │
│  P4 ──┤                       ↓                 │
│  P5 ──┤                    Continue             │
│  P6 ──┤                                         │
│  P7 ──┤                                         │
│  P8 ──┘                                         │
└─────────────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────────┐
│             BATCH 2 (Items 9-16)                │
│  P9  ──┐                                        │
│  P10 ──┼──→ Task.WhenAll() ──→ Check Results   │
│  P11 ──┤                       ✅ All Success? │
│  ...   ──┘                       ↓              │
│                              Continue          │
└─────────────────────────────────────────────────┘
                     ↓
         ... Continue for all batches ...
                     ↓
┌─────────────────────────────────────────────────┐
│         BATCH 13 (Items 97-100)                 │
│  P97 ──┐                                        │
│  P98 ──┼──→ Task.WhenAll() ──→ Check Results   │
│  P99 ──┤                       ✅ All Success? │
│  P100 ─┘                       ↓               │
│                            Complete            │
└─────────────────────────────────────────────────┘
```

## Error Handling & Rollback Flow

```
┌────────────────────────────────────┐
│  Try to Add Invoice Particular #45 │
└──────────────┬─────────────────────┘
               │
               ↓
        ┌─────────────────┐
        │  INSERT into DB │
        └────────┬────────┘
                 │
            ┌────┴────┐
            ↓         ↓
        SUCCESS   ❌ ERROR
            │     (e.g., Duplicate
            │      HSN/SAC)
            │         │
            ↓         ↓
      ✅ Return    Create
      Success     OperationResult
                  {
                    Success = false,
                    ServiceId = 45,
                    ErrorMessage = "..."
                  }
                         │
                         ↓
                 Check Batch Results
                         │
                    ┌────┴────┐
                    │          │
                Any Failed?    No
                    │          │
                   YES      Continue
                    │
                    ↓
         ┌──────────────────────┐
         │ Throw Exception:     │
         │ InvoiceCreation      │
         │ Exception with       │
         │ FailedOperations:    │
         │ ["ServiceId: 45"]    │
         └──────────┬───────────┘
                    │
                    ↓
         ┌──────────────────────┐
         │ ROLLBACK TRANSACTION │
         │ (Undo batches 1-2)   │
         └──────────┬───────────┘
                    │
                    ↓
         ┌──────────────────────┐
         │ Return Error to      │
         │ Controller           │
         │ + Error Details      │
         └──────────────────────┘
```

## Transaction Lifecycle

```
START
  │
  ├─ BEGIN TRANSACTION
  │  │
  │  ├─ Phase 1: Parallel Init
  │  │  │
  │  │  ├─ Save Point [SP1]
  │  │  ├─ Add Client
  │  │  ├─ Get Invoice #
  │  │  └─ Restore Point? [NO]
  │  │
  │  ├─ Phase 2: Sequential
  │  │  │
  │  │  ├─ Save Point [SP2]
  │  │  ├─ Add Invoice
  │  │  └─ Restore Point? [NO]
  │  │
  │  ├─ Phase 3: Parallel Batch
  │  │  │
  │  │  ├─ Batch 1 [SP3]
  │  │  │  ├─ Add P1, P2, P3, P4, P5, P6, P7, P8
  │  │  │  └─ Result? [SUCCESS → Continue]
  │  │  │
  │  │  ├─ Batch 2 [SP4]
  │  │  │  ├─ Add P9-P16
  │  │  │  └─ Result? [FAILURE → ROLLBACK TO SP1]
  │  │  │
  │  │  └─ Restore Point? [YES → SP1]
  │  │
  │  └─ ERROR: ROLLBACK ALL
  │
  ├─ COMMIT TRANSACTION (all changes)
  │
  └─ SUCCESS: Return Response
```

## Dependency Injection Flow

```
Program.cs - Startup Configuration
    │
    ├─ builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>()
    │
    ├─ builder.Services.AddScoped<IInvoiceService, InvoiceService>()
    │
    └─ builder.Services.AddDbContext<BillingPortalDbContext>()
           │
           ↓ (Registered in DI Container)
           
At Runtime - Request Processing
    │
    ├─ Incoming HTTP Request
    │  │
    │  ├─ Create InvoicesController Instance
    │  │  │
    │  │  ├─ Inject IInvoiceService
    │  │  │  │
    │  │  │  ├─ Create InvoiceService Instance
    │  │  │  │  │
    │  │  │  │  ├─ Inject IInvoiceRepository
    │  │  │  │  │  │
    │  │  │  │  │  ├─ Create InvoiceRepository Instance
    │  │  │  │  │  │  │
    │  │  │  │  │  │  ├─ Inject BillingPortalDbContext
    │  │  │  │  │  │  └─ Connected to SQL Server
    │  │  │  │  │  │
    │  │  │  │  │  └─ Ready for Data Operations
    │  │  │  │  │
    │  │  │  │  └─ Ready for Business Logic
    │  │  │  │
    │  │  │  └─ Ready to Handle Requests
    │  │  │
    │  │  └─ Ready to Serve HTTP
    │  │
    │  └─ Execute Action
    │
    └─ Return Response
```

## Exception Hierarchy

```
Exception (Base)
    │
    ├─ ApplicationException
    │   │
    │   ├─ ValidationException (Custom)
    │   │  └─ Used for FluentValidation failures
    │   │
    │   ├─ NotFoundException (Custom)
    │   │  └─ Used when entity not found
    │   │
    │   ├─ BusinessLogicException (Custom)
    │   │  └─ Used for business rule violations
    │   │
    │   └─ InvoiceCreationException ⭐ (New)
    │      ├─ Message: "Failed to create invoice"
    │      ├─ FailedOperations: List<string>
    │      │  └─ e.g., ["ServiceId: 45", "ServiceId: 67"]
    │      └─ InnerException: Original error details
    │
    └─ ... other Framework exceptions
```

## Data Flow Diagram

```
INPUT: CreateCompleteInvoiceDto
  │
  ├─ ClientDetailsDto
  ├─ CreateInvoiceDetailsDto
  └─ List<CreateInvoiceParticularDto> (1-∞)
       │
       ↓
   [PARALLEL PROCESSING]
       │
       ├─ PHASE 1: Transform to Domain Models
       │  │
       │  ├─ ClientDetailsDto → ClientDetails Model
       │  └─ CreateInvoiceDetailsDto → InvoiceDetails Model
       │  └─ CreateInvoiceParticularDto[] → InvoiceParticular[] Model
       │
       ├─ PHASE 2: Execute Parallel Operations
       │  │
       │  ├─ Repository.AddClientAsync(clientDetails)
       │  │  │
       │  │  └─ EF Core Insert → SQL Server
       │  │     └─ Returns: ClientDetails (with generated ID)
       │  │
       │  ├─ Repository.GetNextInvoiceNumberAsync()
       │  │  │
       │  │  └─ EF Core Query → SQL Server
       │  │     └─ Returns: string (next invoice #)
       │  │
       │  └─ Task.WhenAll() → Wait for both
       │
       ├─ PHASE 3: Create Invoice (Sequential)
       │  │
       │  └─ Repository.AddInvoiceAsync(invoice)
       │     │
       │     └─ EF Core Insert → SQL Server
       │        └─ Returns: InvoiceDetails (with generated ID)
       │
       ├─ PHASE 4: Add Particulars (Batch Parallel)
       │  │
       │  ├─ Batch Processing Loop
       │  │  │
       │  │  └─ Repository.AddParticularAsync(particular[])
       │  │     │
       │  │     └─ EF Core Insert → SQL Server
       │  │        └─ Returns: InvoiceParticular (with generated ID)
       │  │
       │  └─ Task.WhenAll(batch) → Wait for batch
       │
       ├─ PHASE 5: Commit or Rollback
       │  │
       │  ├─ All Success?
       │  │  ├─ YES → Commit Transaction
       │  │  └─ NO → Rollback Transaction
       │  │
       │  └─ Flush changes to SQL Server
       │
       ↓
OUTPUT: 
  ├─ SUCCESS:
  │  └─ InvoiceResponseDto
  │     ├─ InvoiceId
  │     ├─ ClientDetails
  │     ├─ InvoiceDetails
  │     └─ InvoiceParticulars[]
  │
  └─ FAILURE:
     └─ InvoiceCreationException
        ├─ Message
        └─ FailedOperations[]
```

---

These diagrams provide a complete visual representation of:
- ✅ System architecture and layering
- ✅ Parallel processing flow with phases
- ✅ Batch processing mechanics
- ✅ Error handling and rollback paths
- ✅ Transaction lifecycle management
- ✅ Dependency injection resolution
- ✅ Exception hierarchy
- ✅ Data transformation flow
