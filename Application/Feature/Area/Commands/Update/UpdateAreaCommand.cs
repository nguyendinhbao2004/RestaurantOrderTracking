using MediatR;
using RestaurantOrderTracking.Domain.Common;
using System;

namespace RestaurantOrderTracking.Application.Feature.Area.Commands.Update
{
    public record UpdateAreaCommand(
        Guid Id,
        string? Name,
        string? Description
    ) : IRequest<Result>;
}
