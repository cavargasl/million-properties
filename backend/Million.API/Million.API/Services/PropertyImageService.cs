using Million.API.Domain;
using Million.API.DTOs;
using Million.API.Repository;

namespace Million.API.Services
{
    public class PropertyImageService
    {
        private readonly PropertyImageRepository _imageRepository;
        private readonly PropertyRepository _propertyRepository;

        public PropertyImageService(
            PropertyImageRepository imageRepository,
            PropertyRepository propertyRepository)
        {
            _imageRepository = imageRepository;
            _propertyRepository = propertyRepository;
        }

        /// <summary>
        /// Get all images for a property
        /// </summary>
        public async Task<IEnumerable<PropertyImageDto>> GetImagesByPropertyIdAsync(string propertyId)
        {
            var images = await _imageRepository.GetByPropertyIdAsync(propertyId);
            return images.Select(MapToDto);
        }

        /// <summary>
        /// Get image by ID
        /// </summary>
        public async Task<PropertyImageDto?> GetImageByIdAsync(string id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            return image == null ? null : MapToDto(image);
        }

        /// <summary>
        /// Add image to property
        /// </summary>
        public async Task<PropertyImageDto> AddImageAsync(CreatePropertyImageDto createDto)
        {
            // Verificar que la propiedad exista
            var property = await _propertyRepository.GetByIdAsync(createDto.IdProperty);
            if (property == null)
            {
                throw new InvalidOperationException($"Property with ID '{createDto.IdProperty}' not found");
            }

            var image = new PropertyImage
            {
                IdProperty = createDto.IdProperty,
                File = createDto.File,
                Enabled = createDto.Enabled
            };

            await _imageRepository.CreateAsync(image);
            return MapToDto(image);
        }

        /// <summary>
        /// Update image
        /// </summary>
        public async Task<PropertyImageDto?> UpdateImageAsync(string id, UpdatePropertyImageDto updateDto)
        {
            var existingImage = await _imageRepository.GetByIdAsync(id);
            if (existingImage == null) return null;

            existingImage.File = updateDto.File;
            existingImage.Enabled = updateDto.Enabled;

            await _imageRepository.UpdateAsync(id, existingImage);
            return MapToDto(existingImage);
        }

        /// <summary>
        /// Delete image
        /// </summary>
        public async Task<bool> DeleteImageAsync(string id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image == null) return false;

            await _imageRepository.DeleteAsync(id);
            return true;
        }

        /// <summary>
        /// Enable/Disable image
        /// </summary>
        public async Task<bool> ToggleImageAsync(string id, bool enabled)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image == null) return false;

            image.Enabled = enabled;
            await _imageRepository.UpdateAsync(id, image);
            return true;
        }

        private PropertyImageDto MapToDto(PropertyImage image)
        {
            return new PropertyImageDto
            {
                IdPropertyImage = image.IdPropertyImage,
                IdProperty = image.IdProperty,
                File = image.File,
                Enabled = image.Enabled
            };
        }
    }
}
