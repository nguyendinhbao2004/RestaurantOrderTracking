using MediatR;
using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using RestaurantOrderTracking.Domain.Common;
using System;

namespace RestaurantOrderTracking.Application.Feature.Payment.Queries.GetPaymentInfoByOrderId
{
    public record GetPaymentInfoByOrderIdQuery(Guid OrderId) : IRequest<Result<PaymentInfoByOrderIdResponse>>;
}
