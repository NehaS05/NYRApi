using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VansController : ControllerBase
    {
        private readonly IVanService _vanService;

        public VansController(IVanService vanService)
        {
            _vanService = vanService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult> GetAll([FromQuery] PaginationParamsDto? paginationParams = null)
        {
            if (paginationParams != null && (Request.Query.ContainsKey("pageNumber") || Request.Query.ContainsKey("pageSize")))
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _vanService.GetVansPagedAsync(paginationParams);
                return Ok(result);
            }

            var vans = await _vanService.GetAllAsync();
            return Ok(vans);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<VanDto>> GetById(int id)
        {
            var van = await _vanService.GetByIdAsync(id);
            if (van == null) return NotFound();
            return Ok(van);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Staff,Driver")]
        public async Task<ActionResult<IEnumerable<VanDto>>> Search([FromQuery] string searchTerm)
        {
            var vans = await _vanService.SearchAsync(searchTerm);
            return Ok(vans);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VanDto>> Create([FromBody] CreateVanDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _vanService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VanDto>> Update(int id, [FromBody] UpdateVanDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updated = await _vanService.UpdateAsync(id, dto);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var ok = await _vanService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}


