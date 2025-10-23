using Million.API.DTOs;

namespace Million.API.Services
{
    public interface IOwnerService
    {
        Task<IEnumerable<OwnerDto>> GetAllOwnersAsync();
        Task<PaginatedResponseDto<OwnerDto>> GetAllOwnersPaginatedAsync(PaginationRequestDto paginationDto);
        Task<OwnerDto?> GetOwnerByIdAsync(string id);
        Task<OwnerDto> CreateOwnerAsync(CreateOwnerDto createDto);
        Task<OwnerDto?> UpdateOwnerAsync(string id, UpdateOwnerDto updateDto);
        Task<bool> DeleteOwnerAsync(string id);
        Task<IEnumerable<OwnerDto>> SearchByNameAsync(string name);
        Task<PaginatedResponseDto<OwnerDto>> SearchByNamePaginatedAsync(string name, PaginationRequestDto paginationDto);
        Task<IEnumerable<OwnerDto>> SearchByAddressAsync(string address);
        Task<PaginatedResponseDto<OwnerDto>> SearchByAddressPaginatedAsync(string address, PaginationRequestDto paginationDto);
        Task<IEnumerable<OwnerDto>> GetByAgeRangeAsync(int minAge, int maxAge);
        Task<PaginatedResponseDto<OwnerDto>> GetByAgeRangePaginatedAsync(int minAge, int maxAge, PaginationRequestDto paginationDto);
    }
}
