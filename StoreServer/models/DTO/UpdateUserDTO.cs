namespace StoreServer.models.DTO
{
    public class UpdateUserDTO
    {
        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;

        public int IdRole { get; set; }
    }
}
