using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            _warehouseService = warehouseService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult> GetAll([FromQuery] PaginationParamsDto? paginationParams = null)
        {
            if (paginationParams != null && (Request.Query.ContainsKey("pageNumber") || Request.Query.ContainsKey("pageSize")))
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _warehouseService.GetWarehousesPagedAsync(paginationParams);
                return Ok(result);
            }

            var items = await _warehouseService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<WarehouseDto>> GetById(int id)
        {
            var item = await _warehouseService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<IEnumerable<WarehouseDto>>> Search([FromQuery] string searchTerm)
        {
            var items = await _warehouseService.SearchAsync(searchTerm);
            return Ok(items);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<WarehouseDto>> Create([FromBody] CreateWarehouseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _warehouseService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<WarehouseDto>> Update(int id, [FromBody] UpdateWarehouseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _warehouseService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var ok = await _warehouseService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}


