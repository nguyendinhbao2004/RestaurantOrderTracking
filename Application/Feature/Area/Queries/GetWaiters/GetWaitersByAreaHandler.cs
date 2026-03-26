using AutoMapper;
using MediatR;
using RestaurantOrderTracking.Application.Dto.Area;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Feature.Area.Queries.GetWaiters
{
    public class GetWaitersByAreaHandler : IRequestHandler<GetWaitersByAreaQuery, Result<List<AreaWaiterResponse>>>
    {
        private readonly IWaiterRepository _waiterRepository;
        private readonly IMapper _mapper;

        public GetWaitersByAreaHandler(IWaiterRepository waiterRepository, IMapper mapper)
        {
            _waiterRepository = waiterRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<AreaWaiterResponse>>> Handle(GetWaitersByAreaQuery request, CancellationToken cancellationToken)
        {
            var waiters = await _waiterRepository.GetWaitersByAreaIdAsync(request.AreaId);
            var response = _mapper.Map<List<AreaWaiterResponse>>(waiters);
            return Result<List<AreaWaiterResponse>>.Success("Get waiters by area successfully.", response);
        }
    }
}
