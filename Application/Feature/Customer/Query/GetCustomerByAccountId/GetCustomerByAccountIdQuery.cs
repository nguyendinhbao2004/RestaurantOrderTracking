using System;
using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Customers.Query.GetCustomerByAccountId
{
    public class GetCustomerByAccountIdQuery : IRequest<Result<CustomerDto>>
    {
        public Guid AccountId { get; set; }
    }
}
