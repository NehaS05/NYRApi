using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;

namespace NYR.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.CompanyName : null))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location != null ? src.Location.LocationName : null))
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : null));

            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Role mappings
            CreateMap<Role, RoleDto>();
            CreateMap<CreateRoleDto, Role>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateRoleDto, Role>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Customer mappings
            CreateMap<Customer, CustomerDto>();
            CreateMap<CreateCustomerDto, Customer>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateCustomerDto, Customer>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Location mappings
            CreateMap<Location, LocationDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.CompanyName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : null));
            CreateMap<CreateLocationDto, Location>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateLocationDto, Location>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Category mappings
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateCategoryDto, Category>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Brand mappings
            CreateMap<Brand, BrandDto>();
            CreateMap<CreateBrandDto, Brand>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateBrandDto, Brand>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Supplier mappings
            CreateMap<Supplier, SupplierDto>();
            CreateMap<CreateSupplierDto, Supplier>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateSupplierDto, Supplier>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.Variants));
            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Variants, opt => opt.Ignore());
            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Variants, opt => opt.Ignore());

            // ProductVariant mappings
            CreateMap<ProductVariant, ProductVariantDto>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes));
            CreateMap<ProductVariantAttribute, ProductVariantAttributeDto>()
                .ForMember(dest => dest.VariationName, opt => opt.MapFrom(src => src.Variation.Name))
                .ForMember(dest => dest.VariationOptionName, opt => opt.MapFrom(src => src.VariationOption.Name))
                .ForMember(dest => dest.VariationOptionValue, opt => opt.MapFrom(src => src.VariationOption.Value));

			// Van mappings
			CreateMap<Van, VanDto>()
				.ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver != null ? src.Driver.Name : null));
			CreateMap<CreateVanDto, Van>()
				.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
			CreateMap<UpdateVanDto, Van>()
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

			// Warehouse mappings
			CreateMap<Warehouse, WarehouseDto>();
			CreateMap<CreateWarehouseDto, Warehouse>()
				.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
				.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
			CreateMap<UpdateWarehouseDto, Warehouse>()
				.ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // DriverAvailability mappings
            CreateMap<DriverAvailability, DriverAvailabilityDto>();
            CreateMap<CreateDriverAvailabilityDto, DriverAvailability>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateDriverAvailabilityDto, DriverAvailability>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Scanner mappings
            CreateMap<Scanner, ScannerDto>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.LocationName));
            CreateMap<CreateScannerDto, Scanner>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateScannerDto, Scanner>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Variation mappings
            CreateMap<Variation, VariationDto>();
            CreateMap<VariationOption, VariationOptionDto>();
            CreateMap<CreateVariationDto, Variation>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Options, opt => opt.Ignore());
            CreateMap<UpdateVariationDto, Variation>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Options, opt => opt.Ignore());

            // TransferInventory mappings
            CreateMap<TransferInventory, TransferInventoryDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.CompanyName))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.LocationName))
                .ForMember(dest => dest.ContactPerson, opt => opt.MapFrom(src => src.Location.ContactPerson))
                .ForMember(dest => dest.LocationNumber, opt => opt.MapFrom(src => src.Location.LocationPhone))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            CreateMap<CreateTransferInventoryDto, TransferInventory>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.TransferDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            // TransferInventoryItem mappings
            CreateMap<TransferInventoryItem, TransferInventoryItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.SkuCode, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.BarcodeSKU : null))
                .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.VariantName : null))
                .ForMember(dest => dest.VariationType, opt => opt.MapFrom(src => 
                    src.ProductVariant != null && src.ProductVariant.Attributes.Any() 
                    ? src.ProductVariant.Attributes.First().Variation.Name 
                    : null))
                .ForMember(dest => dest.VariationValue, opt => opt.MapFrom(src => 
                    src.ProductVariant != null && src.ProductVariant.Attributes.Any() 
                    ? src.ProductVariant.Attributes.First().VariationOption.Name 
                    : null));
            CreateMap<CreateTransferInventoryItemDto, TransferInventoryItem>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // RequestSupply mappings
            CreateMap<RequestSupply, RequestSupplyDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            CreateMap<CreateRequestSupplyDto, RequestSupply>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Items, opt => opt.Ignore());
            CreateMap<UpdateRequestSupplyDto, RequestSupply>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            // RequestSupplyItem mappings (added)
            CreateMap<RequestSupplyItem, RequestSupplyItemDto>()
                .ForMember(dest => dest.VariationName, opt => opt.MapFrom(src => src.Variation != null ? src.Variation.Name : string.Empty));

            CreateMap<CreateRequestSupplyItemDto, RequestSupplyItem>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateRequestSupplyItemDto, RequestSupplyItem>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // Routes mappings
            CreateMap<Routes, RouteDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
                .ForMember(dest => dest.RouteStops, opt => opt.MapFrom(src => src.RouteStops))
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.User.Warehouse != null ? src.User.Warehouse.Name : ""))
                .ForMember(dest => dest.WarehouseId, opt => opt.MapFrom(src => src.User.WarehouseId != null ? src.User.WarehouseId : 0));
            CreateMap<CreateRouteDto, Routes>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.RouteStops, opt => opt.Ignore());
            CreateMap<UpdateRouteDto, Routes>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.RouteStops, opt => opt.Ignore());

            // RouteStop mappings
            CreateMap<RouteStop, RouteStopDto>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.LocationName))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.CompanyName : null))
                .ForMember(dest => dest.ShippingInventory, opt => opt.MapFrom(src => 
                    src.RestockRequest != null && src.RestockRequest.Items != null 
                        ? src.RestockRequest.Items.Select(item => new TransferInventoryItemDto
                        {
                            Id = item.Id,
                            ProductId = item.ProductId,
                            ProductName = item.Product != null ? item.Product.Name : "Unknown",
                            SkuCode = item.ProductVariant != null ? item.ProductVariant.BarcodeSKU : null,
                            ProductVariantId = item.ProductVariantId,
                            VariantName = item.ProductVariant != null ? item.ProductVariant.VariantName : null,
                            VariationType = item.ProductVariant != null && item.ProductVariant.Attributes.Any() 
                                ? item.ProductVariant.Attributes.First().Variation.Name : null,
                            VariationValue = item.ProductVariant != null && item.ProductVariant.Attributes.Any() 
                                ? item.ProductVariant.Attributes.First().VariationOption.Name : null,
                            Quantity = item.Quantity
                        }).ToList()
                        : new List<TransferInventoryItemDto>()
                ));
            CreateMap<CreateRouteStopDto, RouteStop>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateRouteStopDto, RouteStop>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // LocationInventoryData mappings
            CreateMap<LocationInventoryData, LocationInventoryDataDto>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.LocationName))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductSKU, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.BarcodeSKU : null))
                .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.VariationName))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Name : null))
                .ForMember(dest => dest.UpdatedByName, opt => opt.MapFrom(src => src.UpdatedByUser != null ? src.UpdatedByUser.Name : null))
                .ForMember(dest => dest.customerId, opt => opt.MapFrom(src => src.Location.CustomerId))
                .ForMember(dest => dest.customerName, opt => opt.MapFrom(src => src.Location.Customer.CompanyName))
                .ForMember(dest => dest.ContactPerson, opt => opt.MapFrom(src => src.Location.ContactPerson))
                .ForMember(dest => dest.LocationNumber, opt => opt.MapFrom(src => src.Location.LocationPhone));
            CreateMap<CreateLocationInventoryDataDto, LocationInventoryData>()
                .ForMember(dest => dest.VariationName, opt => opt.MapFrom(src => src.VariantName))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateLocationInventoryDataDto, LocationInventoryData>()
                .ForMember(dest => dest.VariationName, opt => opt.MapFrom(src => src.VariantName))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            // VanInventory mappings
            CreateMap<VanInventory, VanInventoryDto>()
                .ForMember(dest => dest.VanName, opt => opt.MapFrom(src => src.Van.VanName))
                .ForMember(dest => dest.VanNumber, opt => opt.MapFrom(src => src.Van.VanNumber))
                .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.DriverName ?? src.Van.DefaultDriverName))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.LocationName))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Location.Customer.CompanyName))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            CreateMap<CreateVanInventoryDto, VanInventory>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.TransferDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            // VanInventoryItem mappings
            CreateMap<VanInventoryItem, VanInventoryItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.SkuCode, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.BarcodeSKU : null))
                .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.VariantName : null))
                .ForMember(dest => dest.VariationType, opt => opt.MapFrom(src => 
                    src.ProductVariant != null && src.ProductVariant.Attributes.Any() 
                    ? src.ProductVariant.Attributes.First().Variation.Name 
                    : null))
                .ForMember(dest => dest.VariationValue, opt => opt.MapFrom(src => 
                    src.ProductVariant != null && src.ProductVariant.Attributes.Any() 
                    ? src.ProductVariant.Attributes.First().VariationOption.Name 
                    : null));
            CreateMap<CreateVanInventoryItemDto, VanInventoryItem>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // RestockRequest mappings
            CreateMap<RestockRequest, RestockRequestDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.CompanyName))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.LocationName))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<CreateRestockRequestDto, RestockRequest>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.RequestDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Restock Request"))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            // RestockRequestItem mappings
            CreateMap<RestockRequestItem, RestockRequestItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.SkuCode, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.BarcodeSKU : null))
                .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.VariantName : null))
                .ForMember(dest => dest.VariationType, opt => opt.MapFrom(src => 
                    src.ProductVariant != null && src.ProductVariant.Attributes.Any() 
                    ? src.ProductVariant.Attributes.First().Variation.Name 
                    : null))
                .ForMember(dest => dest.VariationValue, opt => opt.MapFrom(src => 
                    src.ProductVariant != null && src.ProductVariant.Attributes.Any() 
                    ? src.ProductVariant.Attributes.First().VariationOption.Name 
                    : null))
                .ForMember(dest => dest.DeliveredQuantity, opt => opt.MapFrom(src => src.DeliveredQuantity));

            CreateMap<CreateRestockRequestItemDto, RestockRequestItem>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // LocationOutwardInventory mappings
            CreateMap<LocationOutwardInventory, LocationOutwardInventoryDto>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.LocationName))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByUser.Name))
                .ForMember(dest => dest.UpdatedByName, opt => opt.MapFrom(src => src.UpdatedByUser != null ? src.UpdatedByUser.Name : null));
            CreateMap<CreateLocationOutwardInventoryDto, LocationOutwardInventory>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateLocationOutwardInventoryDto, LocationOutwardInventory>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            // LocationUnlistedInventory mappings
            CreateMap<LocationUnlistedInventory, LocationUnlistedInventoryDto>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location.LocationName))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByUser.Name))
                .ForMember(dest => dest.UpdatedByName, opt => opt.MapFrom(src => src.UpdatedByUser != null ? src.UpdatedByUser.Name : null));
            CreateMap<CreateLocationUnlistedInventoryDto, LocationUnlistedInventory>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateLocationUnlistedInventoryDto, LocationUnlistedInventory>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            // InventoryCount mappings
            CreateMap<WarehouseInventory, WarehouseInventoryCountDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.SkuCode, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.BarcodeSKU : null))
                .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.VariantName : null));

            CreateMap<VanInventoryItem, VanInventoryCountDto>()
                .ForMember(dest => dest.VanId, opt => opt.MapFrom(src => src.VanInventory.VanId))
                .ForMember(dest => dest.VanName, opt => opt.MapFrom(src => src.VanInventory.Van.VanName))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.SkuCode, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.BarcodeSKU : null))
                .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.VariantName : null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.VanInventory.Status));
        }
    }
}
