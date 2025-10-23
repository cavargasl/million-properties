using Microsoft.AspNetCore.Mvc;
using Million.API.DTOs;
using Million.API.Services;

namespace Million.API.Controllers
{
    /// <summary>
    /// Controller for Owner operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OwnersController : ControllerBase
    {
        private readonly IOwnerService _ownerService;
        private readonly ILogger<OwnersController> _logger;

        public OwnersController(IOwnerService ownerService, ILogger<OwnersController> logger)
        {
            _ownerService = ownerService;
            _logger = logger;
        }

        /// <summary>
        /// Get all owners
        /// </summary>
        /// <returns>List of all owners</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OwnerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OwnerDto>>> GetAll()
        {
            try
            {
                var owners = await _ownerService.GetAllOwnersAsync();
                return Ok(owners);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all owners");
                return StatusCode(500, "An error occurred while retrieving owners");
            }
        }

        /// <summary>
        /// Get all owners with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (starting from 1, default: 1)</param>
        /// <param name="pageSize">Items per page (max 100, default: 10)</param>
        /// <returns>Paginated list of all owners</returns>
        [HttpGet("paginated")]
        [ProducesResponseType(typeof(PaginatedResponseDto<OwnerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<OwnerDto>>> GetAllPaginated(
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

                var result = await _ownerService.GetAllOwnersPaginatedAsync(paginationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all owners with pagination");
                return StatusCode(500, "An error occurred while retrieving owners");
            }
        }

        /// <summary>
        /// Get owner by ID
        /// </summary>
        /// <param name="id">Owner ID</param>
        /// <returns>Owner details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OwnerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OwnerDto>> GetById(string id)
        {
            try
            {
                var owner = await _ownerService.GetOwnerByIdAsync(id);
                
                if (owner == null)
                {
                    return NotFound($"Owner with ID '{id}' not found");
                }

                return Ok(owner);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting owner with ID {OwnerId}", id);
                return StatusCode(500, "An error occurred while retrieving the owner");
            }
        }

        /// <summary>
        /// Create a new owner
        /// </summary>
        /// <param name="createDto">Owner creation data</param>
        /// <returns>Created owner</returns>
        [HttpPost]
        [ProducesResponseType(typeof(OwnerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<OwnerDto>> Create([FromBody] CreateOwnerDto createDto)
        {
            // Validar ModelState (Data Annotations)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var owner = await _ownerService.CreateOwnerAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = owner.IdOwner }, owner);
            }
            catch (ArgumentException ex)
            {
                // Errores de validaci�n de negocio
                _logger.LogWarning(ex, "Validation error creating owner");
                return UnprocessableEntity(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Errores como duplicados
                _logger.LogWarning(ex, "Business rule violation creating owner");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating owner");
                return StatusCode(500, new { error = "An error occurred while creating the owner" });
            }
        }

        /// <summary>
        /// Update an existing owner
        /// </summary>
        /// <param name="id">Owner ID</param>
        /// <param name="updateDto">Owner update data</param>
        /// <returns>Updated owner</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(OwnerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<OwnerDto>> Update(string id, [FromBody] UpdateOwnerDto updateDto)
        {
            // Validar ModelState (Data Annotations)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var owner = await _ownerService.UpdateOwnerAsync(id, updateDto);
                
                if (owner == null)
                {
                    return NotFound($"Owner with ID '{id}' not found");
                }

                return Ok(owner);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error updating owner with ID {OwnerId}", id);
                return UnprocessableEntity(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating owner with ID {OwnerId}", id);
                return StatusCode(500, new { error = "An error occurred while updating the owner" });
            }
        }

        /// <summary>
        /// Delete an owner
        /// </summary>
        /// <param name="id">Owner ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _ownerService.DeleteOwnerAsync(id);
                
                if (!result)
                {
                    return NotFound($"Owner with ID '{id}' not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting owner with ID {OwnerId}", id);
                return StatusCode(500, new { error = "An error occurred while deleting the owner" });
            }
        }

        /// <summary>
        /// Search owners by name
        /// </summary>
        /// <param name="name">Name to search for (partial match, case-insensitive)</param>
        /// <returns>List of matching owners</returns>
        [HttpGet("search/by-name")]
        [ProducesResponseType(typeof(IEnumerable<OwnerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OwnerDto>>> SearchByName([FromQuery] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(new { error = "Name parameter is required" });
                }

                var owners = await _ownerService.SearchByNameAsync(name);
                return Ok(owners);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching owners by name");
                return StatusCode(500, new { error = "An error occurred while searching owners" });
            }
        }

        /// <summary>
        /// Search owners by name with pagination
        /// </summary>
        /// <param name="name">Name to search for (partial match, case-insensitive)</param>
        /// <param name="pageNumber">Page number (starting from 1, default: 1)</param>
        /// <param name="pageSize">Items per page (max 100, default: 10)</param>
        /// <returns>Paginated list of matching owners</returns>
        [HttpGet("search/by-name/paginated")]
        [ProducesResponseType(typeof(PaginatedResponseDto<OwnerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<OwnerDto>>> SearchByNamePaginated(
            [FromQuery] string name,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(new { error = "Name parameter is required" });
                }

                var paginationDto = new PaginationRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _ownerService.SearchByNamePaginatedAsync(name, paginationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching owners by name with pagination");
                return StatusCode(500, new { error = "An error occurred while searching owners" });
            }
        }

        /// <summary>
        /// Search owners by address
        /// </summary>
        /// <param name="address">Address to search for (partial match, case-insensitive)</param>
        /// <returns>List of matching owners</returns>
        [HttpGet("search/by-address")]
        [ProducesResponseType(typeof(IEnumerable<OwnerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OwnerDto>>> SearchByAddress([FromQuery] string address)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    return BadRequest(new { error = "Address parameter is required" });
                }

                var owners = await _ownerService.SearchByAddressAsync(address);
                return Ok(owners);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching owners by address");
                return StatusCode(500, new { error = "An error occurred while searching owners" });
            }
        }

        /// <summary>
        /// Search owners by address with pagination
        /// </summary>
        /// <param name="address">Address to search for (partial match, case-insensitive)</param>
        /// <param name="pageNumber">Page number (starting from 1, default: 1)</param>
        /// <param name="pageSize">Items per page (max 100, default: 10)</param>
        /// <returns>Paginated list of matching owners</returns>
        [HttpGet("search/by-address/paginated")]
        [ProducesResponseType(typeof(PaginatedResponseDto<OwnerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<OwnerDto>>> SearchByAddressPaginated(
            [FromQuery] string address,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(address))
                {
                    return BadRequest(new { error = "Address parameter is required" });
                }

                var paginationDto = new PaginationRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _ownerService.SearchByAddressPaginatedAsync(address, paginationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching owners by address with pagination");
                return StatusCode(500, new { error = "An error occurred while searching owners" });
            }
        }

        /// <summary>
        /// Get owners by age range
        /// </summary>
        /// <param name="minAge">Minimum age</param>
        /// <param name="maxAge">Maximum age</param>
        /// <returns>List of owners in the age range</returns>
        [HttpGet("search/by-age")]
        [ProducesResponseType(typeof(IEnumerable<OwnerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OwnerDto>>> GetByAgeRange([FromQuery] int minAge, [FromQuery] int maxAge)
        {
            try
            {
                if (minAge < 0 || maxAge < 0 || minAge > maxAge)
                {
                    return BadRequest(new { error = "Invalid age range" });
                }

                var owners = await _ownerService.GetByAgeRangeAsync(minAge, maxAge);
                return Ok(owners);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting owners by age range");
                return StatusCode(500, new { error = "An error occurred while retrieving owners" });
            }
        }

        /// <summary>
        /// Get owners by age range with pagination
        /// </summary>
        /// <param name="minAge">Minimum age</param>
        /// <param name="maxAge">Maximum age</param>
        /// <param name="pageNumber">Page number (starting from 1, default: 1)</param>
        /// <param name="pageSize">Items per page (max 100, default: 10)</param>
        /// <returns>Paginated list of owners in the age range</returns>
        [HttpGet("search/by-age/paginated")]
        [ProducesResponseType(typeof(PaginatedResponseDto<OwnerDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<OwnerDto>>> GetByAgeRangePaginated(
            [FromQuery] int minAge,
            [FromQuery] int maxAge,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (minAge < 0 || maxAge < 0 || minAge > maxAge)
                {
                    return BadRequest(new { error = "Invalid age range" });
                }

                var paginationDto = new PaginationRequestDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _ownerService.GetByAgeRangePaginatedAsync(minAge, maxAge, paginationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting owners by age range with pagination");
                return StatusCode(500, new { error = "An error occurred while retrieving owners" });
            }
        }
    }
}
