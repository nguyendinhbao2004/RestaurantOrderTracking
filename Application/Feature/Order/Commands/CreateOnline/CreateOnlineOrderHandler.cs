using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Enums;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using Domain.Interface.Repository;
using OrderEntity = RestaurantOrderTracking.Domain.Entities.Order;
using BillEntity = RestaurantOrderTracking.Domain.Entities.Bill;

namespace RestaurantOrderTracking.Application.Feature.Order.Commands.CreateOnline
{
    public class CreateOnlineOrderHandler : IRequestHandler<CreateOnlineOrderCommand, Result<CreateOnlineOrderResponse>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IBillRepository _billRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOnlineOrderHandler(
            ICustomerRepository customerRepository,
            IOrderRepository orderRepository,
            IBillRepository billRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _billRepository = billRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CreateOnlineOrderResponse>> Handle(CreateOnlineOrderCommand request, CancellationToken cancellationToken)
        {
            // Validate thông tin đầu vào
            if (request.Items == null || request.Items.Count == 0)
                return Result<CreateOnlineOrderResponse>.Failure("Items list cannot be empty.");

            if (string.IsNullOrWhiteSpace(request.CustomerName))
                return Result<CreateOnlineOrderResponse>.Failure("Customer name is required.");

            if (string.IsNullOrWhiteSpace(request.CustomerPhone))
                return Result<CreateOnlineOrderResponse>.Failure("Customer phone is required.");

            if (string.IsNullOrWhiteSpace(request.CustomerAddress))
                return Result<CreateOnlineOrderResponse>.Failure("Customer address is required.");

            // Chỉ cho phép cash hoặc bank_transfer
            if (request.PaymentMethod != PaymentMethod.cash && request.PaymentMethod != PaymentMethod.bank_transfer)
                return Result<CreateOnlineOrderResponse>.Failure("PaymentMethod không hợp lệ. Chỉ chấp nhận 'cash' (1) hoặc 'bank_transfer' (3).");

            // Tính TotalAmount từ Products và validate Quantity
            decimal totalAmount = 0;
            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                if (product == null)
                    return Result<CreateOnlineOrderResponse>.Failure($"Product with ID {item.ProductId} not found.");

                if (item.Quantity <= 0)
                    return Result<CreateOnlineOrderResponse>.Failure($"Quantity for product {product.Name} must be greater than 0.");

                totalAmount += product.Price * item.Quantity;
            }

            // Khởi tạo Customer
            var customer = new Customer(
                name: request.CustomerName,
                phone: request.CustomerPhone,
                address: request.CustomerAddress
            );
            await _customerRepository.AddAsync(customer);

            // Khởi tạo Order online
            var order = new OrderEntity(
                orderType: OrderType.Delivery,
                customerId: customer.Id
            );
            await _orderRepository.AddAsync(order);

            // Thêm OrderItems vào Order
            try
            {
                foreach (var item in request.Items)
                {
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        order.AddItem(
                            productId: item.ProductId,
                            accountId: Guid.Empty,
                            note: item.Note ?? string.Empty,
                            orderChannel: "Online"
                        );
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return Result<CreateOnlineOrderResponse>.Failure(ex.Message);
            }

            // Khởi tạo Bill
            var bill = new BillEntity(
                orderId: order.Id,
                accountId: Guid.Parse("019ced1d-876e-7f94-93fb-92dd934ecee2"),
                amount: totalAmount,
                paymentMethod: request.PaymentMethod,
                discount: null
            );
            await _billRepository.AddAsync(bill);

            // LƯU XUỐNG DATABASE
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Trả về kết quả
            var response = new CreateOnlineOrderResponse(
                OrderId: order.Id,
                BillId: bill.Id,
                PaymentMethod: request.PaymentMethod
            );

            return Result<CreateOnlineOrderResponse>.Success("Online order created successfully.", response);
        }
    }
}
