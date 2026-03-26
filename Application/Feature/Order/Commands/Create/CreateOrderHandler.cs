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
        private readonly ITableRepository _tableRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateOrderHandler(IOrderRepository orderRepository, ITableRepository tableRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _tableRepository = tableRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            if (request.OrderType == OrderType.Delivery)
                return Result<Guid>.Failure("Delivery order must be created via /api/Order/online.");

            // ===============================
            // 1️⃣ Validate: TableId là bắt buộc (DineIn / TakeAway)
            // ===============================

            if (request.TableId == null)
                return Result<Guid>.Failure("TableId is required for DineIn or TakeAway order.");

            var table = await _tableRepository.GetByIdAsync(request.TableId);
            if (table == null)
                return Result<Guid>.Failure($"Table with ID {request.TableId.Value} not found.");

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

            table.SetReserved();
            _tableRepository.Update(table, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success("Order created successfully", order.Id);
        }
    }
}
