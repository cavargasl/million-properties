namespace Million.API.DTOs
{
    /// <summary>
    /// DTO for filtering properties by name, address and price range
    /// </summary>
    public class PropertyFilterDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
