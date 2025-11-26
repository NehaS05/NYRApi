using AutoMapper;
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
        private readonly IProductVariationRepository _productVariationRepository;
        private readonly IMapper _mapper;

        public RestockRequestService(
            IRestockRequestRepository restockRequestRepository,
            ICustomerRepository customerRepository,
            ILocationRepository locationRepository,
            IProductRepository productRepository,
            IProductVariationRepository productVariationRepository,
            IMapper mapper)
        {
            _restockRequestRepository = restockRequestRepository;
            _customerRepository = customerRepository;
            _locationRepository = locationRepository;
            _productRepository = productRepository;
            _productVariationRepository = productVariationRepository;
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
            return _mapper.Map<IEnumerable<RestockRequestDto>>(requests);
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

                if (item.ProductVariationId.HasValue)
                {
                    var variation = await _productVariationRepository.GetByIdAsync(item.ProductVariationId.Value);
                    if (variation == null)
                        throw new ArgumentException($"Product variation with ID {item.ProductVariationId} not found");

                    if (variation.ProductId != item.ProductId)
                        throw new ArgumentException($"Product variation {item.ProductVariationId} does not belong to product {item.ProductId}");
                }
            }

            // Create RestockRequest entity
            var restockRequest = new RestockRequest
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
                ProductVariationId = itemDto.ProductVariationId,
                Quantity = itemDto.Quantity,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            var created = await _restockRequestRepository.AddAsync(restockRequest);
            
            // Fetch with details for proper mapping
            var result = await _restockRequestRepository.GetByIdWithDetailsAsync(created.Id);
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
