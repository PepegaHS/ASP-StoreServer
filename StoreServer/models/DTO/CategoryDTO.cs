namespace StoreServer.models.DTO
{
    public class CategoryDTO
    {
        public int IdCategory { get; set; }
        
        public string CategoryName { get; set; } = null!;

        public string? CategoryDescription { get; set; }
    }
}
