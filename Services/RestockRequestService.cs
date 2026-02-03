using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;
using System.Security.Cryptography;

namespace NYR.API.Services
{
    public class DeliveryStatus
    {
        public bool IsFullyDelivered { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class VanInventoryItemResult
    {
        public int VanId { get; set; }
        public int Id { get; set; }
        public int VanInventoryId { get; set; }
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RestockRequestService : IRestockRequestService
    {
        private readonly IRestockRequestRepository _restockRequestRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGenericRepository<ProductVariant> _productVariantRepository;
        private readonly ILocationInventoryDataService _locationInventoryDataService;
        private readonly IVanRepository _vanRepository;
        private readonly IVanInventoryRepository _vanInventoryRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RestockRequestService(
            IRestockRequestRepository restockRequestRepository,
            ICustomerRepository customerRepository,
            ILocationRepository locationRepository,
            IProductRepository productRepository,
            IGenericRepository<ProductVariant> productVariantRepository,
            ILocationInventoryDataService locationInventoryDataService,
            IVanRepository vanRepository,
            IVanInventoryRepository vanInventoryRepository,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _restockRequestRepository = restockRequestRepository;
            _customerRepository = customerRepository;
            _locationRepository = locationRepository;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _locationInventoryDataService = locationInventoryDataService;
            _vanRepository = vanRepository;
            _vanInventoryRepository = vanInventoryRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RestockRequestDto>> GetAllRequestsAsync()
        {
            var requests = await _restockRequestRepository.GetAllWithDetailsAsync();
            var requestDtos = _mapper.Map<IEnumerable<RestockRequestDto>>(requests).ToList();
            
            // Get DeliveryDate and DeliveryOTP from Route information for each RestockRequest
            foreach (var dto in requestDtos)
            {
                var routeStopInfo = await _context.RouteStops
                    .Where(rs => rs.RestockRequestId == dto.Id && rs.IsActive)
                    .Join(_context.Routes, rs => rs.RouteId, r => r.Id, (rs, r) => new { r.DeliveryDate, rs.DeliveryOTP })
                    .FirstOrDefaultAsync();
                
                if (routeStopInfo != null)
                {
                    dto.DeliveryDate = routeStopInfo.DeliveryDate == default(DateTime) ? null : routeStopInfo.DeliveryDate;
                    dto.DeliveryOTP = routeStopInfo.DeliveryOTP;
                }
            }
            
            return requestDtos;
        }

        public async Task<RestockRequestDto?> GetRequestByIdAsync(int id)
        {
            var request = await _restockRequestRepository.GetByIdWithDetailsAsync(id);
            if (request == null) return null;
            
            var dto = _mapper.Map<RestockRequestDto>(request);
            
            // Get DeliveryDate and DeliveryOTP from Route information
            var routeStopInfo = await _context.RouteStops
                .Where(rs => rs.RestockRequestId == dto.Id && rs.IsActive)
                .Join(_context.Routes, rs => rs.RouteId, r => r.Id, (rs, r) => new { r.DeliveryDate, rs.DeliveryOTP })
                .FirstOrDefaultAsync();
            
            if (routeStopInfo != null)
            {
                dto.DeliveryDate = routeStopInfo.DeliveryDate == default(DateTime) ? null : routeStopInfo.DeliveryDate;
                dto.DeliveryOTP = routeStopInfo.DeliveryOTP;
            }
            
            return dto;
        }

        public async Task<IEnumerable<RestockRequestDto>> GetRequestsByLocationIdAsync(int locationId)
        {
            var requests = await _restockRequestRepository.GetByLocationIdAsync(locationId);
            var requestDtos = _mapper.Map<IEnumerable<RestockRequestDto>>(requests).ToList();
            
            // Get DeliveryDate and DeliveryOTP from Route information for each RestockRequest
            foreach (var dto in requestDtos)
            {
                var routeStopInfo = await _context.RouteStops
                    .Where(rs => rs.RestockRequestId == dto.Id && rs.IsActive)
                    .Join(_context.Routes, rs => rs.RouteId, r => r.Id, (rs, r) => new { r.DeliveryDate, rs.DeliveryOTP })
                    .FirstOrDefaultAsync();
                
                if (routeStopInfo != null)
                {
                    dto.DeliveryDate = routeStopInfo.DeliveryDate == default(DateTime) ? null : routeStopInfo.DeliveryDate;
                    dto.DeliveryOTP = routeStopInfo.DeliveryOTP;
                }
            }
            
            return requestDtos;
        }

        public async Task<IEnumerable<RestockRequestDto>> GetRequestsByCustomerIdAsync(int customerId)
        {
            var requests = await _restockRequestRepository.GetByCustomerIdAsync(customerId);
            var requestDtos = _mapper.Map<IEnumerable<RestockRequestDto>>(requests).ToList();
            
            // Get DeliveryDate and DeliveryOTP from Route information for each RestockRequest
            foreach (var dto in requestDtos)
            {
                var routeStopInfo = await _context.RouteStops
                    .Where(rs => rs.RestockRequestId == dto.Id && rs.IsActive)
                    .Join(_context.Routes, rs => rs.RouteId, r => r.Id, (rs, r) => new { r.DeliveryDate, rs.DeliveryOTP })
                    .FirstOrDefaultAsync();
                
                if (routeStopInfo != null)
                {
                    dto.DeliveryDate = routeStopInfo.DeliveryDate == default(DateTime) ? null : routeStopInfo.DeliveryDate;
                    dto.DeliveryOTP = routeStopInfo.DeliveryOTP;
                }
            }
            
            return requestDtos;
        }

        public async Task<IEnumerable<RestockRequestSummaryDto>> GetRequestsSummaryAsync()
        {
            var requests = await _restockRequestRepository.GetAllWithDetailsAsync();
            
            var summary = requests
                .GroupBy(r => new { r.LocationId, r.CustomerId })
                .Select(g => new RestockRequestSummaryDto
                {
                    LocationId = g.Key.LocationId,
                    LocationName = g.First().Location.LocationName,
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.First().Customer.CompanyName,
                    ContactPerson = g.First().Location.ContactPerson,
                    LocationNumber = g.First().Location.LocationPhone,
                    TotalRequests = g.Count(),
                    TotalItems = g.Sum(r => r.Items.Sum(i => i.Quantity))
                })
                .ToList();

            return summary;
        }

        public async Task<RestockRequestDto> CreateRequestAsync(CreateRestockRequestDto createDto)
        {
            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(createDto.CustomerId);
            if (customer == null)
                throw new ArgumentException("Customer not found");

            // Validate location exists
            var location = await _locationRepository.GetByIdAsync(createDto.LocationId);
            if (location == null)
                throw new ArgumentException("Location not found");

            // Validate all products and variations exist
            foreach (var item in createDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new ArgumentException($"Product with ID {item.ProductId} not found");

                if (item.ProductVariantId.HasValue)
                {
                    var variant = await _productVariantRepository.GetByIdAsync(item.ProductVariantId.Value);
                    if (variant == null)
                        throw new ArgumentException($"Product variant with ID {item.ProductVariantId} not found");

                    if (variant.ProductId != item.ProductId)
                        throw new ArgumentException($"Product variant {item.ProductVariantId} does not belong to product {item.ProductId}");
                }
            }

            // Check if there's an existing RestockRequest for the same customer, location, and date (today)
            var existingRequests = await _restockRequestRepository.GetByLocationIdAsync(createDto.LocationId);
            var today = DateTime.UtcNow.Date;

            //If existingRequests status is delivered then only create new Restock Request
            var existingRequest = existingRequests.FirstOrDefault(r => 
                r.CustomerId == createDto.CustomerId && 
                r.LocationId == createDto.LocationId && 
                // r.RequestDate.Date == today &&
                r.Status.ToLower() != "delivered" &&
                r.IsActive);

            RestockRequest restockRequest;
            
            if (existingRequest != null)
            {
                // Use existing request and add new items to it
                restockRequest = existingRequest;
                
                // Add new items to the existing request
                foreach (var itemDto in createDto.Items)
                {
                    // Check if the same product/variant already exists in the request
                    var existingItem = restockRequest.Items.FirstOrDefault(i => 
                        i.ProductId == itemDto.ProductId && 
                        i.ProductVariantId == itemDto.ProductVariantId);
                    
                    if (existingItem != null)
                    {
                        // Update quantity if item already exists
                        existingItem.Quantity += itemDto.Quantity;
                    }
                    else
                    {
                        // Add new item
                        restockRequest.Items.Add(new RestockRequestItem
                        {
                            RestockRequestId = restockRequest.Id,
                            ProductId = itemDto.ProductId,
                            ProductVariantId = itemDto.ProductVariantId,
                            Quantity = itemDto.Quantity,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
                
                restockRequest.UpdatedAt = DateTime.UtcNow;
                await _restockRequestRepository.UpdateAsync(restockRequest);
            }
            else
            {
                // Create new RestockRequest entity
                restockRequest = new RestockRequest
                {
                    CustomerId = createDto.CustomerId,
                    LocationId = createDto.LocationId,
                    Status = "Restock Request",
                    RequestDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // Add items
                restockRequest.Items = createDto.Items.Select(itemDto => new RestockRequestItem
                {
                    ProductId = itemDto.ProductId,
                    ProductVariantId = itemDto.ProductVariantId,
                    Quantity = itemDto.Quantity,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                restockRequest = await _restockRequestRepository.AddAsync(restockRequest);
            }
            
            // Fetch with details for proper mapping
            var result = await _restockRequestRepository.GetByIdWithDetailsAsync(restockRequest.Id);
            return _mapper.Map<RestockRequestDto>(result);
        }

        public async Task<bool> DeleteRequestAsync(int id)
        {
            var request = await _restockRequestRepository.GetByIdAsync(id);
            if (request == null) return false;
            
            request.IsActive = false;
            request.UpdatedAt = DateTime.UtcNow;
            await _restockRequestRepository.UpdateAsync(request);
            return true;
        }

        public async Task<IEnumerable<ProductVariantInfoDto>> GetProductVariantNameBySkuAsync(string skuCode, int locationId, int? userId = null, int? productVariantId = null)
        {
            if (string.IsNullOrWhiteSpace(skuCode))
                return Enumerable.Empty<ProductVariantInfoDto>();

            var query = _context.RestockRequestItems
                .Include(pc => pc.ProductVariant)
                .Include(pv => pv.Product)
                .Include(pv => pv.RestockRequest)
                .Where(pv => (pv.ProductVariant.BarcodeSKU == skuCode || pv.ProductVariant.BarcodeSKU2 == skuCode || pv.ProductVariant.BarcodeSKU3 == skuCode || pv.ProductVariant.BarcodeSKU4 == skuCode)
                    && pv.RestockRequest.LocationId == locationId && pv.RestockRequest.IsActive && pv.ProductVariant.IsActive);

            if (productVariantId.HasValue && productVariantId.Value > 0)
            {
                query = query.Where(pv => pv.ProductVariantId == productVariantId.Value);
            }

            var productVariants = await query
                .Select(pv => new ProductVariantInfoDto
                {
                    ProductVariantId = pv.ProductVariant.Id,
                    VariantName = pv.ProductVariant.VariantName,
                    ProdcutId = pv.ProductId != null ? pv.ProductId : 0
                })
                .Distinct()
                .ToListAsync();

            //If its only one data then do Scan complete 
            if (productVariants.Count() == 1)
            {
                // Execute CreateOutwardInventoryAsync service for each matching item
                var restockItems = await query.ToListAsync();

                foreach (var item in restockItems)
                {
                    try
                    {
                        var createOutwardDto = new CreateLocationInventoryDataDto
                        {
                            LocationId = locationId,
                            ProductId = item.ProductId,
                            ProductVariantId = item.ProductVariantId,
                            Quantity = 1,  // item.Quantity,  // Quantity is doing one because driver will scan one by one and send
                            VariantName = item.ProductVariant?.VariantName,
                            CreatedBy = userId ?? 1 // Use provided userId or default to 1
                        };

                        // Check current delivery status before processing
                        var currentDeliveryStatus = await CheckDeliveryStatusAsync(item.Id);
                        
                        // If item is already fully delivered, don't create inventory and return completion message
                        if (currentDeliveryStatus.IsFullyDelivered)
                        {
                            return new List<ProductVariantInfoDto>
                            {
                                new ProductVariantInfoDto
                                {
                                    ProductVariantId = item.ProductVariant?.Id ?? 0,
                                    VariantName = currentDeliveryStatus.Message,
                                    ProdcutId = item.ProductId
                                }
                            };
                        }

                        // Only create inventory if item is not delivered or partially delivered
                        await _locationInventoryDataService.CreateInventoryAsync(createOutwardDto);

                        //Start:- According to this Update data in Van As well
                        if (userId > 0)
                        {
                            //Get Van details from UserId (DriverId in Van entity)
                            var vans = await _vanRepository.GetByDriverIdAsync(userId.Value);
                            var van = vans.FirstOrDefault();

                            if (van != null)
                            {
                                // Get Inventory from VanId from VanInventoryItems table by vanId, productVariantId and make it -1 quantity
                                await UpdateVanInventoryQuantityAsync(van.Id, item.ProductVariantId, 1);
                            }
                        }
                        //End

                        // Update delivered quantity after creating inventory
                        var deliveryStatus = await UpdateDeliveredQuantityForItemAsync(item.Id, createOutwardDto.Quantity);
                        
                        return new List<ProductVariantInfoDto>
                        {
                            new ProductVariantInfoDto
                            {
                                ProductVariantId = item.ProductVariant?.Id ?? 0,
                                VariantName = deliveryStatus.Message,
                                ProdcutId = item.ProductId
                            }
                        };                        
                    }
                    catch (Exception ex)
                    {
                        return new List<ProductVariantInfoDto>
                        {
                            new ProductVariantInfoDto
                            {
                                ProductVariantId = 0,
                                VariantName = $"Error: {ex.Message}",
                                ProdcutId = 0
                            }
                        };
                    }
                }
            }
            else
            {
                return productVariants;
            }

            // Ensure all code paths return a value
            return Enumerable.Empty<ProductVariantInfoDto>();
        }

        public async Task<string> UpdateDeliveredQuantityAsync(int restockRequestItemId, int deliveredQuantity)
        {
            // Find the RestockRequestItem
            var restockRequestItem = await _context.RestockRequestItems
                .Include(rri => rri.RestockRequest)
                    .ThenInclude(rr => rr.Items)
                .FirstOrDefaultAsync(rri => rri.Id == restockRequestItemId);

            if (restockRequestItem == null)
            {
                throw new ArgumentException("RestockRequestItem not found");
            }

            // Validate delivered quantity
            if (deliveredQuantity < 0)
            {
                throw new ArgumentException("Delivered quantity cannot be negative");
            }

            if (deliveredQuantity > restockRequestItem.Quantity)
            {
                throw new ArgumentException($"Delivered quantity ({deliveredQuantity}) cannot exceed requested quantity ({restockRequestItem.Quantity})");
            }

            // Update the delivered quantity
            restockRequestItem.DeliveredQuantity = (restockRequestItem.DeliveredQuantity ?? 0) + deliveredQuantity;

            // Ensure delivered quantity doesn't exceed requested quantity
            if (restockRequestItem.DeliveredQuantity > restockRequestItem.Quantity)
            {
                restockRequestItem.DeliveredQuantity = restockRequestItem.Quantity;
            }

            await _context.SaveChangesAsync();

            // Check if all items in the RestockRequest are fully delivered
            var allItems = restockRequestItem.RestockRequest.Items;
            var allItemsDelivered = allItems.All(item => 
                item.DeliveredQuantity.HasValue && item.DeliveredQuantity.Value >= item.Quantity);

            if (allItemsDelivered)
            {
                // Update RestockRequest status to completed
                // restockRequestItem.RestockRequest.Status = "Completed";
                // restockRequestItem.RestockRequest.UpdatedAt = DateTime.UtcNow;
                // await _context.SaveChangesAsync();

                return "Requested Items delivered - All items in this RestockRequest have been fully delivered";
            }
            else
            {
                // Check if this specific item is fully delivered
                if (restockRequestItem.DeliveredQuantity >= restockRequestItem.Quantity)
                {
                    return $"Item fully delivered - {restockRequestItem.DeliveredQuantity}/{restockRequestItem.Quantity} delivered";
                }
                else
                {
                    return $"Partial delivery recorded - {restockRequestItem.DeliveredQuantity}/{restockRequestItem.Quantity} delivered";
                }
            }
        }

        private async Task<DeliveryStatus> UpdateDeliveredQuantityForItemAsync(int restockRequestItemId, int deliveredQuantity)
        {
            // Find the RestockRequestItem
            var restockRequestItem = await _context.RestockRequestItems
                .Include(rri => rri.RestockRequest)
                    .ThenInclude(rr => rr.Items)
                .FirstOrDefaultAsync(rri => rri.Id == restockRequestItemId);

            if (restockRequestItem == null)
            {
                return new DeliveryStatus
                {
                    IsFullyDelivered = false,
                    Message = "RestockRequestItem not found"
                };
            }

            // Update the delivered quantity
            restockRequestItem.DeliveredQuantity = (restockRequestItem.DeliveredQuantity ?? 0) + deliveredQuantity;

            // Ensure delivered quantity doesn't exceed requested quantity
            if (restockRequestItem.DeliveredQuantity > restockRequestItem.Quantity)
            {
                restockRequestItem.DeliveredQuantity = restockRequestItem.Quantity;
            }

            await _context.SaveChangesAsync();

            // Check if this specific item is fully delivered
            bool isItemFullyDelivered = restockRequestItem.DeliveredQuantity >= restockRequestItem.Quantity;

            if (isItemFullyDelivered)
            {
                // Check if all items in the RestockRequest are fully delivered
                var allItems = restockRequestItem.RestockRequest.Items;
                var allItemsDelivered = allItems.All(item => 
                    item.DeliveredQuantity.HasValue && item.DeliveredQuantity.Value >= item.Quantity);

                if (allItemsDelivered)
                {
                    // Update RestockRequest status to completed
                    restockRequestItem.RestockRequest.Status = "Completed";
                    restockRequestItem.RestockRequest.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    return new DeliveryStatus
                    {
                        IsFullyDelivered = true,
                        Message = "Requested Items delivered - All items in this RestockRequest have been fully delivered"
                    };
                }
                else
                {
                    return new DeliveryStatus
                    {
                        IsFullyDelivered = true,
                        Message = $"Item fully delivered - {restockRequestItem.DeliveredQuantity}/{restockRequestItem.Quantity} delivered"
                    };
                }
            }
            else
            {
                return new DeliveryStatus
                {
                    IsFullyDelivered = false,
                    Message = $"Partial delivery - {restockRequestItem.DeliveredQuantity}/{restockRequestItem.Quantity} delivered"
                };
            }
        }

        private async Task<DeliveryStatus> CheckDeliveryStatusAsync(int restockRequestItemId)
        {
            // Find the RestockRequestItem
            var restockRequestItem = await _context.RestockRequestItems
                .FirstOrDefaultAsync(rri => rri.Id == restockRequestItemId);

            if (restockRequestItem == null)
            {
                return new DeliveryStatus
                {
                    IsFullyDelivered = false,
                    Message = "RestockRequestItem not found"
                };
            }

            // Check if this specific item is already fully delivered
            bool isItemFullyDelivered = restockRequestItem.DeliveredQuantity.HasValue && 
                                       restockRequestItem.DeliveredQuantity.Value >= restockRequestItem.Quantity;

            if (isItemFullyDelivered)
            {
                return new DeliveryStatus
                {
                    IsFullyDelivered = true,
                    Message = $"Item already fully delivered - {restockRequestItem.DeliveredQuantity}/{restockRequestItem.Quantity} delivered"
                };
            }
            else
            {
                var currentDelivered = restockRequestItem.DeliveredQuantity ?? 0;
                return new DeliveryStatus
                {
                    IsFullyDelivered = false,
                    Message = $"Item available for delivery - {currentDelivered}/{restockRequestItem.Quantity} delivered"
                };
            }
        }

        /// <summary>
        /// Updates van inventory quantity for a specific product variant
        /// </summary>
        /// <param name="vanId">Van ID</param>
        /// <param name="productVariantId">Product Variant ID</param>
        /// <param name="quantityChange">Quantity change (negative to decrease)</param>
        /// <returns>Task</returns>
        private async Task UpdateVanInventoryQuantityAsync(int vanId, int? productVariantId, int quantityChange)
        {
            if (!productVariantId.HasValue) return;

            // Use direct SQL query to get and update van inventory item
            var vanInventoryItems = await _context.Database.SqlQueryRaw<VanInventoryItemResult>(
                @"SELECT vi.VanId, vii.* 
                  FROM VanInventories vi 
                  INNER JOIN VanInventoryItems vii ON vi.Id = vii.VanInventoryId 
                  WHERE vi.VanId = {0} AND vii.ProductVariantId = {1}", 
                vanId, productVariantId.Value).ToListAsync();
            
            var vanInventoryItem = vanInventoryItems.FirstOrDefault();
            
            if (vanInventoryItem != null && vanInventoryItem.Quantity > 0)
            {
                // Update quantity directly using SQL
                await _context.Database.ExecuteSqlRawAsync(
                    @"UPDATE VanInventoryItems 
                      SET Quantity = Quantity - {0} 
                      WHERE Id = {1}", 
                    quantityChange, vanInventoryItem.Id);
            }
        }
       
    }
}
