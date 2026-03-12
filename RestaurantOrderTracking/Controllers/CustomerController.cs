using Application.Feature.Customers.Command.DeleteCustomer;
using Application.Feature.Customers.Command.UpdateCustomer;
using Application.Feature.Customers.Query.GetCustomerByAccountId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RestaurantOrderTracking.Controllers
{
    /// <summary>
    /// Controller for Customer APIs - manages customer data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a customer's detailed information by their associated Account ID.
        /// </summary>
        /// <param name="accountId">The unique identifier of the user's account.</param>
        /// <returns>Customer information including Id, Name, Phone, and Address.</returns>
        /// <response code="200">Returns the customer details.</response>
        /// <response code="404">Account or Customer not found.</response>
        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetCustomerByAccountId(Guid accountId)
        {
            var query = new GetCustomerByAccountIdQuery { AccountId = accountId };
            var result = await _mediator.Send(query);
            return result.Succeeded ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Updates an existing customer's basic information.
        /// </summary>
        /// <param name="id">The unique identifier of the customer to update.</param>
        /// <param name="command">Command containing updated Name, Phone, and Address data.</param>
        /// <returns>Result of the update operation.</returns>
        /// <response code="200">Customer updated successfully.</response>
        /// <response code="400">Validation failed or Customer ID mismatch.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerCommand command)
        {
            if (id != command.CustomerId)
            {
                return BadRequest("Customer Id in path doesn't match Id in body.");
            }

            var result = await _mediator.Send(command);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Soft deletes a customer from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the customer to delete.</param>
        /// <returns>Result of the soft delete operation.</returns>
        /// <response code="200">Customer deleted successfully.</response>
        /// <response code="400">Deletion failed (e.g., Customer not found).</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var command = new DeleteCustomerCommand { CustomerId = id };
            var result = await _mediator.Send(command);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
