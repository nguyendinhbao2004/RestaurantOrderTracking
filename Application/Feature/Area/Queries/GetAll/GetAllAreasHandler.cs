using AutoMapper;
using MediatR;
using RestaurantOrderTracking.Application.Dto.Area;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;
using AreaEntity = RestaurantOrderTracking.Domain.Entities.Area;

namespace RestaurantOrderTracking.Application.Feature.Area.Queries.GetAll
{
    public class GetAllAreasHandler : IRequestHandler<GetAllAreasQuery, Result<List<AreaResponse>>>
    {
        private readonly IGenericRepository<AreaEntity> _areaRepository;
        private readonly IMapper _mapper;

        public GetAllAreasHandler(IGenericRepository<AreaEntity> areaRepository, IMapper mapper)
        {
            _areaRepository = areaRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<AreaResponse>>> Handle(GetAllAreasQuery request, CancellationToken cancellationToken)
        {
            var areas = await _areaRepository.GetAllAsync();
            var response = _mapper.Map<List<AreaResponse>>(areas);
            return Result<List<AreaResponse>>.Success("Get all areas successfully.", response);
        }
    }
}
