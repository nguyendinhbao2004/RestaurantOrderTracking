using Domain.Enums;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Auth.Command.Register.RegisterHeadChef
{
    public class RegisterHeadChefHandler : IRequestHandler<RegisterHeadChefCommand, Result<string>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IGenericRepository<Chef> _chefRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterHeadChefHandler( IAccountRepository accountRepository, IGenericRepository<Chef> chefRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _chefRepository = chefRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(RegisterHeadChefCommand request, CancellationToken cancellationToken)
        {
            var existingAccount = await _accountRepository.GetByUserNameAsync(request.UserName);
            if (existingAccount != null)
            {
                return Result<string>.Failure("Username already exists.");
            }

            var hashedPassword = _accountRepository.HashPassword(request.Password);
            var account = new RestaurantOrderTracking.Domain.Entities.Account(
                roleId: 7,
                userName: request.UserName,
                fullName: request.FullName,
                phone: request.Phone,
                passwordHash: hashedPassword,
                image: request.Img
            );

            await _accountRepository.AddAsync(account);

            var chef = new Chef(
                accountId: account.Id,
                specialty: ExpertiseChef.HeadChef,
                skillLevel: request.SkillLevel
            );

            await _chefRepository.AddAsync(chef);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("HeadChef registered successfully.", account.Id.ToString());
        }
    }
}
