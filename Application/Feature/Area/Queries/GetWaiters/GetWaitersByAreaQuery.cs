using MediatR;
using RestaurantOrderTracking.Application.Dto.Area;
using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;

namespace RestaurantOrderTracking.Application.Feature.Area.Queries.GetWaiters
{
    public record GetWaitersByAreaQuery(Guid AreaId) : IRequest<Result<List<AreaWaiterResponse>>>;
}
