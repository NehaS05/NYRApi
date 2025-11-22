# Routes Schema Changes Summary

## Overview
The LocationId has been moved from the Routes table to the RouteStops table to allow each stop in a route to have its own location.

## Database Schema Changes

### Routes Table
**Removed:**
- `LocationId` (int, FK to Locations)

**Current Structure:**
- `Id` (int, PK)
- `UserId` (int, FK to Users)
- `DeliveryDate` (datetime2)
- `CreatedAt` (datetime2)
- `UpdatedAt` (datetime2, nullable)
- `IsActive` (bit)

### RouteStops Table
**Added:**
- `LocationId` (int, FK to Locations)

**Current Structure:**
- `Id` (int, PK)
- `RouteId` (int, FK to Routes)
- `LocationId` (int, FK to Locations) ← **NEW**
- `StopOrder` (int)
- `Address` (nvarchar(500))
- `CustomerName` (nvarchar(100), nullable)
- `ContactPhone` (nvarchar(20), nullable)
- `Notes` (nvarchar(1000), nullable)
- `Status` (nvarchar(50))
- `CompletedAt` (datetime2, nullable)
- `CreatedAt` (datetime2)
- `IsActive` (bit)

## Relationships

### Before:
- Routes → Location (Many-to-One)
- Routes → RouteStops (One-to-Many)

### After:
- Routes → User (Many-to-One)
- Routes → RouteStops (One-to-Many)
- RouteStops → Location (Many-to-One) ← **NEW**
- RouteStops → Routes (Many-to-One)

## Migration File
**File:** `Migrations/20251121230912_AddRoutesAndRouteStops.cs`

This migration creates both tables with the correct schema from the start.

## Code Changes

### Entities
- `Routes.cs` - Removed LocationId and Location navigation property
- `RouteStop.cs` - Added LocationId and Location navigation property

### DTOs
- `RouteDto` - Removed LocationId and LocationName
- `RouteStopDto` - Added LocationId and LocationName
- All Create/Update DTOs updated accordingly

### Repository
- `RouteRepository` - Updated queries to use `.ThenInclude(rs => rs.Location)`
- `GetByLocationIdAsync` now filters by `RouteStops.Any(rs => rs.LocationId == locationId)`

### Service
- `RouteService` - Updated validation to check LocationId for each stop

## Benefits of This Change

1. **Flexibility**: Each stop can be at a different location
2. **Better Data Model**: More accurately represents real-world delivery routes
3. **Scalability**: Easier to add location-specific features per stop
4. **Reporting**: Can analyze routes by multiple locations

## API Impact

### Request Body Changes

**Before (CreateRouteDto):**
```json
{
  "locationId": 1,
  "userId": 1,
  "deliveryDate": "2024-01-20",
  "routeStops": [...]
}
```

**After (CreateRouteDto):**
```json
{
  "userId": 1,
  "deliveryDate": "2024-01-20",
  "routeStops": [
    {
      "locationId": 1,
      "stopOrder": 1,
      "address": "123 Main St"
    }
  ]
}
```

### Response Changes

**Before (RouteDto):**
```json
{
  "id": 1,
  "locationId": 1,
  "locationName": "Warehouse A",
  "routeStops": [...]
}
```

**After (RouteDto):**
```json
{
  "id": 1,
  "userId": 1,
  "userName": "John Doe",
  "routeStops": [
    {
      "id": 1,
      "locationId": 1,
      "locationName": "Warehouse A",
      "stopOrder": 1
    }
  ]
}
```

## Migration Instructions

1. Ensure all code changes are in place
2. Run: `dotnet ef database update`
3. The migration will create the tables with the correct schema

## Rollback

If needed, rollback using:
```bash
dotnet ef database update <PreviousMigrationName>
```

Then remove the migration:
```bash
dotnet ef migrations remove
```
