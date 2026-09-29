namespace StoreServer.models.DTO
{
    public class CreateCategoryDTO
    {
        public string CategoryName { get; set; } = null!;

        public string? CategoryDescription { get; set; }
    }
}
