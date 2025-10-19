using Microsoft.AspNetCore.Mvc;
using Million.API.DTOs;
using Million.API.Services;

namespace Million.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PropertiesController : ControllerBase
    {
        private readonly PropertyService _propertyService;
        private readonly ILogger<PropertiesController> _logger;

        public PropertiesController(PropertyService propertyService, ILogger<PropertiesController> logger)
        {
            _propertyService = propertyService;
            _logger = logger;
        }

        /// <summary>
        /// Search properties with filters (name, address, price range)
        /// </summary>
        /// <param name="name">Property name (partial match, case-insensitive)</param>
        /// <param name="address">Property address (partial match, case-insensitive)</param>
        /// <param name="minPrice">Minimum price</param>
        /// <param name="maxPrice">Maximum price</param>
        /// <returns>List of properties matching the filters. Each property includes: IdOwner, Name, Address, Price, and one image</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<PropertyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> Search(
            [FromQuery] string? name,
            [FromQuery] string? address,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            try
            {
                var filterDto = new PropertyFilterDto
                {
                    Name = name,
                    Address = address,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };

                var properties = await _propertyService.SearchPropertiesAsync(filterDto);
                return Ok(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching properties");
                return StatusCode(500, "An error occurred while searching properties");
            }
        }

        /// <summary>
        /// Search properties with filters and pagination
        /// </summary>
        /// <param name="name">Property name (partial match, case-insensitive)</param>
        /// <param name="address">Property address (partial match, case-insensitive)</param>
        /// <param name="minPrice">Minimum price</param>
        /// <param name="maxPrice">Maximum price</param>
        /// <param name="pageNumber">Page number (starting from 1, default: 1)</param>
        /// <param name="pageSize">Items per page (max 100, default: 10)</param>
        /// <returns>Paginated list of properties matching the filters</returns>
        [HttpGet("search/paginated")]
        [ProducesResponseType(typeof(PaginatedResponseDto<PropertyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<PropertyDto>>> SearchPaginated(
            [FromQuery] string? name,
            [FromQuery] string? address,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filterDto = new PropertyFilterDto
                {
                    Name = name,
                    Address = address,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };

                var paginationDto = new PaginationRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _propertyService.SearchPropertiesPaginatedAsync(filterDto, paginationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching properties with pagination");
                return StatusCode(500, "An error occurred while searching properties");
            }
        }

        /// <summary>
        /// Get all properties
        /// Returns: IdOwner, Name, Address, Price, and one image for each property
        /// </summary>
        /// <returns>List of all properties</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PropertyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetAll()
        {
            try
            {
                // Usar el filtro vacío para obtener todas las propiedades
                var filterDto = new PropertyFilterDto();
                var properties = await _propertyService.SearchPropertiesAsync(filterDto);
                return Ok(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all properties");
                return StatusCode(500, "An error occurred while retrieving properties");
            }
        }

        /// <summary>
        /// Get property by ID with full details
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <returns>Property details including all images</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PropertyDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PropertyDetailDto>> GetById(string id)
        {
            try
            {
                var property = await _propertyService.GetPropertyByIdAsync(id);
                
                if (property == null)
                {
                    return NotFound($"Property with ID '{id}' not found");
                }

                return Ok(property);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting property with ID {PropertyId}", id);
                return StatusCode(500, "An error occurred while retrieving the property");
            }
        }

        /// <summary>
        /// Create a new property
        /// </summary>
        /// <param name="createDto">Property creation data</param>
        /// <returns>Created property</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<PropertyDto>> Create([FromBody] CreatePropertyDto createDto)
        {
            // Validar ModelState (Data Annotations)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var property = await _propertyService.CreatePropertyAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = property.IdProperty }, property);
            }
            catch (ArgumentException ex)
            {
                // Errores de validación de negocio
                _logger.LogWarning(ex, "Validation error creating property");
                return UnprocessableEntity(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Errores como duplicados
                _logger.LogWarning(ex, "Business rule violation creating property");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating property");
                return StatusCode(500, new { error = "An error occurred while creating the property" });
            }
        }

        /// <summary>
        /// Update an existing property
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <param name="updateDto">Property update data</param>
        /// <returns>No content</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePropertyDto updateDto)
        {
            // Validar ModelState (Data Annotations)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _propertyService.UpdatePropertyAsync(id, updateDto);
                
                if (!result)
                {
                    return NotFound($"Property with ID '{id}' not found");
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error updating property with ID {PropertyId}", id);
                return UnprocessableEntity(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating property with ID {PropertyId}", id);
                return StatusCode(500, new { error = "An error occurred while updating the property" });
            }
        }

        /// <summary>
        /// Delete a property
        /// </summary>
        /// <param name="id">Property ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _propertyService.DeletePropertyAsync(id);
                
                if (!result)
                {
                    return NotFound($"Property with ID '{id}' not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting property with ID {PropertyId}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the property" });
            }
        }

        /// <summary>
        /// Get all properties for a specific owner
        /// </summary>
        /// <param name="ownerId">Owner ID</param>
        /// <returns>List of properties owned by the specified owner</returns>
        [HttpGet("by-owner/{ownerId}")]
        [ProducesResponseType(typeof(IEnumerable<PropertyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetByOwner(string ownerId)
        {
            try
            {
                var properties = await _propertyService.GetPropertiesByOwnerAsync(ownerId);
                return Ok(properties);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting properties for owner {OwnerId}", ownerId);
                return StatusCode(500, new { error = "An error occurred while retrieving properties" });
            }
        }

        /// <summary>
        /// Get all properties for a specific owner with pagination
        /// </summary>
        /// <param name="ownerId">Owner ID</param>
        /// <param name="pageNumber">Page number (starting from 1, default: 1)</param>
        /// <param name="pageSize">Items per page (max 100, default: 10)</param>
        /// <returns>Paginated list of properties owned by the specified owner</returns>
        [HttpGet("by-owner/{ownerId}/paginated")]
        [ProducesResponseType(typeof(PaginatedResponseDto<PropertyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<PropertyDto>>> GetByOwnerPaginated(
            string ownerId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginationDto = new PaginationRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _propertyService.GetPropertiesByOwnerPaginatedAsync(ownerId, paginationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting properties for owner {OwnerId} with pagination", ownerId);
                return StatusCode(500, new { error = "An error occurred while retrieving properties" });
            }
        }
    }
}
