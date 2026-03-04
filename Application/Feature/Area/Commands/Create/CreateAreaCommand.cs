using MediatR;
using RestaurantOrderTracking.Domain.Common;
using System;

namespace RestaurantOrderTracking.Application.Feature.Area.Commands.Create
{
    public record CreateAreaCommand(
        string Name,
        string? Description
    ) : IRequest<Result<Guid>>;
}
