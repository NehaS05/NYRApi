using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("catalogue")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetCatalogueProducts()
        {
            var products = await _productService.GetCatalogueProductsAsync();
            return Ok(products);
        }

        [HttpGet("universal")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetUniversalProducts()
        {
            var products = await _productService.GetUniversalProductsAsync();
            return Ok(products);
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        [HttpGet("by-brand/{brandId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByBrand(int brandId)
        {
            var products = await _productService.GetProductsByBrandAsync(brandId);
            return Ok(products);
        }

        [HttpGet("by-supplier/{supplierId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsBySupplier(int supplierId)
        {
            var products = await _productService.GetProductsBySupplierAsync(supplierId);
            return Ok(products);
        }

        [HttpGet("by-barcode/{barcode}")]
        public async Task<ActionResult<ProductDto>> GetProductByBarcode(string barcode)
        {
            var product = await _productService.GetProductByBarcodeAsync(barcode);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = await _productService.CreateProductAsync(createProductDto);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = await _productService.UpdateProductAsync(id, updateProductDto);
                if (product == null)
                    return NotFound();

                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
