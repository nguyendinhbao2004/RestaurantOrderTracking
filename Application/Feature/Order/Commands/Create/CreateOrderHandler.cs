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
            // 1️⃣ Validate theo OrderType
            // ===============================

            if (request.OrderType == OrderType.Delivery)
            {
                if (request.CustomerId == null)
                    return Result<Guid>.Failure("CustomerId is required for Delivery order.");

                if (request.TableId != null)
                    return Result<Guid>.Failure("Delivery order cannot have TableId.");
            }
            else
            {
                if (request.TableId == null)
                    return Result<Guid>.Failure("TableId is required for DineIn or TakeAway order.");

                if (request.CustomerId != null)
                    return Result<Guid>.Failure("DineIn/TakeAway order cannot have CustomerId.");
            }



            if (request.TableId.HasValue)
            {
                var hasActiveOrder = await _orderRepository
                    .TableHasActiveOrder(request.TableId.Value);

                if (hasActiveOrder)
                    return Result<Guid>.Failure("This table already has an active order.");
            }


            var order = new OrderEntity(
                tableId: request.TableId ?? Guid.Empty,  // nếu delivery thì tạm để Guid.Empty
                orderType: request.OrderType,
                waiterId: request.AccountId
            );

            if (request.OrderType == OrderType.Delivery)
            {
                typeof(OrderEntity)
                    .GetProperty("TableId")?
                    .SetValue(order, null);
            }

            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success("Order created successfully", order.Id);
        }
    }
}
