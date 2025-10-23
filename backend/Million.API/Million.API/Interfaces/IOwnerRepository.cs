using Million.API.Domain;

namespace Million.API.Repository
{
    public interface IOwnerRepository : IRepository<Owner>
    {
        Task<IEnumerable<Owner>> FindByNameAsync(string name);
        Task<(IEnumerable<Owner> Items, long TotalCount)> FindByNamePaginatedAsync(string name, int pageNumber, int pageSize);
        Task<IEnumerable<Owner>> FindByAddressAsync(string address);
        Task<(IEnumerable<Owner> Items, long TotalCount)> FindByAddressPaginatedAsync(string address, int pageNumber, int pageSize);
        Task<IEnumerable<Owner>> FindByAgeRangeAsync(int minAge, int maxAge);
        Task<(IEnumerable<Owner> Items, long TotalCount)> FindByAgeRangePaginatedAsync(int minAge, int maxAge, int pageNumber, int pageSize);
        Task<bool> ExistsByNameAsync(string name);
    }
}
