namespace StoreServer.models.DTO
{
    public class PatchUserDTO
    {
        public string? Login { get; set; }

        public string? Password { get; set; }

        public int? IdRole { get; set; }
    }
}
