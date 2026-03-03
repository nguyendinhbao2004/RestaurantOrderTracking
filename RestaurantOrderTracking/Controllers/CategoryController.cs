using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderTracking.Application.Feature.Category.Commands.Create;
using RestaurantOrderTracking.Application.Feature.Category.Commands.Delete;
using RestaurantOrderTracking.Application.Feature.Category.Commands.Update;
using RestaurantOrderTracking.Application.Feature.Category.Queries.GetAll;

namespace RestaurantOrderTracking.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing product categories.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets all categories.
        /// </summary>
        /// <returns>List of all categories.</returns>
        /// <response code="200">Returns all categories.</response>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var query = new GetAllCategoriesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="command">Category creation request.</param>
        /// <returns>The ID of the created category.</returns>
        /// <response code="200">Category created successfully.</response>
        /// <response code="400">Validation failed.</response>
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="command">Update request with optional fields.</param>
        /// <returns>Result of the update operation.</returns>
        /// <response code="200">Category updated successfully.</response>
        /// <response code="400">Update failed.</response>
        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Deletes a category by ID.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>Result of the delete operation.</returns>
        /// <response code="200">Category deleted successfully.</response>
        /// <response code="400">Delete failed (category has products).</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var command = new DeleteCategoryCommand(id);
            var result = await _mediator.Send(command);

            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
