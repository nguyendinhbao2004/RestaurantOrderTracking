namespace Application.Dto.Role
{
    public class EmployeeByRoleResponse
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int TotalEmployees { get; set; }
        public IReadOnlyList<EmployeeSummaryResponse> Employees { get; set; } = Array.Empty<EmployeeSummaryResponse>();
    }

    public class EmployeeSummaryResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}