using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Auth.Command.Register.RegisterCustomer
{
    public class RegisterCustomerHandler : IRequestHandler<RegisterCustomerCommand, Result<string>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IGenericRepository<Customer> _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCustomerHandler(IAccountRepository accountRepository, IGenericRepository<Customer> customerRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<string>> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {
            var existingAccount = await _accountRepository.GetByUserNameAsync(request.UserName);
            if (existingAccount != null)
            {
                return Result<string>.Failure("Username already exists.");
            }
            var hashedPassword = _accountRepository.HashPassword(request.Password);
            var account = new RestaurantOrderTracking.Domain.Entities.Account(
                userName: request.UserName,
                fullName: request.FullName,
                phone: request.Phone,
                passwordHash: hashedPassword,
                roleId: 6,
                image: request.Image
            );
            await _accountRepository.AddAsync(account);
            var customer = new Customer(
                name: request.FullName,
                phone: request.Phone,
                address: request.Address
            );
            await _customerRepository.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            return Result<string>.Success("Customer registered successfully.");
        }
    }
}