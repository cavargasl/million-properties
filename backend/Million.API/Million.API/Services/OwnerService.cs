using Million.API.Domain;
using Million.API.DTOs;
using Million.API.Repository;

namespace Million.API.Services
{
    /// <summary>
    /// Service layer for Owner business logic and DTO mapping
    /// </summary>
    public class OwnerService
    {
        private readonly OwnerRepository _ownerRepository;

        public OwnerService(OwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }

        /// <summary>
        /// Get all owners
        /// </summary>
        public async Task<IEnumerable<OwnerDto>> GetAllOwnersAsync()
        {
            var owners = await _ownerRepository.GetAllAsync();
            return owners.Select(MapToDto);
        }

        /// <summary>
        /// Get owner by ID
        /// </summary>
        public async Task<OwnerDto?> GetOwnerByIdAsync(string id)
        {
            var owner = await _ownerRepository.GetByIdAsync(id);
            return owner == null ? null : MapToDto(owner);
        }

        /// <summary>
        /// Create new owner
        /// </summary>
        public async Task<OwnerDto> CreateOwnerAsync(CreateOwnerDto createDto)
        {
            // Validar si ya existe un owner con el mismo nombre
            if (await _ownerRepository.ExistsByNameAsync(createDto.Name))
            {
                throw new InvalidOperationException($"Owner with name '{createDto.Name}' already exists");
            }

            // DTO ? Entity
            var owner = new Owner
            {
                Name = createDto.Name,
                Address = createDto.Address,
                Photo = createDto.Photo,
                Birthday = createDto.Birthday
            };

            await _ownerRepository.CreateAsync(owner);

            // Entity ? DTO
            return MapToDto(owner);
        }

        /// <summary>
        /// Update existing owner
        /// </summary>
        public async Task<OwnerDto?> UpdateOwnerAsync(string id, UpdateOwnerDto updateDto)
        {

            var existingOwner = await _ownerRepository.GetByIdAsync(id);
            if (existingOwner == null) return null;

            // DTO ? Entity (actualizar propiedades)
            existingOwner.Name = updateDto.Name;
            existingOwner.Address = updateDto.Address;
            existingOwner.Photo = updateDto.Photo;
            existingOwner.Birthday = updateDto.Birthday;

            await _ownerRepository.UpdateAsync(id, existingOwner);

            return MapToDto(existingOwner);
        }

        /// <summary>
        /// Delete owner
        /// </summary>
        public async Task<bool> DeleteOwnerAsync(string id)
        {
            var owner = await _ownerRepository.GetByIdAsync(id);
            if (owner == null) return false;

            await _ownerRepository.DeleteAsync(id);
            return true;
        }

        /// <summary>
        /// Search owners by name
        /// </summary>
        public async Task<IEnumerable<OwnerDto>> SearchByNameAsync(string name)
        {
            var owners = await _ownerRepository.FindByNameAsync(name);
            return owners.Select(MapToDto);
        }

        /// <summary>
        /// Search owners by address
        /// </summary>
        public async Task<IEnumerable<OwnerDto>> SearchByAddressAsync(string address)
        {
            var owners = await _ownerRepository.FindByAddressAsync(address);
            return owners.Select(MapToDto);
        }

        /// <summary>
        /// Get owners by age range
        /// </summary>
        public async Task<IEnumerable<OwnerDto>> GetByAgeRangeAsync(int minAge, int maxAge)
        {
            var owners = await _ownerRepository.FindByAgeRangeAsync(minAge, maxAge);
            return owners.Select(MapToDto);
        }

        /// <summary>
        /// Map Entity to DTO
        /// </summary>
        private OwnerDto MapToDto(Owner owner)
        {
            return new OwnerDto
            {
                IdOwner = owner.IdOwner,
                Name = owner.Name,
                Address = owner.Address,
                Photo = owner.Photo,
                Birthday = owner.Birthday
            };
        }
    }
}
