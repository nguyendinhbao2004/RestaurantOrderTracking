using MediatR;
using RestaurantOrderTracking.Application.Dto.Account;

namespace RestaurantOrderTracking.Application.Feature.Chef.Queries.GetAvailableChefs
{
    public record GetAvailableChefsQuery() : IRequest<List<AvailableChefResponse>>;
}