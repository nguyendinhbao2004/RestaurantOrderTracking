using AutoMapper;
using MediatR;
using RestaurantOrderTracking.Application.Dto.Bill;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.Bill.Queries.GetById
{
    public class GetBillByIdHandler : IRequestHandler<GetBillByIdQuery, Result<BillDetailResponse>>
    {
        private readonly IBillRepository _billRepository;
        private readonly IMapper _mapper;

        public GetBillByIdHandler(IBillRepository billRepository, IMapper mapper)
        {
            _billRepository = billRepository;
            _mapper = mapper;
        }

        public async Task<Result<BillDetailResponse>> Handle(GetBillByIdQuery request, CancellationToken cancellationToken)
        {
            var bill = await _billRepository.GetByIdWithDetailsAsync(request.BillId);
            if (bill == null)
                return Result<BillDetailResponse>.Failure("Bill not found.");

            var response = _mapper.Map<BillDetailResponse>(bill);
            return Result<BillDetailResponse>.Success("Get Bill Detail Successfully", response);
        }
    }
}
