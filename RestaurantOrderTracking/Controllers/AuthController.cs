using Application.Feature.Auth.Command.Register.RegisterWaiter;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RestaurantOrderTracking.Application.Feature.Auth.Command.Login;
using RestaurantOrderTracking.Application.Feature.Auth.Command.Register;
using WebApi.Common;

namespace RestaurantOrderTracking.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Authenticates a user based on the provided login credentials.
        /// </summary>
        /// <remarks>
        /// This a Api endpoint to Login a user in the system.
        /// </remarks>
        /// <param name="command">An object containing the user's login credentials and any additional authentication parameters. Cannot be
        /// null.</param>
        /// <returns>An <see cref="IActionResult"/> that represents the result of the login operation. Returns a 200 OK response
        /// with authentication details if successful; otherwise, returns a 400 Bad Request with error information.</returns>
        /// <response code="200">Login Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [EnableRateLimiting(AppConstants.ConfigConstants.FixedPolicy)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


        /// <summary>
        /// Creates a new user account using the specified registration details.
        /// </summary>
        /// <remarks>
        /// This is an API endpoint to create a new employee account in the system.
        /// </remarks>
        /// <param name="command">The registration information required to create the new account. Must not be null.</param>
        /// <returns>An IActionResult that represents the result of the account creation operation. Returns 200 OK with the
        /// result if successful; otherwise, returns 400 Bad Request with error details.</returns>
        /// <response code="200">Create Successful</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Creates a new waiter account using the specified registration details.
        /// </summary>
        /// <remarks>
        /// This is an API endpoint to create a new waiter account in the system.
        /// <br/>
        /// Locallhost:5015/api/Auth/CreateWaiter
        /// </remarks>  
        /// <param name="command"></param>
        /// <returns>
        /// An IActionResult that represents the result of the waiter account creation operation. Returns 200 OK with the
        /// result if successful; otherwise, returns 400 Bad Request with error details.</returns>
        /// <returns>Message: Waiter Register Successfully</returns>
        /// <response code="200">Waiter Register Successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPost("CreateWaiter")]
        public async Task<IActionResult> CreateWaiter([FromBody] RegisterWaiterCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
