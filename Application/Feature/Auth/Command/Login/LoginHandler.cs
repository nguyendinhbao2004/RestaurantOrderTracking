using MediatR;
using Microsoft.AspNetCore.Identity;
using RestaurantOrderTracking.Application.Dto.Auth;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantOrderTracking.Application.Feature.Auth.Command.Login
{
    public class LoginHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IAccountRepository _accountRepo;
        private readonly IUnitOfWork _unitOfWork;
        public LoginHandler(IJwtTokenGenerator jwtTokenGenerator, IAccountRepository accountRepo, IUnitOfWork unitOfWork)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _accountRepo = accountRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _accountRepo.GetByUserNameAsync(request.UserName);
            if (user == null)
            {
                return Result<AuthResponse>.Failure("Invalid username or password.");
            }
            var isPasswordValid = await _accountRepo.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                return Result<AuthResponse>.Failure("Invalid username or password.");
            }

            
            var accessToken = _jwtTokenGenerator.GenerateToken(user, user.Role.Name);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            var jwtId = Guid.NewGuid().ToString();

            user.AddRefreshToken(accessToken, refreshToken);
            _accountRepo.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<AuthResponse>.Success("Login Successfully",new AuthResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Role = user.Role.Name,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}
