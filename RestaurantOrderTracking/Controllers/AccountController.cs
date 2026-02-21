using Application.Feature.Account.Queries.GetAllAccount;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
namespace RestaurantOrderTracking.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all accounts with pagination and filtering by keyword.
        /// </summary>
        /// <remarks>
        /// This endpoint retrieves a paginated list of accounts. You can filter the accounts by providing a keyword that matches the account's username or other relevant fields.
        /// <br/>
        /// **Sample Request**: Get accounts with keyword (Name or UserName), page index 1, and page size 10.
        /// </remarks>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>
        /// Returns a paginated list of accounts and the total count of matching accounts.
        /// </returns>
        /// <response code="200">Returns the list of accounts and total count.</response>
        /// <response code="400">If the request parameters are invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet]
        public async Task<IActionResult> GetAllAccount(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var query = new GetAllAccountQueries(keyword, pageIndex, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}