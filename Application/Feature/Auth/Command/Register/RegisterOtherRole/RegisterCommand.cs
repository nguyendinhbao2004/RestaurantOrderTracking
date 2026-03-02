using MediatR;
using RestaurantOrderTracking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Feature.Auth.Command.Register.RegisterOtherRole
{
    public class RegisterCommand : IRequest<Result<string>>
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public int RoleId { get; set; }
    }
}
