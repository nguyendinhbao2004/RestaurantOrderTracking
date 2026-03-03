using System.Globalization;
using Application.Feature.Tables.Commands.Create;
using Application.Feature.Tables.Commands.Update.UpdateInfo;
using Application.Feature.Tables.Commands.Update.UpdateStatus;
using Application.Feature.Tables.Queries.GetAllTable;
using Application.Feature.Tables.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderTracking.Application.Feature.Tables.Commands.GenerateQRSession;
using RestaurantOrderTracking.Application.Feature.Tables.Commands.RefreshQRSession;

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
        /// Gets all tables with optional filtering and pagination.
        /// </summary>
        /// <remarks>
        /// Api endpoint to get all data of table in the system
        /// <br/>
        /// **Sample Request**: Login with valid credentials
        /// </remarks>
        /// <param name="Keyword">Optional keyword to filter tables by name or description.</param>
        /// <param name="PageIndex">The zero-based index of the page to retrieve.</param>
        /// <param name="PageSize">The number of items to include in each page.</param>
        /// <returns>A list of tables matching the criteria, paginated.</returns>
        /// <response code="200">Returns the list of tables matching the criteria.</response>
        /// <response code="400">If the request parameters are invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet]
        public async Task<IActionResult> GetAllTables(string? Keyword, int PageIndex = 1, int PageSize = 10)
        {
            var query = new GetAllTableQueries(Keyword, PageIndex, PageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Gets the details of a specific table by its ID.
        /// </summary>
        /// <remarks>
        /// Api endpoint to get detail of a table in the system
        /// <br/>
        /// **Sample Request**: Login with valid credentials
        /// </remarks>  
        /// <param name="id">The unique identifier of the table to retrieve.</param>
        /// <returns>The details of the specified table.</returns>
        /// <response code="200">Returns the details of the specified table.</response>
        /// <response code="400">If the provided ID is invalid.</response>
        /// <response code="404">If a table with the specified ID is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTableById([FromRoute] Guid id)
        {
            var query = new GetTableByIdQueries(id);
            var result = await _mediator.Send(query);
            return Ok(result);
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
        
        /// <summary>
        /// Updates the status of an existing table.
        /// </summary>
        /// <remarks>
        /// Api endpoint to update the status of a table in the system
        /// <br/>
        /// **Sample Request**: Login with valid credentials
        /// </remarks>
        /// <param name="command">The command containing the details required to update the table status. Cannot be null.</param>
        /// <returns>An <see cref="IActionResult"/> that represents the result of the update operation. Returns a 200 OK response
        /// with the result of the table status update.</returns>
        /// <response code="200">Update Table Status Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut("update-info")]
        public async Task<IActionResult> UpdateTableInfo([FromBody] UpdateTableCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Updates the status of an existing table.
        /// </summary>
        /// <remarks>
        /// Api endpoint to update the status of a table in the system
        /// <br/>
        /// **Sample Request**: Login with valid credentials
        /// </remarks>
        /// <param name="command">The command containing the details required to update the table status. Cannot be null.
        /// <br/>Available = 0,
        ///<br/>Occupied = 1,
        ///<br/>Reserved = 2,
        ///<br/>OutOfService = 3
        /// </param>
        /// <returns>An <see cref="IActionResult"/> that represents the result of the update operation. Returns a 200 OK response
        /// with the result of the table status update.</returns>
        /// <response code="200">Update Table Status Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateTableStatus([FromBody] UpdateStatusTableCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        #region QR Session Management

        /// <summary>
        /// Generates a new QR session for a table.
        /// </summary>
        /// <remarks>
        /// This will revoke all existing active sessions for the table and create a new one.
        /// The session token is also updated on the Table entity.
        /// Default expiration: 8 hours (480 minutes).
        /// </remarks>
        /// <param name="tableId">The ID of the table to generate a QR session for.</param>
        /// <returns>The generated QR session information.</returns>
        /// <response code="200">QR session generated successfully.</response>
        /// <response code="400">Table not found.</response>
        [HttpPost("qr-session/{tableId}")]
        public async Task<IActionResult> GenerateQRSession([FromRoute] Guid tableId)
        {
            var command = new GenerateQRSessionCommand(tableId);
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Refreshes the QR session for a table.
        /// </summary>
        /// <remarks>
        /// If an active session exists, it generates a new token and extends the expiration.
        /// If no active session exists, a new one is created.
        /// </remarks>
        /// <param name="tableId">The ID of the table to refresh the QR session for.</param>
        /// <returns>The refreshed QR session information.</returns>
        /// <response code="200">QR session refreshed successfully.</response>
        /// <response code="400">Table not found.</response>
        [HttpPut("qr-session/{tableId}/refresh")]
        public async Task<IActionResult> RefreshQRSession([FromRoute] Guid tableId)
        {
            var command = new RefreshQRSessionCommand(tableId);
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        #endregion

    }
}
