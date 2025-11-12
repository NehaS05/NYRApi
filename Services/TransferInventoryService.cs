using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class TransferInventoryService : ITransferInventoryService
    {
        private readonly ITransferInventoryRepository _transferInventoryRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductVariationRepository _productVariationRepository;
        private readonly IMapper _mapper;

        public TransferInventoryService(
            ITransferInventoryRepository transferInventoryRepository,
            ICustomerRepository customerRepository,
            ILocationRepository locationRepository,
            IProductRepository productRepository,
            IProductVariationRepository productVariationRepository,
            IMapper mapper)
        {
            _transferInventoryRepository = transferInventoryRepository;
            _customerRepository = customerRepository;
            _locationRepository = locationRepository;
            _productRepository = productRepository;
            _productVariationRepository = productVariationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TransferInventoryLocationDto>> GetLocationsWithTransfersAsync()
        {
            var transfers = await _transferInventoryRepository.GetAllWithDetailsAsync();
            
            var locationGroups = transfers
                .GroupBy(t => t.LocationId)
                .Select(g => g.First())
                .Select(t => new TransferInventoryLocationDto
                {
                    LocationId = t.LocationId,
                    LocationName = t.Location.LocationName,
                    CustomerId = t.CustomerId,
                    CustomerName = t.Customer.CompanyName,
                    ContactPerson = t.Location.ContactPerson,
                    LocationNumber = t.Location.LocationPhone
                })
                .ToList();

            return locationGroups;
        }

        public async Task<IEnumerable<TransferInventoryDto>> GetAllTransfersAsync()
        {
            var transfers = await _transferInventoryRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<TransferInventoryDto>>(transfers);
        }

        public async Task<TransferInventoryDto?> GetTransferByIdAsync(int id)
        {
            var transfer = await _transferInventoryRepository.GetByIdWithDetailsAsync(id);
            return transfer != null ? _mapper.Map<TransferInventoryDto>(transfer) : null;
        }

        public async Task<IEnumerable<TransferInventoryItemDto>> GetTransferItemsByLocationIdAsync(int locationId)
        {
            var transfers = await _transferInventoryRepository.GetByLocationIdAsync(locationId);
            var allItems = transfers.SelectMany(t => t.Items).ToList();
            return _mapper.Map<IEnumerable<TransferInventoryItemDto>>(allItems);
        }

        public async Task<TransferInventoryDto> CreateTransferAsync(CreateTransferInventoryDto createDto)
        {
            // Validate customer exists
            var customer = await _customerRepository.GetByIdAsync(createDto.CustomerId);
            if (customer == null)
                throw new ArgumentException("Invalid customer ID");

            // Validate location exists
            var location = await _locationRepository.GetByIdAsync(createDto.LocationId);
            if (location == null)
                throw new ArgumentException("Invalid location ID");

            // Validate products exist
            foreach (var item in createDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new ArgumentException($"Invalid product ID: {item.ProductId}");

                if (item.ProductVariationId.HasValue)
                {
                    var variation = await _productVariationRepository.GetByIdAsync(item.ProductVariationId.Value);
                    if (variation == null)
                        throw new ArgumentException($"Invalid product variation ID: {item.ProductVariationId}");
                }
            }

            var transfer = _mapper.Map<TransferInventory>(createDto);
            transfer.Items = createDto.Items.Select(i => new TransferInventoryItem
            {
                ProductId = i.ProductId,
                ProductVariationId = i.ProductVariationId,
                Quantity = i.Quantity,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            var createdTransfer = await _transferInventoryRepository.AddAsync(transfer);
            return _mapper.Map<TransferInventoryDto>(await _transferInventoryRepository.GetByIdWithDetailsAsync(createdTransfer.Id));
        }

        public async Task<bool> DeleteTransferAsync(int id)
        {
            var transfer = await _transferInventoryRepository.GetByIdAsync(id);
            if (transfer == null)
                return false;

            await _transferInventoryRepository.DeleteAsync(transfer);
            return true;
        }
    }
}
