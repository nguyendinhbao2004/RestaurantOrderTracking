using System;
using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Customers.Command.DeleteCustomer
{
    public class DeleteCustomerCommand : IRequest<Result<string>>
    {
        public Guid CustomerId { get; set; }
    }
}
