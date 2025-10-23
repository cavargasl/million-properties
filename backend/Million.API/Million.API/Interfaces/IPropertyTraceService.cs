using Million.API.DTOs;

namespace Million.API.Services
{
    public interface IPropertyTraceService
    {
        Task<IEnumerable<PropertyTraceDto>> GetTracesByPropertyIdAsync(string propertyId);
        Task<PaginatedResponseDto<PropertyTraceDto>> GetTracesByPropertyIdPaginatedAsync(string propertyId, PaginationRequestDto paginationDto);
        Task<PropertyTraceDto?> GetTraceByIdAsync(string id);
        Task<PropertyTraceDto> CreateTraceAsync(CreatePropertyTraceDto createDto);
        Task<PropertyTraceDto?> UpdateTraceAsync(string id, UpdatePropertyTraceDto updateDto);
        Task<bool> DeleteTraceAsync(string id);
        Task<IEnumerable<PropertyTraceDto>> GetTracesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<PaginatedResponseDto<PropertyTraceDto>> GetTracesByDateRangePaginatedAsync(DateTime startDate, DateTime endDate, PaginationRequestDto paginationDto);
        Task<IEnumerable<PropertyTraceDto>> GetTracesByValueRangeAsync(decimal minValue, decimal maxValue);
        Task<PaginatedResponseDto<PropertyTraceDto>> GetTracesByValueRangePaginatedAsync(decimal minValue, decimal maxValue, PaginationRequestDto paginationDto);
    }
}
