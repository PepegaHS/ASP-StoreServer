namespace StoreServer.models.DTO
{
    public class CartDTO
    {
        public int IdCart { get; set; }

        public int IdUser { get; set; }

        public DateTime CreatedAt { get; set; }
        public UserSummaryDTO User { get; set; } = null!;
        public List<CartItemDTO> Items { get; set; } = new();
    }
}
