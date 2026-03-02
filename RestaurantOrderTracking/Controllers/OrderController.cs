using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderTracking.Application.Feature.Order.Commands.Create;
using RestaurantOrderTracking.Application.Feature.Order.Commands.Update.UpdateInfo;
using RestaurantOrderTracking.Application.Feature.Order.Commands.Update.UpdateStatus;

namespace RestaurantOrderTracking.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Retrieves a paginated list of orders, optionally filtered by a keyword.
        /// </summary>
        /// <remarks>This method uses the mediator pattern to handle the query and retrieve the data. The
        /// result includes the specified page of orders and any relevant metadata, such as total count, if supported by
        /// the underlying query.</remarks>
        /// <param name="keyword">An optional search term to filter orders. If null or empty, all orders are retrieved.</param>
        /// <param name="pageIndex">The page number to retrieve. Must be 1 or greater. Defaults to 1.</param>
        /// <param name="pageSize">The number of orders to include per page. Must be 1 or greater. Defaults to 10.</param>
        /// <returns>An <see cref="IActionResult"/> containing a paginated list of orders. The result is returned as an HTTP 200
        /// OK response.</returns>
        /// <response=code="200">Returns a paginated list of orders.</response>
        /// <response=code="400">Returns a bad request if the input parameters are invalid.</response>
        /// <response=code="500">Returns an internal server error if an unexpected error occurs.</response>
        [HttpGet]
        public async Task<IActionResult> GetAllOrder(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var query = new Application.Feature.Order.Queries.GetAllOrder.GetAllOrderQueries(keyword, pageIndex, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="command">Order creation request</param>
        /// <returns>Result of order creation</returns>
        /// <response code="200">Order created successfully</response>
        /// <response code="400">Validation failed</response>
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result.Errors);
        }
        /// <summary>
        /// Updates basic information of an order.
        /// </summary>
        /// <param name="command">Updated order information</param>
        /// <returns>Result of update operation</returns>
        /// <response code="200">Order updated successfully</response>
        /// <response code="400">Validation failed</response>
        [HttpPut("Update-Info")]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateInfoOrderCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result.Errors);
        }
        /// <summary>
        /// Updates status of an order.
        /// </summary>
        /// <param name="command">Status update request</param>
        /// <returns>Result of status update</returns>
        [HttpPut("Update-Status")]
        public async Task<IActionResult> UpdateOrderStatus(
            [FromBody] UpdateStatusOrderCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result.Errors);
        }
    }
}
