using MediatR;
using RestaurantOrderTracking.Application.Dto.Area;
using RestaurantOrderTracking.Domain.Common;
using System.Collections.Generic;

namespace RestaurantOrderTracking.Application.Feature.Area.Queries.GetAll
{
    public record GetAllAreasQuery() : IRequest<Result<List<AreaResponse>>>;
}
