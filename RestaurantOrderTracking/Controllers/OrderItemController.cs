using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderTracking.Application.Feature.OrderItem.Commands.Create;
using RestaurantOrderTracking.Application.Feature.OrderItem.Commands.AssignChef;
using RestaurantOrderTracking.Application.Feature.OrderItem.Commands.UpdateInfo;
using RestaurantOrderTracking.Application.Feature.OrderItem.Commands.UpdateStatus;
using RestaurantOrderTracking.Application.Feature.OrderItem.Queries.GetAllOrderItem;
using RestaurantOrderTracking.Application.Feature.OrderItem.Queries.GetConfirmedOrderItems;
using RestaurantOrderTracking.Application.Feature.OrderItem.Queries.GetOrderItemsByStatus;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace RestaurantOrderTracking.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a paginated list of order items, optionally filtered by product name.
        /// </summary>
        /// <param name="keyword">An optional search term to filter order items by product name. If null or empty, all order items are retrieved.</param>
        /// <param name="pageIndex">The page number to retrieve. Must be 1 or greater. Defaults to 1.</param>
        /// <param name="pageSize">The number of order items to include per page. Must be 1 or greater. Defaults to 10.</param>
        /// <returns>A paginated list of order items with OrderId, ProductName, and Status.</returns>
        /// <response code="200">Returns a paginated list of order items.</response>
        /// <response code="400">Returns a bad request if the input parameters are invalid.</response>
        /// <response code="500">Returns an internal server error if an unexpected error occurs.</response>
        [HttpGet]
        public async Task<IActionResult> GetAllOrderItems(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var query = new GetAllOrderItemsQuery(keyword, pageIndex, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Creates multiple order items for a single order in one request.
        /// </summary>
        /// <remarks>
        /// The shared fields (OrderId, OrderChannel, CreatedBy) apply to every item.
        /// Each entry in Items contains ProductId, optional Note, and Quantity.
        /// </remarks>
        /// <param name="command">The create order items request.</param>
        /// <returns>Result of the operation.</returns>
        /// <response code="200">Order items created successfully.</response>
        /// <response code="400">Validation failed or order not found.</response>
        [HttpPost]
        public async Task<IActionResult> CreateOrderItems([FromBody] CreateOrderItemsCommand command)
        {
            var accountIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                                 ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                                 ?? User.FindFirstValue("sub");

            Guid? createdBy = Guid.TryParse(accountIdClaim, out var parsedId) ? parsedId : null;

            var finalCommand = command with { CreatedBy = createdBy };

            var result = await _mediator.Send(finalCommand);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Updates the status of an order item to the next step in the workflow.
        /// </summary>
        /// <remarks>
        /// Status must advance sequentially: Pending(0)→Confirmed(1)→Cooking(2)→Ready(3)→Delivering(4)→Served(5).
        /// Cancelled(6) is allowed from any non-terminal state.
        ///
        /// Special assignment rules:
        /// - Confirmed→Cooking (1→2): AssigneeId is required (chef to be assigned).
        /// - Ready→Delivering (3→4): AssigneeId is automatically set to AccountId (the person performing this action).
        /// - All transitions create an audit entry in OrderItemLog.
        /// </remarks>
        /// <param name="command">The update status request.</param>
        /// <returns>Result of the operation.</returns>
        /// <response code="200">Status updated successfully.</response>
        /// <response code="400">Invalid transition, item not found, or missing AssigneeId for chef assignment.</response>
        [HttpPut("Update-Status")]
        public async Task<IActionResult> UpdateOrderItemStatus([FromBody] UpdateStatusOrderItemRequest request)
        {
            var command = new UpdateStatusOrderItemCommand(
                OrderItemIds: request.OrderItemIds,
                NewStatus: request.NewStatus,
                AccountId: request.AccountId,
                ChangeSource: request.ChangeSource,
                AssigneeId: request.AssigneeId
            );

            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Updates info fields of an order item with status-based restrictions.
        /// </summary>
        /// <remarks>
        /// - ChefAccountId: can only be updated when status is Cooking (2).
        /// - WaiterAccountId: can only be updated when status is Delivering (4).
        /// - Note: can only be updated when status is Pending (0) or Confirmed (1).
        /// Pass null for any field to skip updating it.
        /// </remarks>
        /// <param name="orderItemId">The ID of the order item to update.</param>
        /// <param name="request">Fields to update.</param>
        /// <returns>Result of the operation.</returns>
        /// <response code="200">OrderItem info updated successfully.</response>
        /// <response code="400">Validation failed, item not found, or status does not allow the update.</response>
        [HttpPut("{orderItemId}/Update-Info")]
        public async Task<IActionResult> UpdateOrderItemInfo([FromRoute] Guid orderItemId, [FromBody] UpdateInfoOrderItemRequest request)
        {
            var command = new UpdateInfoOrderItemCommand(
                OrderItemId: orderItemId,
                ChefAccountId: request.ChefAccountId,
                WaiterAccountId: request.WaiterAccountId,
                Note: request.Note
            );

            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Retrieves a list of order items filtered by a specific status.
        /// </summary>
        /// <param name="status">The order item status (e.g. 3 for Ready).</param>
        /// <returns>A list of order items with full details including their relevant TableId and TableNumber.</returns>
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetOrderItemsByStatus([FromRoute] int status)
        {
            var query = new GetOrderItemsByStatusQuery(status);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("confirmed")]
        public async Task<IActionResult> GetConfirmedOrderItems([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetConfirmedOrderItemsQuery(pageIndex, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("assign-chef")]
        public async Task<IActionResult> AssignChefToOrderItem([FromBody] AssignChefToOrderItemRequest request)
        {
            var command = new AssignChefToOrderItemCommand(request.OrderItemId, request.AccountId);
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result.Errors);
        }
    }
}
