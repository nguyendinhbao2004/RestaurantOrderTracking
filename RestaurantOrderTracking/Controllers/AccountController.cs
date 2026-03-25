using Application.Feature.Account.Queries.GetAllAccount;
using Application.Feature.Roles.Queries.GetEmployeesByRole;
using Application.Dto.Account;
using Application.Dto.Role;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantOrderTracking.Domain.Common;
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
        /// Get all accounts with pagination and optional keyword filter.
        /// </summary>
        /// <remarks>
        /// How to use:
        /// - Call <c>GET /api/Account</c>.
        /// - Optional query params:
        ///   - <c>keyword</c>: filter by account name/username.
        ///   - <c>pageIndex</c>: page number, default is 1.
        ///   - <c>pageSize</c>: number of records per page, default is 10.
        ///
        /// Sample request:
        /// <c>GET /api/Account?keyword=nguyen&amp;pageIndex=1&amp;pageSize=10</c>
        ///
        /// Response body when success (200):
        /// - <c>succeeded</c>: true
        /// - <c>message</c>: operation message
        /// - <c>data</c>: list of accounts in current page
        /// - <c>meta.pagination</c>: pagination metadata (page number, page size, total pages, total records)
        /// </remarks>
        /// <param name="keyword"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>
        /// Returns a paginated list of accounts and the total count of matching accounts.
        /// </returns>
        /// <response code="200">Request processed successfully. Returns paged account data and pagination metadata.</response>
        /// <response code="400">Invalid query string format (for example non-numeric pageIndex/pageSize).</response>
        /// <response code="500">Unexpected server-side error.</response>
        [ProducesResponseType(typeof(PagedResult<AccountResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAllAccount(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var query = new GetAllAccountQueries(keyword, pageIndex, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get employees grouped by role with pagination.
        /// </summary>
        /// <remarks>
        /// How to use:
        /// - Call <c>GET /api/Account/employees/by-role</c>.
        /// - Optional query params:
        ///   - <c>roleId</c>: filter by a specific role id.
        ///   - <c>pageIndex</c>: page number, default is 1.
        ///   - <c>pageSize</c>: number of grouped role records per page, default is 10.
        ///
        /// Sample requests:
        /// <c>GET /api/Account/employees/by-role?pageIndex=1&amp;pageSize=10</c>
        /// <c>GET /api/Account/employees/by-role?roleId=3&amp;pageIndex=1&amp;pageSize=5</c>
        ///
        /// Response body when success (200):
        /// - <c>succeeded</c>: true
        /// - <c>message</c>: operation message
        /// - <c>data</c>: list of grouped roles, each item contains:
        ///   - <c>roleId</c>, <c>roleName</c>, <c>totalEmployees</c>
        ///   - <c>employees</c>: employee list of that role
        /// - <c>meta.pagination</c>: pagination metadata
        ///
        /// Notes:
        /// - If no data is found, API still returns 200 with empty <c>data</c>.
        /// - Negative or zero page values are normalized by handler to safe defaults.
        /// </remarks>
        /// <param name="roleId">Optional role id. If provided, returns only employees of that role.</param>
        /// <param name="pageIndex">Page index for paginated result.</param>
        /// <param name="pageSize">Page size for paginated result.</param>
        /// <returns>Returns paged employees grouped by role.</returns>
        /// <response code="200">Request processed successfully. Returns grouped employee data and pagination metadata.</response>
        /// <response code="400">Invalid query string format (for example non-numeric roleId/pageIndex/pageSize).</response>
        /// <response code="500">Unexpected server-side error.</response>
        [ProducesResponseType(typeof(PagedResult<EmployeeByRoleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [HttpGet("employees/by-role")]
        public async Task<IActionResult> GetEmployeesByRole([FromQuery] int? roleId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetEmployeesByRoleQuery(roleId, pageIndex, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}