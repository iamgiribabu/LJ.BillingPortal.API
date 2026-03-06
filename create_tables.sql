USE LJLifterBillingDB;
GO

-- Create ClientDetails table
IF OBJECT_ID(N'dbo.ClientDetails', N'U') IS NULL
CREATE TABLE dbo.ClientDetails (
    ClientID INT PRIMARY KEY IDENTITY(1,1),
    CompanyName NVARCHAR(255) NOT NULL,
    AddressLine1 NVARCHAR(500) NOT NULL,
    AddressLine2 NVARCHAR(500),
    AddressLine3 NVARCHAR(500),
    Gstin NVARCHAR(50),
    State NVARCHAR(100),
    StateCode NVARCHAR(10),
    CreatedDate DATETIME DEFAULT GETUTCDATE()
);
GO

-- Create InvoiceDetails table
IF OBJECT_ID(N'dbo.InvoiceDetails', N'U') IS NULL
CREATE TABLE dbo.InvoiceDetails (
    InvoiceID INT PRIMARY KEY IDENTITY(1,1),
    ClientID INT NOT NULL,
    InvoiceNumber NVARCHAR(50) NOT NULL UNIQUE,
    InvoiceDate DATETIME NOT NULL,
    PlaceOfSupply NVARCHAR(100) NOT NULL,
    PoNumber NVARCHAR(50),
    CraneReg NVARCHAR(50),
    TotalAmountBeforeTax DECIMAL(18,2) NOT NULL,
    CGST DECIMAL(18,2) NOT NULL,
    SGST DECIMAL(18,2) NOT NULL,
    IGST DECIMAL(18,2) NOT NULL,
    NetAmountAfterTax DECIMAL(18,2) NOT NULL,
    CreatedDate DATETIME DEFAULT GETUTCDATE(),
    FOREIGN KEY (ClientID) REFERENCES ClientDetails(ClientID) ON DELETE CASCADE
);
GO

-- Create InvoiceParticulars table
IF OBJECT_ID(N'dbo.InvoiceParticulars', N'U') IS NULL
CREATE TABLE dbo.InvoiceParticulars (
    ServiceID INT PRIMARY KEY IDENTITY(1,1),
    InvoiceID INT NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    HsnSac NVARCHAR(20),
    Quantity INT NOT NULL,
    Rate DECIMAL(18,2) NOT NULL,
    TaxableValue DECIMAL(18,2) NOT NULL,
    CreatedDate DATETIME DEFAULT GETUTCDATE(),
    FOREIGN KEY (InvoiceID) REFERENCES InvoiceDetails(InvoiceID) ON DELETE CASCADE
);
GO

-- Create __EFMigrationsHistory table for EF Core tracking
IF OBJECT_ID(N'dbo.__EFMigrationsHistory', N'U') IS NULL
CREATE TABLE dbo.__EFMigrationsHistory (
    MigrationId NVARCHAR(150) NOT NULL PRIMARY KEY,
    ProductVersion NVARCHAR(32) NOT NULL
);
GO

-- Insert the initial migration record
INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion)
VALUES ('20260306165650_InitialSetup', '9.0.0');
GO
