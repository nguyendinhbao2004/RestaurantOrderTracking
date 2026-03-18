using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Feature.Order.Commands.Update.UpdateInfo
{
    public class UpdateInfoOrderHandler : IRequestHandler<UpdateInfoOrderCommand, Result<Guid>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateInfoOrderHandler(
            IOrderRepository orderRepository,
            ITableRepository tableRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(UpdateInfoOrderCommand request, CancellationToken cancellationToken)
        {

            var order = await _orderRepository.GetByIdAsync(request.Id);

            if (order == null)
                return Result<Guid>.Failure("Order not found.");


            if (request.OrderType == OrderType.Delivery)
                return Result<Guid>.Failure("Order type can only be DineIn or TakeAway.");

            if (request.TableId == Guid.Empty)
                return Result<Guid>.Failure("TableId is invalid.");

            var table = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken);
            if (table == null)
                return Result<Guid>.Failure("Table not found.");

            var hasActiveOrder = await _orderRepository
                .TableHasActiveOrder(request.TableId);

            if (hasActiveOrder && order.TableId != request.TableId)
                return Result<Guid>.Failure("This table already has an active order.");

            try
            {
                order.UpdateInfo(request.TableId, request.OrderType);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success("Update Order Info Successfully", order.Id);
        }
    }
}
