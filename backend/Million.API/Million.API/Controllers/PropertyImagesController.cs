using Microsoft.AspNetCore.Mvc;
using Million.API.DTOs;
using Million.API.Services;

namespace Million.API.Controllers
{
    [ApiController]
    [Route("api/properties/{propertyId}/images")]
    [Produces("application/json")]
    public class PropertyImagesController : ControllerBase
    {
        private readonly PropertyImageService _imageService;
        private readonly ILogger<PropertyImagesController> _logger;

        public PropertyImagesController(PropertyImageService imageService, ILogger<PropertyImagesController> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        /// <summary>
        /// Get all images for a property
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <returns>List of images for the property</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PropertyImageDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PropertyImageDto>>> GetByPropertyId(string propertyId)
        {
            try
            {
                var images = await _imageService.GetImagesByPropertyIdAsync(propertyId);
                return Ok(images);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting images for property {PropertyId}", propertyId);
                return StatusCode(500, "An error occurred while retrieving images");
            }
        }

        /// <summary>
        /// Add image to property
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="createDto">Image data</param>
        /// <returns>Created image</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PropertyImageDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PropertyImageDto>> AddImage(
            string propertyId, 
            [FromBody] CreatePropertyImageDto createDto)
        {
            // Validar ModelState (Data Annotations)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Asegurar que el propertyId de la ruta coincida con el del DTO
                if (createDto.IdProperty != propertyId)
                {
                    createDto = new CreatePropertyImageDto
                    {
                        IdProperty = propertyId,
                        File = createDto.File,
                        Enabled = createDto.Enabled
                    };
                }

                var image = await _imageService.AddImageAsync(createDto);
                return CreatedAtAction(
                    nameof(GetById), 
                    new { propertyId, id = image.IdPropertyImage }, 
                    image);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error adding image to property {PropertyId}", propertyId);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding image to property {PropertyId}", propertyId);
                return StatusCode(500, new { error = "An error occurred while adding the image" });
            }
        }

        /// <summary>
        /// Get image by ID
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="id">Image ID</param>
        /// <returns>Image details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PropertyImageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PropertyImageDto>> GetById(string propertyId, string id)
        {
            try
            {
                var image = await _imageService.GetImageByIdAsync(id);
                
                if (image == null || image.IdProperty != propertyId)
                {
                    return NotFound($"Image with ID '{id}' not found for property '{propertyId}'");
                }

                return Ok(image);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting image {ImageId} for property {PropertyId}", id, propertyId);
                return StatusCode(500, "An error occurred while retrieving the image");
            }
        }

        /// <summary>
        /// Update image
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="id">Image ID</param>
        /// <param name="updateDto">Image update data</param>
        /// <returns>Updated image</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PropertyImageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PropertyImageDto>> Update(
            string propertyId, 
            string id, 
            [FromBody] UpdatePropertyImageDto updateDto)
        {
            // Validar ModelState (Data Annotations)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var image = await _imageService.UpdateImageAsync(id, updateDto);
                
                if (image == null || image.IdProperty != propertyId)
                {
                    return NotFound($"Image with ID '{id}' not found for property '{propertyId}'");
                }

                return Ok(image);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating image {ImageId} for property {PropertyId}", id, propertyId);
                return StatusCode(500, new { error = "An error occurred while updating the image" });
            }
        }

        /// <summary>
        /// Delete image
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="id">Image ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string propertyId, string id)
        {
            try
            {
                // Verificar que la imagen pertenezca a la propiedad
                var image = await _imageService.GetImageByIdAsync(id);
                if (image == null || image.IdProperty != propertyId)
                {
                    return NotFound($"Image with ID '{id}' not found for property '{propertyId}'");
                }

                await _imageService.DeleteImageAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image {ImageId} for property {PropertyId}", id, propertyId);
                return StatusCode(500, new { error = "An error occurred while deleting the image" });
            }
        }

        /// <summary>
        /// Enable or disable an image
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="id">Image ID</param>
        /// <param name="enabled">Enable or disable the image</param>
        /// <returns>No content</returns>
        [HttpPatch("{id}/toggle")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Toggle(string propertyId, string id, [FromQuery] bool enabled)
        {
            try
            {
                var image = await _imageService.GetImageByIdAsync(id);
                if (image == null || image.IdProperty != propertyId)
                {
                    return NotFound($"Image with ID '{id}' not found for property '{propertyId}'");
                }

                await _imageService.ToggleImageAsync(id, enabled);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling image {ImageId} for property {PropertyId}", id, propertyId);
                return StatusCode(500, new { error = "An error occurred while toggling the image" });
            }
        }
    }
}
