using Million.API.Domain;

namespace Million.API.Repository
{
    public interface IPropertyImageRepository : IRepository<PropertyImage>
    {
        Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(string propertyId);

        Task<(IEnumerable<PropertyImage> Items, long TotalCount)> GetByPropertyIdPaginatedAsync(
            string propertyId,
            int pageNumber,
            int pageSize);

        Task<IEnumerable<PropertyImage>> GetEnabledByPropertyIdAsync(string propertyId);

        Task DisableAllForPropertyAsync(string propertyId);

        Task DeleteAllForPropertyAsync(string propertyId);
    }
}
