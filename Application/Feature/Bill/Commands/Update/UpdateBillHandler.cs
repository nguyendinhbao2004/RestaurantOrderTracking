using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.Bill.Commands.Update
{
    public class UpdateBillHandler : IRequestHandler<UpdateBillCommand, Result>
    {
        private readonly IBillRepository _billRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBillHandler(IBillRepository billRepository, IUnitOfWork unitOfWork)
        {
            _billRepository = billRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateBillCommand request, CancellationToken cancellationToken)
        {
            var bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            if (bill == null)
                return Result.Failure("Bill not found.");

            try
            {
                bill.Update(request.PaymentMethod, request.Discount);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }

            _billRepository.Update(bill, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success("Bill updated successfully.");
        }
    }
}
