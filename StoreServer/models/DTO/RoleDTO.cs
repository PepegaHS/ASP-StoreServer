namespace StoreServer.models.DTO
{
    public class RoleDTO
    {
        public int IdRole { get; set; }
        public string RoleName { get; set; } = null!;
        public List<UserSummaryDTO> Users { get; set; } = new();
    }
}
