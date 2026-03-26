using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderTracking.Application.Feature.Area.Commands.Create;
using RestaurantOrderTracking.Application.Feature.Area.Commands.Delete;
using RestaurantOrderTracking.Application.Feature.Area.Commands.Update;
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

        /// <summary>
        /// Updates an existing area.
        /// </summary>
        /// <param name="command">Update request with optional fields.</param>
        /// <returns>Result of the update operation.</returns>
        /// <response code="200">Area updated successfully.</response>
        /// <response code="400">Update failed.</response>
        [HttpPut]
        public async Task<IActionResult> UpdateArea([FromBody] UpdateAreaCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Deletes an area by ID.
        /// </summary>
        /// <param name="id">The ID of the area to delete.</param>
        /// <returns>Result of the delete operation.</returns>
        /// <response code="200">Area deleted successfully.</response>
        /// <response code="400">Delete failed (area has tables or waiters).</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea([FromRoute] Guid id)
        {
            var command = new DeleteAreaCommand(id);
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Gets waiters by area ID.
        /// </summary>
        /// <param name="id">The ID of the area.</param>
        /// <returns>List of waiters in the area.</returns>
        /// <response code="200">Returns list of waiters.</response>
        [HttpGet("{id}/waiters")]
        public async Task<IActionResult> GetWaitersByArea([FromRoute] Guid id)
        {
            var query = new Application.Feature.Area.Queries.GetWaiters.GetWaitersByAreaQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
