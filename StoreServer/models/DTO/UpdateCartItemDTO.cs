namespace StoreServer.models.DTO
{
    public class UpdateCartItemDTO
    {
        public int IdCart { get; set; }

        public int IdProduct { get; set; }

        public int Quantity { get; set; }
    }
}
