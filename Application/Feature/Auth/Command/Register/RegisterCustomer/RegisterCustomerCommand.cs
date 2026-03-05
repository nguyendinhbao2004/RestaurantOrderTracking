using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Auth.Command.Register.RegisterCustomer
{
    public class RegisterCustomerCommand : IRequest<Result<string>>
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }

    }
}