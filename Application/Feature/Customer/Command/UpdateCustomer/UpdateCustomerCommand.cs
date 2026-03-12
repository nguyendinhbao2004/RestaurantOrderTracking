using System;
using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Customers.Command.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest<Result<string>>
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
