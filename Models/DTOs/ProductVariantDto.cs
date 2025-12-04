namespace NYR.API.Models.DTOs
{
    public class ProductVariantDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public decimal? Price { get; set; }
        public bool IsEnabled { get; set; }
        public List<ProductVariantAttributeDto> Attributes { get; set; } = new List<ProductVariantAttributeDto>();
    }

    public class ProductVariantAttributeDto
    {
        public int Id { get; set; }
        public int VariationId { get; set; }
        public string VariationName { get; set; } = string.Empty;
        public int VariationOptionId { get; set; }
        public string VariationOptionName { get; set; } = string.Empty;
        public string? VariationOptionValue { get; set; }
    }

    public class CreateProductVariantDto
    {
        public string VariantName { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public decimal? Price { get; set; }
        public bool IsEnabled { get; set; } = true;
        public List<CreateProductVariantAttributeDto> Attributes { get; set; } = new List<CreateProductVariantAttributeDto>();
    }

    public class CreateProductVariantAttributeDto
    {
        public int VariationId { get; set; }
        public int VariationOptionId { get; set; }
    }
}
