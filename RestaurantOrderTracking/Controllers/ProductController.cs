using Application.Feature.Product.Commands.Create;
using Application.Feature.Product.Queries.GetAllProduct;
using Application.Feature.Products.Commands.Update.UpdateInfo;
using Application.Feature.Products.Commands.Update.UpdateStatus;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace RestaurantOrderTracking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets a paginated list of products with optional keyword filtering.
        /// </summary>
        /// <remarks>
        /// This endpoint returns a paginated list of products. The results can be filtered by keyword.
        /// <br/>
        /// **Sample Request**: Login with valid credentials
        /// </remarks>
        /// <param name="keyword">The keyword to filter products by name.</param>
        /// <param name="pageIndex">The page index of the results.</param>
        /// <param name="pageSize">The number of products per page.</param>
        /// <returns>
        /// Returns a tuple containing the list of products and the total count.
        /// </returns>
        /// <response code="200">Returns the list of products and total count.</response>
        /// <response code="400">If the request parameters are invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet]
        public async Task<IActionResult> GetAllProducts(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var query = new GetAllProductQueries(keyword, pageIndex, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="command">The command containing product details.</param>
        /// <returns>The result of the product creation operation.</returns>
        /// <response code="200">If the product is created successfully.</response>
        /// <response code="400">If the product creation fails due to validation errors.</response>
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            if(result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Updates the information of an existing product.
        /// </summary>
        /// <param name="command">The command containing updated product information.</param>
        /// <returns>The result of the product update operation.</returns>
        /// <response code="200">If the product is updated successfully.</response>
        /// <response code="400">If the product update fails due to validation errors or if the product is not found.</response>
        [HttpPut("Update-Info")]
        public async Task<IActionResult> UpdateProduct([FromBody]UpdateInfoProductCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result.Errors);
        }

        [HttpPut("Update-Status/{id}")]
        public async Task<IActionResult> UpdateProductStatus(Guid id)
        {
            var command = new UpdateStatusProductCommand(id);
            var result = await _mediator.Send(command);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result.Errors);
        }
    }
}