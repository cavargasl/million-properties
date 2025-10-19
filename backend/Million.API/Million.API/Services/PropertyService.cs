using Million.API.Domain;
using Million.API.DTOs;
using Million.API.Repository;

namespace Million.API.Services
{
    public class PropertyService
    {
        private readonly PropertyRepository _propertyRepository;
        private readonly PropertyImageRepository _imageRepository;

        public PropertyService(
            PropertyRepository propertyRepository,
            PropertyImageRepository imageRepository)
        {
            _propertyRepository = propertyRepository;
            _imageRepository = imageRepository;
        }

        /// <summary>
        /// Search properties using filters from DTO
        /// </summary>
        public async Task<IEnumerable<PropertyDto>> SearchPropertiesAsync(PropertyFilterDto filterDto)
        {
            // ✅ DTO → Repository (con parámetros primitivos)
            var properties = await _propertyRepository.SearchPropertiesAsync(
                name: filterDto.Name,
                address: filterDto.Address,
                minPrice: filterDto.MinPrice,
                maxPrice: filterDto.MaxPrice
            );

            // ✅ Entity → DTO con una sola imagen
            var propertyDtos = new List<PropertyDto>();
            foreach (var property in properties)
            {
                // Obtener solo la primera imagen habilitada
                var images = await _imageRepository.GetByPropertyIdAsync(property.IdProperty);
                var firstImage = images.FirstOrDefault(img => img.Enabled)?.File;

                propertyDtos.Add(new PropertyDto
                {
                    IdProperty = property.IdProperty,
                    IdOwner = property.IdOwner,
                    Name = property.Name,
                    Address = property.Address,
                    Price = property.Price,
                    Image = firstImage // ✅ Solo una imagen como requiere el DTO
                });
            }

            return propertyDtos;
        }

        /// <summary>
        /// Get property by ID with full details
        /// </summary>
        public async Task<PropertyDetailDto?> GetPropertyByIdAsync(string id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null) return null;

            // Obtener todas las imágenes para la vista de detalle
            var images = await _imageRepository.GetByPropertyIdAsync(id);
            var imageDtos = images.Select(img => new PropertyImageDto
            {
                IdPropertyImage = img.IdPropertyImage,
                IdProperty = img.IdProperty,
                File = img.File,
                Enabled = img.Enabled
            }).ToList();

            return new PropertyDetailDto
            {
                IdProperty = property.IdProperty,
                Name = property.Name,
                Address = property.Address,
                Price = property.Price,
                CodeInternal = property.CodeInternal,
                Year = property.Year,
                IdOwner = property.IdOwner,
                Images = imageDtos
            };
        }

        /// <summary>
        /// Create new property
        /// </summary>
        public async Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createDto)
        {
            // Validar que el código interno no exista
            if (await _propertyRepository.ExistsByCodeAsync(createDto.CodeInternal))
            {
                throw new InvalidOperationException($"Property with code '{createDto.CodeInternal}' already exists");
            }

            // ✅ DTO → Entity
            var property = new Property
            {
                Name = createDto.Name,
                Address = createDto.Address,
                Price = createDto.Price,
                CodeInternal = createDto.CodeInternal,
                Year = createDto.Year,
                IdOwner = createDto.IdOwner
            };

            await _propertyRepository.CreateAsync(property);

            // ✅ Entity → DTO
            return new PropertyDto
            {
                IdProperty = property.IdProperty,
                IdOwner = property.IdOwner,
                Name = property.Name,
                Address = property.Address,
                Price = property.Price,
                Image = null // Nueva propiedad sin imagen
            };
        }

        /// <summary>
        /// Update existing property
        /// </summary>
        public async Task<bool> UpdatePropertyAsync(string id, UpdatePropertyDto updateDto)
        {
            var existingProperty = await _propertyRepository.GetByIdAsync(id);
            if (existingProperty == null) return false;

            // ✅ DTO → Entity (actualizar propiedades)
            existingProperty.Name = updateDto.Name;
            existingProperty.Address = updateDto.Address;
            existingProperty.Price = updateDto.Price;
            existingProperty.CodeInternal = updateDto.CodeInternal;
            existingProperty.Year = updateDto.Year;
            existingProperty.IdOwner = updateDto.IdOwner;

            await _propertyRepository.UpdateAsync(id, existingProperty);
            return true;
        }

        /// <summary>
        /// Delete property
        /// </summary>
        public async Task<bool> DeletePropertyAsync(string id)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null) return false;

            await _propertyRepository.DeleteAsync(id);
            return true;
        }

        /// <summary>
        /// Get properties by owner
        /// </summary>
        public async Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerAsync(string ownerId)
        {
            var properties = await _propertyRepository.GetByOwnerIdAsync(ownerId);

            var propertyDtos = new List<PropertyDto>();
            foreach (var property in properties)
            {
                var images = await _imageRepository.GetByPropertyIdAsync(property.IdProperty);
                var firstImage = images.FirstOrDefault(img => img.Enabled)?.File;

                propertyDtos.Add(new PropertyDto
                {
                    IdProperty = property.IdProperty,
                    IdOwner = property.IdOwner,
                    Name = property.Name,
                    Address = property.Address,
                    Price = property.Price,
                    Image = firstImage
                });
            }

            return propertyDtos;
        }
    }
}
