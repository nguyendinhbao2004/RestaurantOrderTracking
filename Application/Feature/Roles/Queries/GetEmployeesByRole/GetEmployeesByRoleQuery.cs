using Application.Dto.Role;
using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace Application.Feature.Roles.Queries.GetEmployeesByRole
{
    public record GetEmployeesByRoleQuery(int? RoleId, int PageIndex, int PageSize) : IRequest<PagedResult<EmployeeByRoleResponse>>;
}