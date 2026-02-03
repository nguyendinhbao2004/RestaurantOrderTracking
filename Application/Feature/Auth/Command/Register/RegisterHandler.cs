using MediatR;
using Microsoft.AspNetCore.Identity;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Feature.Auth.Command.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, Result<string>>
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IUnitOfWork _unitOfWork;
        public RegisterHandler(IAccountRepository accountRepo, IUnitOfWork unitOfWork)
        {
           _accountRepo = accountRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _accountRepo.GetByUserNameAsync(request.UserName);
            if (existingUser != null)
            {
                return Result<string>.Failure("User is already exist.");
            }
            var hassedPassword = _accountRepo.HashPassword(request.Password);
            var account = new Account(request.RoleId, request.UserName, request.FullName, request.Phone, hassedPassword);
            await _accountRepo.AddAsync(account);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("User registered successfully.", account.Id.ToString());
        }
    }
}
