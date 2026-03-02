using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Feature.Order.Commands.Update.UpdateStatus
{
    public class UpdateStatusOrderHandler : IRequestHandler<UpdateStatusOrderCommand, Result<Guid>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateStatusOrderHandler(
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(UpdateStatusOrderCommand request, CancellationToken cancellationToken)
        {

            var order = await _orderRepository.GetByIdAsync(request.Id);

            if (order == null)
                return Result<Guid>.Failure("Order not found.");

            try
            {

                order.UpdateStatus(request.NewStatus);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success("Update Order Status Successfully", order.Id);
        }
    }
}
