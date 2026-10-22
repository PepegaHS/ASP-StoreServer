namespace StoreServer.models.DTO
{
    public class UserSummaryDTO
    {
        public int IdUser { get; set; }
        public string Login { get; set; } = null!;
        public int IdRole { get; set; }
    }
}
