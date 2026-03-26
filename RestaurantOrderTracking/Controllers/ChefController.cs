using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderTracking.Application.Feature.Chef.Queries.GetAvailableChefs;

namespace RestaurantOrderTracking.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChefController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChefController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableChefs()
        {
            var query = new GetAvailableChefsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}