using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderTracking.Application.Feature.Order.Commands.Create;
using RestaurantOrderTracking.Application.Feature.Order.Commands.CreateOnline;
using RestaurantOrderTracking.Application.Feature.Order.Commands.Update.UpdateInfo;
using RestaurantOrderTracking.Application.Feature.Order.Commands.Update.UpdateStatus;
using RestaurantOrderTracking.Application.Feature.Order.Queries.GetOrderById;

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
        /// Retrieves full details of a single order by its ID,
        /// including all order items with product name, chef name and waiter name.
        /// </summary>
        /// <param name="id">The order ID.</param>
        /// <returns>Full order detail including all order items.</returns>
        /// <response code="200">Returns the full order detail.</response>
        /// <response code="404">Order not found.</response>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid id)
        {
            var query = new GetOrderByIdQuery(id);
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
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

        /// <summary>
        /// Tạo đơn hàng online từ khách hàng đặt tại nhà.
        /// Tự động tạo Customer, Order (TableId=null, OrderType=Delivery, Status=Pending) và các OrderItem.
        /// </summary>
        /// <param name="command">Thông tin khách hàng và danh sách sản phẩm</param>
        /// <returns>Id của Order vừa được tạo</returns>
        /// <response code="200">Tạo đơn hàng online thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        [HttpPost("online")]
        public async Task<IActionResult> CreateOnlineOrder([FromBody] CreateOnlineOrderCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result.Errors);
        }
    }
}
