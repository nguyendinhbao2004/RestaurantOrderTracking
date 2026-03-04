using Domain.Enums;
using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Auth.Command.Register.RegisterChef
{
    public class RegisterChefCommand : IRequest<Result<string>>
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string? Img { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public ExpertiseChef Specialty { get; set; }
        public string SkillLevel { get; set; }
        public string Station { get; set; }
    }
}