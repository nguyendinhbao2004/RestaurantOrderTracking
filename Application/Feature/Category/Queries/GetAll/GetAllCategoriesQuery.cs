using MediatR;
using RestaurantOrderTracking.Application.Dto.Category;
using RestaurantOrderTracking.Domain.Common;
using System.Collections.Generic;

namespace RestaurantOrderTracking.Application.Feature.Category.Queries.GetAll
{
    public record GetAllCategoriesQuery() : IRequest<Result<List<CategoryResponse>>>;
}
