using AutoMapper;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class RequestSupplyService : IRequestSupplyService
    {
        private readonly IRequestSupplyRepository _requestSupplyRepository;
        private readonly IRequestSupplyItemRepository _requestSupplyItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly IVariationOptionRepository _variationOptionRepository;
        private readonly IMapper _mapper;

        public RequestSupplyService(
            IRequestSupplyRepository requestSupplyRepository,
            IRequestSupplyItemRepository requestSupplyItemRepository,
            IProductRepository productRepository,
            IVariationOptionRepository variationOptionRepository,
            IMapper mapper)
        {
            _requestSupplyRepository = requestSupplyRepository;
            _requestSupplyItemRepository = requestSupplyItemRepository;
            _productRepository = productRepository;
            _variationOptionRepository = variationOptionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RequestSupplyDto>> GetAllRequestSuppliesAsync()
        {
            var requestSupplies = await _requestSupplyRepository.GetAllWithItemsAsync();
            return _mapper.Map<IEnumerable<RequestSupplyDto>>(requestSupplies);
        }

        public async Task<RequestSupplyDto?> GetRequestSupplyByIdAsync(int id)
        {
            var requestSupply = await _requestSupplyRepository.GetByIdWithItemsAsync(id);
            return requestSupply != null ? _mapper.Map<RequestSupplyDto>(requestSupply) : null;
        }

        public async Task<RequestSupplyDto> CreateRequestSupplyAsync(CreateRequestSupplyDto createRequestSupplyDto)
        {
            // Validate products and variation options exist
            try
            {
                foreach (var item in createRequestSupplyDto.Items)
                {
                    var product = await _productRepository.GetByIdAsync(createRequestSupplyDto.ProductId);
                    if (product == null)
                        throw new ArgumentException($"Invalid product ID: {createRequestSupplyDto.ProductId}");

                    var variationOption = await _variationOptionRepository.GetByIdAsync(item.VariationId);
                    if (variationOption == null)
                        throw new ArgumentException($"Invalid variation option ID: {item.VariationId}");
                }

                var requestSupply = _mapper.Map<RequestSupply>(createRequestSupplyDto);
                var createdRequestSupply = await _requestSupplyRepository.AddAsync(requestSupply);

                //// Create items
                foreach (var itemDto in createRequestSupplyDto.Items)
                {
                    var item = _mapper.Map<RequestSupplyItem>(itemDto);
                    item.RequestSupplyId = createdRequestSupply.Id;
                    await _requestSupplyItemRepository.AddAsync(item);
                }

                return await GetRequestSupplyByIdAsync(createdRequestSupply.Id) ?? throw new Exception("Failed to retrieve created request supply");
            } catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<RequestSupplyDto?> UpdateRequestSupplyAsync(int id, UpdateRequestSupplyDto updateRequestSupplyDto)
        {
            var requestSupply = await _requestSupplyRepository.GetByIdAsync(id);
            if (requestSupply == null)
                return null;

            // Validate products and variation options exist
            foreach (var item in updateRequestSupplyDto.Items)
            {
                //var product = await _productRepository.GetByIdAsync(item.ProductId);
                //if (product == null)
                //    throw new ArgumentException($"Invalid product ID: {item.ProductId}");

                var variationOption = await _variationOptionRepository.GetByIdAsync(item.VariationId);
                if (variationOption == null)
                    throw new ArgumentException($"Invalid variation option ID: {item.VariationId}");
            }

            _mapper.Map(updateRequestSupplyDto, requestSupply);
            requestSupply.UpdatedAt = DateTime.UtcNow;
            await _requestSupplyRepository.UpdateAsync(requestSupply);

            // Update items
            var existingItems = await _requestSupplyItemRepository.GetByRequestSupplyIdAsync(id);
            
            // Remove items not in the update
            var itemsToRemove = existingItems.Where(ei => !updateRequestSupplyDto.Items.Any(i => i.Id == ei.Id)).ToList();
            foreach (var item in itemsToRemove)
            {
                await _requestSupplyItemRepository.DeleteAsync(item.Id);
            }

            // Add or update items
            foreach (var itemDto in updateRequestSupplyDto.Items)
            {
                if (itemDto.Id.HasValue)
                {
                    var existingItem = existingItems.FirstOrDefault(ei => ei.Id == itemDto.Id.Value);
                    if (existingItem != null)
                    {
                        _mapper.Map(itemDto, existingItem);
                        await _requestSupplyItemRepository.UpdateAsync(existingItem);
                    }
                }
                else
                {
                    var newItem = _mapper.Map<RequestSupplyItem>(itemDto);
                    newItem.RequestSupplyId = id;
                    await _requestSupplyItemRepository.AddAsync(newItem);
                }
            }

            return await GetRequestSupplyByIdAsync(id);
        }

        public async Task<bool> DeleteRequestSupplyAsync(int id)
        {
            return await _requestSupplyRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<RequestSupplyDto>> GetRequestSuppliesByStatusAsync(string status)
        {
            var requestSupplies = await _requestSupplyRepository.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<RequestSupplyDto>>(requestSupplies);
        }
    }
}
