using Million.API.Domain;

namespace Million.API.Repository
{
    public interface IPropertyRepository : IRepository<Property>
    {
        Task<IEnumerable<Property>> SearchPropertiesAsync(
            string? name = null,
            string? address = null,
            decimal? minPrice = null,
            decimal? maxPrice = null);

        Task<(IEnumerable<Property> Items, long TotalCount)> SearchPropertiesPaginatedAsync(
            string? name = null,
            string? address = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int pageNumber = 1,
            int pageSize = 10);

        Task<(IEnumerable<Property> Items, long TotalCount)> GetByOwnerIdPaginatedAsync(
            string ownerId,
            int pageNumber,
            int pageSize);

        Task<IEnumerable<Property>> GetByOwnerIdAsync(string ownerId);

        Task<IEnumerable<Property>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        Task<bool> ExistsByCodeAsync(string codeInternal);

        Task<IEnumerable<Property>> GetByYearAsync(int year);
    }
}
