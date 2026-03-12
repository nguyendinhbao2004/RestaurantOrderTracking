using AutoMapper;
using MediatR;
using RestaurantOrderTracking.Application.Dto.Bill;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.Bill.Queries.GetAll
{
    public class GetAllBillsHandler : IRequestHandler<GetAllBillsQuery, PagedResult<BillResponse>>
    {
        private readonly IBillRepository _billRepository;
        private readonly IMapper _mapper;

        public GetAllBillsHandler(IBillRepository billRepository, IMapper mapper)
        {
            _billRepository = billRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<BillResponse>> Handle(GetAllBillsQuery request, CancellationToken cancellationToken)
        {
            var (bills, totalCount) = await _billRepository.GetPagedBillsAsync(request.Keyword, request.PageIndex, request.PageSize);
            var billResponses = _mapper.Map<List<BillResponse>>(bills);
            return new PagedResult<BillResponse>(billResponses, request.PageIndex, request.PageSize, totalCount, "Get Bills Successfully");
        }
    }
}
