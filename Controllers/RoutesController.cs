using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NYR.API.Models.DTOs;
using NYR.API.Services.Interfaces;

namespace NYR.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RoutesController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetAllRoutes()
        {
            var routes = await _routeService.GetAllRoutesAsync();
            return Ok(routes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RouteDto>> GetRoute(int id)
        {
            var route = await _routeService.GetRouteByIdAsync(id);
            if (route == null)
                return NotFound();

            return Ok(route);
        }

        [HttpGet("by-location/{locationId}")]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetRoutesByLocation(int locationId)
        {
            var routes = await _routeService.GetRoutesByLocationIdAsync(locationId);
            return Ok(routes);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetRoutesByUser(int userId)
        {
            var routes = await _routeService.GetRoutesByUserIdAsync(userId);
            return Ok(routes);
        }

        [HttpGet("by-date/{deliveryDate}")]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetRoutesByDeliveryDate(DateTime deliveryDate)
        {
            var routes = await _routeService.GetRoutesByDeliveryDateAsync(deliveryDate);
            return Ok(routes);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<RouteDto>> CreateRoute([FromBody] CreateRouteDto createRouteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var route = await _routeService.CreateRouteAsync(createRouteDto);
                return CreatedAtAction(nameof(GetRoute), new { id = route.Id }, route);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<RouteDto>> UpdateRoute(int id, [FromBody] UpdateRouteDto updateRouteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var route = await _routeService.UpdateRouteAsync(id, updateRouteDto);
                if (route == null)
                    return NotFound();

                return Ok(route);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteRoute(int id)
        {
            var result = await _routeService.DeleteRouteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
