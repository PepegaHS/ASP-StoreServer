namespace StoreServer.models.DTO
{
    public class CreateUserDTO
    {
        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;

        public int IdRole { get; set; }
    }
}
