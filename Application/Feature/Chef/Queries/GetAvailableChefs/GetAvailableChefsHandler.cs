using MediatR;
using RestaurantOrderTracking.Application.Dto.Account;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.Chef.Queries.GetAvailableChefs
{
    public class GetAvailableChefsHandler : IRequestHandler<GetAvailableChefsQuery, List<AvailableChefResponse>>
    {
        private readonly IChefRepository _chefRepository;

        public GetAvailableChefsHandler(IChefRepository chefRepository)
        {
            _chefRepository = chefRepository;
        }

        public async Task<List<AvailableChefResponse>> Handle(GetAvailableChefsQuery request, CancellationToken cancellationToken)
        {
            var chefs = await _chefRepository.GetAvailableChefsAsync();

            return chefs.Select(chef => new AvailableChefResponse
            {
                AccountId = chef.AccountId,
                FullName = chef.Account.FullName,
                Specialty = ((int)chef.Specialty).ToString(),
                SkillLevel = chef.SkillLevel,
                IsAvailable = chef.IsAvailable
            }).ToList();
        }
    }
}