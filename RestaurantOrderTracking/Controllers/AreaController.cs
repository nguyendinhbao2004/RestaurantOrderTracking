using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderTracking.Application.Feature.Area.Commands.Create;
using RestaurantOrderTracking.Application.Feature.Area.Queries.GetAll;

namespace RestaurantOrderTracking.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing areas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AreaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets all areas.
        /// </summary>
        /// <returns>List of all areas.</returns>
        /// <response code="200">Returns all areas.</response>
        [HttpGet]
        public async Task<IActionResult> GetAllAreas()
        {
            var query = new GetAllAreasQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new area.
        /// </summary>
        /// <param name="command">Area creation request.</param>
        /// <returns>The ID of the created area.</returns>
        /// <response code="200">Area created successfully.</response>
        /// <response code="400">Validation failed.</response>
        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] CreateAreaCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
