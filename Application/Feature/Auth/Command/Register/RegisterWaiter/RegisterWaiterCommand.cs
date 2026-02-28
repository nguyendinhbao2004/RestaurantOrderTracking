using System.Runtime.Intrinsics.X86;
using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Auth.Command.Register.RegisterWaiter
{
    public class RegisterWaiterCommand : IRequest<Result<string>>
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string? Img { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public Guid AreaId { get; set; }
    } 
}