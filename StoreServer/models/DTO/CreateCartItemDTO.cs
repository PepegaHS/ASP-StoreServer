namespace StoreServer.models.DTO
{
    public class CreateCartItemDTO
    {
        public int IdCart { get; set; }

        public int IdProduct { get; set; }

        public int Quantity { get; set; }
    }
}
