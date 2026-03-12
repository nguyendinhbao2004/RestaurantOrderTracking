using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using OrderEntity = RestaurantOrderTracking.Domain.Entities.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Feature.Order.Commands.Create
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateOrderHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // ===============================
            // 1️⃣ Validate: TableId là bắt buộc (DineIn / TakeAway)
            // ===============================

            if (request.TableId == null)
                return Result<Guid>.Failure("TableId is required for DineIn or TakeAway order.");

            var hasActiveOrder = await _orderRepository
                .TableHasActiveOrder(request.TableId.Value);

            if (hasActiveOrder)
                return Result<Guid>.Failure("This table already has an active order.");

            var order = new OrderEntity(
                tableId: request.TableId.Value,
                orderType: request.OrderType,
                waiterId: request.AccountId
            );

            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success("Order created successfully", order.Id);
        }
    }
}
