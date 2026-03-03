using MediatR;
using Microsoft.Extensions.Logging;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Auth.Command.Register.RegisterWaiter
{
    public class RegisterWaiterHandler : IRequestHandler<RegisterWaiterCommand, Result<string>>
    {
        private readonly IGenericRepository<Waiter> _waiterRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegisterWaiterHandler> _logger; // Added Logger
        public RegisterWaiterHandler(IGenericRepository<Waiter> waiterRepository, IAccountRepository accountRepository, IUnitOfWork unitOfWork, ILogger<RegisterWaiterHandler> logger)
        {
            _waiterRepository = waiterRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<Result<string>> Handle(RegisterWaiterCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to register a new waiter with username: {UserName}", request.UserName);

            try
            {
                // 1. Check if user already exists
                var account = await _accountRepository.GetByUserNameAsync(request.UserName);
                if (account != null)
                {
                    _logger.LogWarning("Registration failed: Username {UserName} already exists.", request.UserName);
                    return Result<string>.Failure("Username already exists");
                }

                // 2. Process Account Creation
                var hashedPassword = _accountRepository.HashPassword(request.Password);
                var newAccount = new RestaurantOrderTracking.Domain.Entities.Account(
                        roleId: 4,
                        userName: request.UserName,
                        fullName: request.FullName,
                        phone: request.Phone,
                        passwordHash: hashedPassword,
                        image: request.Img
                    );
                
                await _accountRepository.AddAsync(newAccount);
                _logger.LogDebug("Account entity created for {UserName}", request.UserName);

                // 3. Process Waiter Creation
                // Note: If Waiter depends on Account.Id, ensure your repository/DB handles identity generation
                var newWaiter = new Waiter(
                    accountId: newAccount.Id,
                    assignedAreaId: request.AreaId,
                    isAvailable: true,
                    orderCount: 5
                    );
                await _waiterRepository.AddAsync(newWaiter);

                // 4. Atomic Save
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully registered waiter: {UserName} with ID: {Id}", request.UserName, newAccount.Id);
                return Result<string>.Success("Waiter registered successfully");
            }
            catch (Exception ex)
            {
                // Log the full exception stack trace for the developer, but return a generic message to the user
                _logger.LogError(ex, "An unexpected error occurred while registering waiter {UserName}", request.UserName);
                
                return Result<string>.Failure("An internal error occurred. Please try again later.");
            }
        }
    }
}