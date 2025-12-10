using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class RestockRequestService : IRestockRequestService
    {
        private readonly IRestockRequestRepository _restockRequestRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGenericRepository<ProductVariant> _productVariantRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RestockRequestService(
            IRestockRequestRepository restockRequestRepository,
            ICustomerRepository customerRepository,
            ILocationRepository locationRepository,
            IProductRepository productRepository,
            IGenericRepository<ProductVariant> productVariantRepository,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _restockRequestRepository = restockRequestRepository;
            _customerRepository = customerRepository;
            _locationRepository = locationRepository;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RestockRequestDto>> GetAllRequestsAsync()
        {
            var requests = await _restockRequestRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<RestockRequestDto>>(requests);
        }

        public async Task<RestockRequestDto?> GetRequestByIdAsync(int id)
        {
            var request = await _restockRequestRepository.GetByIdWithDetailsAsync(id);
            return request != null ? _mapper.Map<RestockRequestDto>(request) : null;
        }

        public async Task<IEnumerable<RestockRequestDto>> GetRequestsByLocationIdAsync(int locationId)
        {
            var requests = await _restockRequestRepository.GetByLocationIdAsync(locationId);
            var requestDtos = _mapper.Map<IEnumerable<RestockRequestDto>>(requests).ToList();
            
            // Get DeliveryDate from Route information for each RestockRequest
            foreach (var dto in requestDtos)
            {
                var deliveryDate = await _context.RouteStops
                    .Where(rs => rs.RestockRequestId == dto.Id && rs.IsActive)
                    .Join(_context.Routes, rs => rs.RouteId, r => r.Id, (rs, r) => r.DeliveryDate)
                    .FirstOrDefaultAsync();
                
                dto.DeliveryDate = deliveryDate == default(DateTime) ? null : deliveryDate;
            }
            
            return requestDtos;
        }

        public async Task<IEnumerable<RestockRequestDto>> GetRequestsByCustomerIdAsync(int customerId)
        {
            var requests = await _restockRequestRepository.GetByCustomerIdAsync(customerId);
            return _mapper.Map<IEnumerable<RestockRequestDto>>(requests);
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
            var existingRequest = existingRequests.FirstOrDefault(r => 
                r.CustomerId == createDto.CustomerId && 
                r.LocationId == createDto.LocationId && 
                r.RequestDate.Date == today &&
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
    }
}
