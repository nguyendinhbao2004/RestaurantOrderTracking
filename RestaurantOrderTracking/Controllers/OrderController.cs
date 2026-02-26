using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
