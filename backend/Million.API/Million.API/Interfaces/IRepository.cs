namespace Million.API.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(string id);
        Task CreateAsync(T entity);
        Task UpdateAsync(string id, T entity);
        Task DeleteAsync(string id);
        
        /// <summary>
        /// Get paginated results
        /// </summary>
        /// <param name="pageNumber">Page number (starting from 1)</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>Paginated items</returns>
        Task<(IEnumerable<T> Items, long TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize);
    }
}
