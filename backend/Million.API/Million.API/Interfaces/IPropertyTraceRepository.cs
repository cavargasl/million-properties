using Million.API.Domain;

namespace Million.API.Repository
{
    public interface IPropertyTraceRepository : IRepository<PropertyTrace>
    {
        /// <summary>
        /// Get all traces for a specific property
        /// </summary>
        Task<IEnumerable<PropertyTrace>> GetByPropertyIdAsync(string propertyId);

        /// <summary>
        /// Get all traces for a specific property with pagination
        /// </summary>
        Task<(IEnumerable<PropertyTrace> Items, long TotalCount)> GetByPropertyIdPaginatedAsync(
            string propertyId,
            int pageNumber,
            int pageSize);

        /// <summary>
        /// Get traces by date range
        /// </summary>
        Task<IEnumerable<PropertyTrace>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get traces by date range with pagination
        /// </summary>
        Task<(IEnumerable<PropertyTrace> Items, long TotalCount)> GetByDateRangePaginatedAsync(
            DateTime startDate,
            DateTime endDate,
            int pageNumber,
            int pageSize);

        /// <summary>
        /// Get traces by value range
        /// </summary>
        Task<IEnumerable<PropertyTrace>> GetByValueRangeAsync(decimal minValue, decimal maxValue);

        /// <summary>
        /// Get traces by value range with pagination
        /// </summary>
        Task<(IEnumerable<PropertyTrace> Items, long TotalCount)> GetByValueRangePaginatedAsync(
            decimal minValue,
            decimal maxValue,
            int pageNumber,
            int pageSize);

        /// <summary>
        /// Delete all traces for a specific property
        /// </summary>
        Task DeleteAllForPropertyAsync(string propertyId);
    }
}
