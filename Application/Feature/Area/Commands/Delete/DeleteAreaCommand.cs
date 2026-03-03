using MediatR;
using RestaurantOrderTracking.Domain.Common;
using System;

namespace RestaurantOrderTracking.Application.Feature.Area.Commands.Delete
{
    public record DeleteAreaCommand(Guid Id) : IRequest<Result>;
}
