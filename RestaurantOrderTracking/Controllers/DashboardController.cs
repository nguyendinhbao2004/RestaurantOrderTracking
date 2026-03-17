using Application.Feature.Dashboard.Queries.GetDashboardSummary;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Dashboard Summary Metrics
        /// </summary>
        /// <remarks>
        /// This endpoint retrieves the key metrics for the restaurant dashboard, including total orders, total revenue, average order value, and pending orders.
        /// </remarks>
        /// <returns>Returns the dashboard summary data.</returns>
        /// <response code="200">Returns the dashboard summary successfully.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var query = new GetDashboardSummaryQueries();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
