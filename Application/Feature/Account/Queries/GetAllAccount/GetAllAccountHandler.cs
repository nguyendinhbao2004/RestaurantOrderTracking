using Application.Dto.Account;
using AutoMapper;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Account.Queries.GetAllAccount
{
    public class GetAllAccountHandler : IRequestHandler<GetAllAccountQueries, PagedResult<AccountResponse>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        public GetAllAccountHandler(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }
        
        public async Task<PagedResult<AccountResponse>> Handle(GetAllAccountQueries request, CancellationToken cancellationToken)
        {
            var (accounts, totalCount) = await _accountRepository.GetPageAccountAsync(request.Keyword, request.PageIndex, request.PageSize);
            var accountResponses = _mapper.Map<IEnumerable<AccountResponse>>(accounts).ToList();
            return new PagedResult<AccountResponse>(accountResponses, totalCount, request.PageIndex, request.PageSize, "Get Account Successfully");
        }
    }
}