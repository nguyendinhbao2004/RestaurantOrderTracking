using Application.Dto.Table;
using AutoMapper;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Tables.Queries.GetById
{
    public class GetTableByIdHandler : IRequestHandler<GetTableByIdQueries, Result<TableDetailResponse>>
    {
        private readonly ITableRepository _tableRepository;
        private readonly IMapper _mapper;
        public GetTableByIdHandler(ITableRepository tableRepository, IMapper mapper)
        {
            _tableRepository = tableRepository;
            _mapper = mapper;
        }
        public async Task<Result<TableDetailResponse>> Handle(GetTableByIdQueries request, CancellationToken cancellationToken)
        {
            var table = await _tableRepository.GetByIdAsync(request.Id);
            if (table == null)
            {
                return Result<TableDetailResponse>.Failure($"Table with ID {request.Id} not found.");
            }
            var tableDetailResponse = _mapper.Map<TableDetailResponse>(table);
            return Result<TableDetailResponse>.Success("Get Table Detail Successfully", tableDetailResponse);
        }
    }
}