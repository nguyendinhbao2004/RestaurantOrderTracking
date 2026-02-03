using Application.Feature.Tables.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestaurantOrderTracking.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TableController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new table based on the specified command.
        /// </summary>
        /// <remarks>
        /// Api endpoint to create a new table in the system
        /// <br/>
        /// **Sample Request**: Login with valid credentials
        /// </remarks>
        /// <param name="command">The command containing the details required to create the table. Cannot be null.</param>
        /// <returns>An <see cref="IActionResult"/> that represents the result of the create operation. Returns a 200 OK response
        /// with the result of the table creation.</returns>
        /// <response code="200">Create Table Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        public async Task<IActionResult> CreateTable([FromBody] CreateTableCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
