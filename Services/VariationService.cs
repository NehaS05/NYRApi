using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NYR.API.Data;
using NYR.API.Models.DTOs;
using NYR.API.Models.Entities;
using NYR.API.Repositories.Interfaces;
using NYR.API.Services.Interfaces;

namespace NYR.API.Services
{
    public class VariationService : IVariationService
    {
        private readonly IVariationRepository _variationRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public VariationService(IVariationRepository variationRepository, ApplicationDbContext context, IMapper mapper)
        {
            _variationRepository = variationRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VariationDto>> GetAllVariationsAsync()
        {
            var variations = await _variationRepository.GetAllWithOptionsAsync();
            variations = variations.Where(x => x.IsActive == true);
            return _mapper.Map<IEnumerable<VariationDto>>(variations);
        }

        public async Task<VariationDto?> GetVariationByIdAsync(int id)
        {
            var variation = await _variationRepository.GetByIdWithOptionsAsync(id);
            return variation != null ? _mapper.Map<VariationDto>(variation) : null;
        }

        public async Task<VariationDto> CreateVariationAsync(CreateVariationDto createVariationDto)
        {
            // Check if variation with same name already exists
            var existingVariation = await _variationRepository.GetByNameAsync(createVariationDto.Name);
            if (existingVariation != null)
                throw new ArgumentException("Variation with this name already exists");

            // Validate ValueType
            if (createVariationDto.ValueType != "Dropdown" && createVariationDto.ValueType != "TextInput")
                throw new ArgumentException("ValueType must be either 'Dropdown' or 'TextInput'");

            var variation = _mapper.Map<Variation>(createVariationDto);
            
            // Map options
            if (createVariationDto.Options != null && createVariationDto.Options.Any())
            {
                variation.Options = createVariationDto.Options.Select(o => new VariationOption
                {
                    Name = o.Name,
                    Value = o.Value,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
            }

            var createdVariation = await _variationRepository.AddAsync(variation);
            return _mapper.Map<VariationDto>(createdVariation);
        }

        public async Task<VariationDto?> UpdateVariationAsync(int id, UpdateVariationDto updateVariationDto)
        {
            var variation = await _variationRepository.GetByIdWithOptionsAsync(id);
            if (variation == null)
                return null;

            // Check if another variation with same name exists
            var existingVariation = await _variationRepository.GetByNameAsync(updateVariationDto.Name);
            if (existingVariation != null && existingVariation.Id != id)
                throw new ArgumentException("Variation with this name already exists");

            // Validate ValueType
            if (updateVariationDto.ValueType != "Dropdown" && updateVariationDto.ValueType != "TextInput")
                throw new ArgumentException("ValueType must be either 'Dropdown' or 'TextInput'");

            // Update variation properties
            variation.Name = updateVariationDto.Name;
            variation.ValueType = updateVariationDto.ValueType;
            variation.IsActive = updateVariationDto.IsActive;
            variation.UpdatedAt = DateTime.UtcNow;

            // Handle options update
            var existingOptionIds = variation.Options.Select(o => o.Id).ToList();
            var updatedOptionIds = updateVariationDto.Options.Where(o => o.Id.HasValue).Select(o => o.Id!.Value).ToList();

            // Remove options that are not in the update list
            var optionsToRemove = variation.Options.Where(o => !updatedOptionIds.Contains(o.Id)).ToList();
            foreach (var option in optionsToRemove)
            {
                _context.Set<VariationOption>().Remove(option);
            }

            // Update existing options and add new ones
            foreach (var optionDto in updateVariationDto.Options)
            {
                if (optionDto.Id.HasValue)
                {
                    // Update existing option
                    var existingOption = variation.Options.FirstOrDefault(o => o.Id == optionDto.Id.Value);
                    if (existingOption != null)
                    {
                        existingOption.Name = optionDto.Name;
                        existingOption.Value = optionDto.Value;
                        existingOption.IsActive = optionDto.IsActive;
                    }
                }
                else
                {
                    // Add new option
                    variation.Options.Add(new VariationOption
                    {
                        VariationId = variation.Id,
                        Name = optionDto.Name,
                        Value = optionDto.Value,
                        IsActive = optionDto.IsActive,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _variationRepository.UpdateAsync(variation);
            
            // Reload to get updated options
            var updatedVariation = await _variationRepository.GetByIdWithOptionsAsync(id);
            return _mapper.Map<VariationDto>(updatedVariation);
        }

        public async Task<bool> DeleteVariationAsync(int id)
        {
            var variation = await _variationRepository.GetByIdAsync(id);
            if (variation == null)
                return false;

            // Check if there are variation options associated with this variation
            var hasOptionsQuery = _context.VariationOptions
                .Where(vo => vo.VariationId == id && vo.IsActive);
            
            var hasOptions = await hasOptionsQuery.AnyAsync();
            
            if (hasOptions)
            {
                // Check if any variation options are referenced in ProductVariantAttributes
                var hasAttributeReferencesQuery = _context.ProductVariantAttributes
                    .Where(pva => _context.VariationOptions.Any(vo => vo.Id == pva.VariationOptionId && vo.VariationId == id));
                
                var hasAttributeReferences = await hasAttributeReferencesQuery.AnyAsync();
                
                if (hasAttributeReferences)
                {
                    // Soft delete: deactivate variation and its options
                    variation.IsActive = false;
                    variation.UpdatedAt = DateTime.UtcNow;
                    await _variationRepository.UpdateAsync(variation);
                    
                    // Deactivate associated variation options
                    var options = await hasOptionsQuery.ToListAsync();
                    foreach (var option in options)
                    {
                        option.IsActive = false;
                    }
                    
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // No attribute references, but has options - soft delete variation and options
                    variation.IsActive = false;
                    variation.UpdatedAt = DateTime.UtcNow;
                    await _variationRepository.UpdateAsync(variation);
                    
                    var options = await hasOptionsQuery.ToListAsync();
                    foreach (var option in options)
                    {
                        option.IsActive = false;
                    }
                    
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Check if variation itself is referenced in ProductVariantAttributes
                var hasDirectAttributeReferences = await _context.ProductVariantAttributes
                    .AnyAsync(pva => pva.VariationId == id);
                
                if (hasDirectAttributeReferences)
                {
                    // Soft delete variation
                    variation.IsActive = false;
                    variation.UpdatedAt = DateTime.UtcNow;
                    await _variationRepository.UpdateAsync(variation);
                }
                else
                {
                    // No references, safe to hard delete
                    await _variationRepository.DeleteAsync(variation);
                }
            }
            
            return true;
        }

        public async Task<IEnumerable<VariationDto>> GetActiveVariationsAsync()
        {
            var variations = await _variationRepository.GetActiveVariationsAsync();
            return _mapper.Map<IEnumerable<VariationDto>>(variations);
        }
    }
}
