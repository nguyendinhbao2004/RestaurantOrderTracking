namespace RestaurantOrderTracking.Application.Dto.Account
{
    public class AvailableChefResponse
    {
        public Guid AccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string Specialty { get; set; } = null!;
        public string SkillLevel { get; set; } = null!;
        public bool IsAvailable { get; set; }
    }
}