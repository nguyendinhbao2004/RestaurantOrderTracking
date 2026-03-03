using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderTracking.Application.Feature.Bill.Commands.Cancel;
using RestaurantOrderTracking.Application.Feature.Bill.Commands.Create;
using RestaurantOrderTracking.Application.Feature.Bill.Commands.Pay;
using RestaurantOrderTracking.Application.Feature.Bill.Commands.Update;
using RestaurantOrderTracking.Application.Feature.Bill.Queries.GetAll;
using RestaurantOrderTracking.Application.Feature.Bill.Queries.GetById;
using RestaurantOrderTracking.Application.Feature.Table.Commands.GenerateQRSession;
using RestaurantOrderTracking.Application.Feature.Table.Commands.RefreshQRSession;

namespace RestaurantOrderTracking.WebApi.Controllers
{
    /// <summary>
    /// Controller for Cashier role - manages bills and QR sessions.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CashierController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CashierController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Bill Management

        /// <summary>
        /// Gets all bills with optional filtering and pagination.
        /// </summary>
        /// <param name="keyword">Optional keyword to filter bills by table number, cashier name, or status.</param>
        /// <param name="pageIndex">The page number to retrieve (1-based). Defaults to 1.</param>
        /// <param name="pageSize">The number of items per page. Defaults to 10.</param>
        /// <returns>A paginated list of bills.</returns>
        /// <response code="200">Returns the paginated list of bills.</response>
        [HttpGet("bill")]
        public async Task<IActionResult> GetAllBills(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var query = new GetAllBillsQuery(keyword, pageIndex, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Gets detailed information of a specific bill.
        /// </summary>
        /// <param name="id">The unique identifier of the bill.</param>
        /// <returns>Detailed bill information including order items.</returns>
        /// <response code="200">Returns the bill detail.</response>
        /// <response code="404">Bill not found.</response>
        [HttpGet("bill/{id}")]
        public async Task<IActionResult> GetBillById([FromRoute] Guid id)
        {
            var query = new GetBillByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result.Succeeded)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Creates a new bill for an order.
        /// </summary>
        /// <remarks>
        /// The order must be in 'Paying' status. Amount is automatically calculated from order items.
        /// <br/>
        /// **Payment Methods**: cash = 1, credit_card = 2, bank_transfer = 3
        /// </remarks>
        /// <param name="command">Bill creation request containing OrderId, CashierAccountId, PaymentMethod, and optional Discount.</param>
        /// <returns>The ID of the created bill.</returns>
        /// <response code="200">Bill created successfully.</response>
        /// <response code="400">Validation failed (order not found, wrong status, or duplicate bill).</response>
        [HttpPost("bill")]
        public async Task<IActionResult> CreateBill([FromBody] CreateBillCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Updates an existing unpaid bill.
        /// </summary>
        /// <remarks>
        /// Only unpaid bills can be updated. You can change the payment method and/or discount.
        /// </remarks>
        /// <param name="command">Update request containing BillId, optional PaymentMethod, and optional Discount.</param>
        /// <returns>Result of the update operation.</returns>
        /// <response code="200">Bill updated successfully.</response>
        /// <response code="400">Update failed.</response>
        [HttpPut("bill/update")]
        public async Task<IActionResult> UpdateBill([FromBody] UpdateBillCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Marks a bill as paid.
        /// </summary>
        /// <remarks>
        /// This will also:
        /// <br/>- Set the Order status to 'Completed'
        /// <br/>- Set the Table status to 'Available' (for DineIn orders)
        /// </remarks>
        /// <param name="command">Payment request containing BillId and optional TransactionId.</param>
        /// <returns>Result of the payment operation.</returns>
        /// <response code="200">Bill paid successfully.</response>
        /// <response code="400">Payment failed.</response>
        [HttpPut("bill/pay")]
        public async Task<IActionResult> PayBill([FromBody] PayBillCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Cancels an unpaid bill.
        /// </summary>
        /// <remarks>
        /// This will also revert the Order status back to 'Confirmed'.
        /// </remarks>
        /// <param name="command">Cancel request containing BillId.</param>
        /// <returns>Result of the cancellation.</returns>
        /// <response code="200">Bill cancelled successfully.</response>
        /// <response code="400">Cancellation failed.</response>
        [HttpPut("bill/cancel")]
        public async Task<IActionResult> CancelBill([FromBody] CancelBillCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        #endregion

        #region QR Session Management

        /// <summary>
        /// Generates a new QR session for a table.
        /// </summary>
        /// <remarks>
        /// This will revoke all existing active sessions for the table and create a new one.
        /// The session token is also updated on the Table entity.
        /// Default expiration: 8 hours (480 minutes).
        /// </remarks>
        /// <param name="tableId">The ID of the table to generate a QR session for.</param>
        /// <returns>The generated QR session information.</returns>
        /// <response code="200">QR session generated successfully.</response>
        /// <response code="400">Table not found.</response>
        [HttpPost("qr-session/{tableId}")]
        public async Task<IActionResult> GenerateQRSession([FromRoute] Guid tableId)
        {
            var command = new GenerateQRSessionCommand(tableId);
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Refreshes the QR session for a table.
        /// </summary>
        /// <remarks>
        /// If an active session exists, it generates a new token and extends the expiration.
        /// If no active session exists, a new one is created.
        /// </remarks>
        /// <param name="tableId">The ID of the table to refresh the QR session for.</param>
        /// <returns>The refreshed QR session information.</returns>
        /// <response code="200">QR session refreshed successfully.</response>
        /// <response code="400">Table not found.</response>
        [HttpPut("qr-session/{tableId}/refresh")]
        public async Task<IActionResult> RefreshQRSession([FromRoute] Guid tableId)
        {
            var command = new RefreshQRSessionCommand(tableId);
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        #endregion
    }
}
