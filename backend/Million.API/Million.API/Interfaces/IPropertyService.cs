using Million.API.DTOs;

namespace Million.API.Services
{
    public interface IPropertyService
    {
        Task<IEnumerable<PropertyDto>> SearchPropertiesAsync(PropertyFilterDto filterDto);
        Task<PaginatedResponseDto<PropertyDto>> SearchPropertiesPaginatedAsync(PropertyFilterDto filterDto, PaginationRequestDto paginationDto);
        Task<PropertyDetailDto?> GetPropertyByIdAsync(string id);
        Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createDto);
        Task<bool> UpdatePropertyAsync(string id, UpdatePropertyDto updateDto);
        Task<bool> DeletePropertyAsync(string id);
        Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerAsync(string ownerId);
        Task<PaginatedResponseDto<PropertyDto>> GetPropertiesByOwnerPaginatedAsync(string ownerId, PaginationRequestDto paginationDto);
    }
}
