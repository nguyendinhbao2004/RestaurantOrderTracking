namespace Application.Dto.Account
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
    }
}