namespace StoreServer.models.DTO
{
    public class UserDTO
    {
        public int IdUser { get; set; }

        public string Login { get; set; } = null!;

        public int IdRole { get; set; }


        public RoleSummaryDTO Role { get; set; } = null!;
    }
}
