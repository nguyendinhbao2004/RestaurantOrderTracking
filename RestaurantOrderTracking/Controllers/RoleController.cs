using Application.Feature.Roles.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create a new role.
        /// </summary>
        /// <remarks>
        /// How to use:
        /// - Call <c>POST /api/Role</c>.
        /// - Request body:
        ///   - <c>name</c>: unique role name.
        ///   - <c>description</c>: role description.
        ///
        /// Sample request body:
        /// <code>
        /// {
        ///   "name": "Supervisor",
        ///   "description": "Can monitor operations and support manager"
        /// }
        /// </code>
        /// </remarks>
        /// <param name="command">Role creation payload.</param>
        /// <returns>Returns created role id when successful.</returns>
        /// <response code="200">Role created successfully.</response>
        /// <response code="400">Validation failed or role name already exists.</response>
        /// <response code="500">Unexpected server-side error.</response>
        [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}