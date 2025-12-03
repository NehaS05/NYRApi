-- Migration Script: Convert ProductVariations to ProductVariants
-- This script migrates data from the old ProductVariation system to the new ProductVariant system

-- Step 1: Create ProductVariants from existing ProductVariations
-- Group variations by product and create unique combinations

BEGIN TRANSACTION;

-- Insert ProductVariants based on unique combinations of variations per product
INSERT INTO ProductVariants (ProductId, VariantName, SKU, Price, IsEnabled, CreatedAt, IsActive)
SELECT 
    p.Id AS ProductId,
    STRING_AGG(pv.VariationValue, ' / ') WITHIN GROUP (ORDER BY pv.VariationType) AS VariantName,
    NULL AS SKU,
    NULL AS Price,
    1 AS IsEnabled,
    GETUTCDATE() AS CreatedAt,
    1 AS IsActive
FROM Products p
INNER JOIN ProductVariations pv ON p.Id = pv.ProductId
WHERE pv.IsActive = 1
GROUP BY p.Id;

-- Step 2: Create ProductVariantAttributes
-- Link each variant to its variation options
-- This requires matching variation types and values to the Variations and VariationOptions tables

-- First, we need to ensure Variations and VariationOptions exist for all ProductVariation types/values
-- Insert missing Variations
INSERT INTO Variations (Name, ValueType, CreatedAt, IsActive)
SELECT DISTINCT 
    pv.VariationType,
    'Dropdown' AS ValueType,
    GETUTCDATE() AS CreatedAt,
    1 AS IsActive
FROM ProductVariations pv
WHERE pv.VariationType NOT IN (SELECT Name FROM Variations)
AND pv.IsActive = 1;

-- Insert missing VariationOptions
INSERT INTO VariationOptions (VariationId, Name, Value, CreatedAt, IsActive)
SELECT DISTINCT
    v.Id AS VariationId,
    pv.VariationValue AS Name,
    pv.VariationValue AS Value,
    GETUTCDATE() AS CreatedAt,
    1 AS IsActive
FROM ProductVariations pv
INNER JOIN Variations v ON v.Name = pv.VariationType
WHERE NOT EXISTS (
    SELECT 1 FROM VariationOptions vo 
    WHERE vo.VariationId = v.Id 
    AND vo.Name = pv.VariationValue
)
AND pv.IsActive = 1;

-- Step 3: Create ProductVariantAttributes
-- For each ProductVariant, create attributes linking to VariationOptions
WITH VariantCombinations AS (
    SELECT 
        pv.ProductId,
        STRING_AGG(pv.VariationValue, ' / ') WITHIN GROUP (ORDER BY pv.VariationType) AS VariantName,
        pv.VariationType,
        pv.VariationValue
    FROM ProductVariations pv
    WHERE pv.IsActive = 1
    GROUP BY pv.ProductId, pv.VariationType, pv.VariationValue
)
INSERT INTO ProductVariantAttributes (ProductVariantId, VariationId, VariationOptionId, CreatedAt)
SELECT DISTINCT
    pvt.Id AS ProductVariantId,
    v.Id AS VariationId,
    vo.Id AS VariationOptionId,
    GETUTCDATE() AS CreatedAt
FROM VariantCombinations vc
INNER JOIN ProductVariants pvt ON pvt.ProductId = vc.ProductId AND pvt.VariantName = vc.VariantName
INNER JOIN Variations v ON v.Name = vc.VariationType
INNER JOIN VariationOptions vo ON vo.VariationId = v.Id AND vo.Name = vc.VariationValue;

-- Step 4: Update inventory tables to use ProductVariantId
-- Note: This assumes a simple 1:1 mapping. For complex scenarios, manual review may be needed.

-- Update WarehouseInventories
-- For now, we'll set ProductVariantId to NULL for items that had ProductVariations
-- Manual mapping will be needed based on business logic

-- Update VanInventoryItems
-- Similar approach - set to NULL initially

-- Update TransferInventoryItems
-- Similar approach - set to NULL initially

-- Update RestockRequestItems
-- Similar approach - set to NULL initially

-- Update LocationInventoryData
-- Similar approach - set to NULL initially

COMMIT TRANSACTION;

-- Note: After running this migration, you should:
-- 1. Review the created ProductVariants to ensure they match your business requirements
-- 2. Manually map inventory items to the correct ProductVariants
-- 3. Consider deactivating old ProductVariations (IsActive = 0) once migration is verified
-- 4. Update your application code to use ProductVariants instead of ProductVariations
