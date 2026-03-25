using Application.Dto.Role;
using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace Application.Feature.Roles.Queries.GetEmployeesByRole
{
    public class GetEmployeesByRoleHandler : IRequestHandler<GetEmployeesByRoleQuery, PagedResult<EmployeeByRoleResponse>>
    {
        private readonly IAccountRepository _accountRepository;

        public GetEmployeesByRoleHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<PagedResult<EmployeeByRoleResponse>> Handle(GetEmployeesByRoleQuery request, CancellationToken cancellationToken)
        {
            var pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var accounts = await _accountRepository.GetAccountsByRoleAsync(request.RoleId, cancellationToken);

            var groupedEmployees = accounts
                .GroupBy(a => new { a.RoleId, RoleName = a.Role.Name })
                .OrderBy(g => g.Key.RoleId)
                .Select(g => new EmployeeByRoleResponse
                {
                    RoleId = g.Key.RoleId,
                    RoleName = g.Key.RoleName,
                    TotalEmployees = g.Count(),
                    Employees = g.Select(a => new EmployeeSummaryResponse
                    {
                        Id = a.Id,
                        FullName = a.FullName,
                        UserName = a.UserName,
                        IsActive = a.IsActive
                    }).ToList()
                })
                .ToList();

            var totalCount = groupedEmployees.Count;
            var pagedData = groupedEmployees
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<EmployeeByRoleResponse>(pagedData, pageIndex, pageSize, totalCount, "Get employees by role successfully");
        }
    }
}