using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Customers.Query.GetCustomerByAccountId
{
    public class GetCustomerByAccountIdHandler : IRequestHandler<GetCustomerByAccountIdQuery, Result<CustomerDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerByAccountIdHandler(IAccountRepository accountRepository, ICustomerRepository customerRepository)
        {
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
        }

        public async Task<Result<CustomerDto>> Handle(GetCustomerByAccountIdQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId);
            if (account == null)
            {
                return Result<CustomerDto>.Failure("Account not found.");
            }

            if (string.IsNullOrEmpty(account.Phone))
            {
               return Result<CustomerDto>.Failure("Account does not have an associated phone number.");
            }

            var customer = await _customerRepository.GetByPhoneAsync(account.Phone);
            if (customer == null)
            {
                return Result<CustomerDto>.Failure("Customer not found for this account.");
            }

            var customerDto = new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                Address = customer.Address
            };

            return Result<CustomerDto>.Success(customerDto);
        }
    }
}
