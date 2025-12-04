-- Migration: Add Product Variants System
-- This migration adds support for product variants (combinations of variations)
-- Date: 2025-12-03

-- Step 1: Create ProductVariants table
CREATE TABLE ProductVariants (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    VariantName NVARCHAR(500) NOT NULL,
    SKU NVARCHAR(100) NULL,
    Price DECIMAL(18,2) NULL,
    IsEnabled BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_ProductVariants_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);

CREATE INDEX IX_ProductVariants_ProductId ON ProductVariants(ProductId);
CREATE INDEX IX_ProductVariants_IsActive ON ProductVariants(IsActive);

-- Step 2: Create ProductVariantAttributes table
CREATE TABLE ProductVariantAttributes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductVariantId INT NOT NULL,
    VariationId INT NOT NULL,
    VariationOptionId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_ProductVariantAttributes_ProductVariants FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ProductVariantAttributes_Variations FOREIGN KEY (VariationId) REFERENCES Variations(Id),
    CONSTRAINT FK_ProductVariantAttributes_VariationOptions FOREIGN KEY (VariationOptionId) REFERENCES VariationOptions(Id)
);

CREATE INDEX IX_ProductVariantAttributes_ProductVariantId ON ProductVariantAttributes(ProductVariantId);
CREATE INDEX IX_ProductVariantAttributes_VariationId ON ProductVariantAttributes(VariationId);
CREATE INDEX IX_ProductVariantAttributes_VariationOptionId ON ProductVariantAttributes(VariationOptionId);

-- Step 3: Add ProductVariantId to WarehouseInventories
ALTER TABLE WarehouseInventories ADD ProductVariantId INT NULL;
ALTER TABLE WarehouseInventories ADD CONSTRAINT FK_WarehouseInventories_ProductVariants 
    FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id);
CREATE INDEX IX_WarehouseInventories_ProductVariantId ON WarehouseInventories(ProductVariantId);

-- Step 4: Add ProductVariantId to RestockRequestItems
ALTER TABLE RestockRequestItems ADD ProductVariantId INT NULL;
ALTER TABLE RestockRequestItems ADD CONSTRAINT FK_RestockRequestItems_ProductVariants 
    FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id);
CREATE INDEX IX_RestockRequestItems_ProductVariantId ON RestockRequestItems(ProductVariantId);

-- Step 5: Add ProductVariantId to TransferInventoryItems
ALTER TABLE TransferInventoryItems ADD ProductVariantId INT NULL;
ALTER TABLE TransferInventoryItems ADD CONSTRAINT FK_TransferInventoryItems_ProductVariants 
    FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id);
CREATE INDEX IX_TransferInventoryItems_ProductVariantId ON TransferInventoryItems(ProductVariantId);

-- Step 6: Add ProductVariantId to VanInventoryItems
ALTER TABLE VanInventoryItems ADD ProductVariantId INT NULL;
ALTER TABLE VanInventoryItems ADD CONSTRAINT FK_VanInventoryItems_ProductVariants 
    FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id);
CREATE INDEX IX_VanInventoryItems_ProductVariantId ON VanInventoryItems(ProductVariantId);

-- Step 7: Add ProductVariantId to LocationInventoryData
ALTER TABLE LocationInventoryData ADD ProductVariantId INT NULL;
ALTER TABLE LocationInventoryData ADD CONSTRAINT FK_LocationInventoryData_ProductVariants 
    FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id);
CREATE INDEX IX_LocationInventoryData_ProductVariantId ON LocationInventoryData(ProductVariantId);

-- Step 8: Drop old unique constraint on WarehouseInventories and create new one
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_WarehouseInventories_WarehouseId_ProductVariationId' AND object_id = OBJECT_ID('WarehouseInventories'))
BEGIN
    DROP INDEX IX_WarehouseInventories_WarehouseId_ProductVariationId ON WarehouseInventories;
END;

CREATE UNIQUE INDEX IX_WarehouseInventories_WarehouseId_ProductId_ProductVariantId 
    ON WarehouseInventories(WarehouseId, ProductId, ProductVariantId) 
    WHERE ProductVariantId IS NOT NULL;

-- Step 9: Make ProductVariationId nullable in WarehouseInventories (if not already)
-- This allows for backward compatibility during migration
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('WarehouseInventories') AND name = 'ProductVariationId' AND is_nullable = 0)
BEGIN
    ALTER TABLE WarehouseInventories ALTER COLUMN ProductVariationId INT NULL;
END;

-- Step 10: Make ProductVariationId nullable in VanInventoryItems (if not already)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VanInventoryItems') AND name = 'ProductVariationId' AND is_nullable = 0)
BEGIN
    ALTER TABLE VanInventoryItems ALTER COLUMN ProductVariationId INT NULL;
END;

PRINT 'Product Variants System migration completed successfully';
