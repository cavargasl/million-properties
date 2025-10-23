using Million.API.DTOs;

namespace Million.API.Services
{
    public interface IPropertyImageService
    {
        Task<IEnumerable<PropertyImageDto>> GetImagesByPropertyIdAsync(string propertyId);
        Task<PaginatedResponseDto<PropertyImageDto>> GetImagesByPropertyIdPaginatedAsync(string propertyId, PaginationRequestDto paginationDto);
        Task<PropertyImageDto?> GetImageByIdAsync(string id);
        Task<PropertyImageDto> AddImageAsync(CreatePropertyImageDto createDto);
        Task<IEnumerable<PropertyImageDto>> AddImagesBulkAsync(string propertyId, List<CreatePropertyImageDto> imageDtos);
        Task<PropertyImageDto?> UpdateImageAsync(string id, UpdatePropertyImageDto updateDto);
        Task<bool> DeleteImageAsync(string id);
        Task<bool> ToggleImageAsync(string id, bool enabled);
    }
}
