namespace StoreServer.models.DTO
{
    public class CartItemDTO
    {
        public int IdCartItem { get; set; }

        public int IdCart { get; set; }

        public int IdProduct { get; set; }

        public int Quantity { get; set; }
        public CartSummaryDTO Cart { get; set; } = null!;
        public ProductDTO Product { get; set; } = null!;
    }
}
