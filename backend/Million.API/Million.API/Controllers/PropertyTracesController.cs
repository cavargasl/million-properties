using Microsoft.AspNetCore.Mvc;
using Million.API.DTOs;
using Million.API.Services;

namespace Million.API.Controllers
{
    [ApiController]
    [Route("api/properties/{propertyId}/traces")]
    [Produces("application/json")]
    public class PropertyTracesController : ControllerBase
    {
        private readonly PropertyTraceService _traceService;
        private readonly ILogger<PropertyTracesController> _logger;

        public PropertyTracesController(PropertyTraceService traceService, ILogger<PropertyTracesController> logger)
        {
            _traceService = traceService;
            _logger = logger;
        }

        /// <summary>
        /// Get all traces for a property
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <returns>List of traces for the property</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PropertyTraceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PropertyTraceDto>>> GetByPropertyId(string propertyId)
        {
            try
            {
                var traces = await _traceService.GetTracesByPropertyIdAsync(propertyId);
                return Ok(traces);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting traces for property {PropertyId}", propertyId);
                return StatusCode(500, new { error = "An error occurred while retrieving traces" });
            }
        }

        /// <summary>
        /// Get all traces for a property with pagination
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="pageNumber">Page number (starting from 1, default: 1)</param>
        /// <param name="pageSize">Items per page (max 100, default: 10)</param>
        /// <returns>Paginated list of traces for the property</returns>
        [HttpGet("paginated")]
        [ProducesResponseType(typeof(PaginatedResponseDto<PropertyTraceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<PropertyTraceDto>>> GetByPropertyIdPaginated(
            string propertyId,
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

                var result = await _traceService.GetTracesByPropertyIdPaginatedAsync(propertyId, paginationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting traces for property {PropertyId} with pagination", propertyId);
                return StatusCode(500, new { error = "An error occurred while retrieving traces" });
            }
        }

        /// <summary>
        /// Get trace by ID
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="id">Trace ID</param>
        /// <returns>Trace details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PropertyTraceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PropertyTraceDto>> GetById(string propertyId, string id)
        {
            try
            {
                var trace = await _traceService.GetTraceByIdAsync(id);
                
                if (trace == null || trace.IdProperty != propertyId)
                {
                    return NotFound($"Trace with ID '{id}' not found for property '{propertyId}'");
                }

                return Ok(trace);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trace {TraceId} for property {PropertyId}", id, propertyId);
                return StatusCode(500, new { error = "An error occurred while retrieving the trace" });
            }
        }

        /// <summary>
        /// Create new trace for a property
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="createDto">Trace data</param>
        /// <returns>Created trace</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PropertyTraceDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<PropertyTraceDto>> Create(
            string propertyId, 
            [FromBody] CreatePropertyTraceDto createDto)
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
                    createDto = new CreatePropertyTraceDto
                    {
                        IdProperty = propertyId,
                        DateSale = createDto.DateSale,
                        Name = createDto.Name,
                        Value = createDto.Value,
                        Tax = createDto.Tax
                    };
                }

                var trace = await _traceService.CreateTraceAsync(createDto);
                return CreatedAtAction(
                    nameof(GetById), 
                    new { propertyId, id = trace.IdPropertyTrace }, 
                    trace);
            }
            catch (ArgumentException ex)
            {
                // Errores de validación de negocio
                _logger.LogWarning(ex, "Validation error creating trace for property {PropertyId}", propertyId);
                return UnprocessableEntity(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Errores como propiedad no encontrada
                _logger.LogWarning(ex, "Business rule violation creating trace");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trace for property {PropertyId}", propertyId);
                return StatusCode(500, new { error = "An error occurred while creating the trace" });
            }
        }

        /// <summary>
        /// Update an existing trace
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="id">Trace ID</param>
        /// <param name="updateDto">Trace update data</param>
        /// <returns>Updated trace</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PropertyTraceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<PropertyTraceDto>> Update(
            string propertyId, 
            string id, 
            [FromBody] UpdatePropertyTraceDto updateDto)
        {
            // Validar ModelState (Data Annotations)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var trace = await _traceService.UpdateTraceAsync(id, updateDto);
                
                if (trace == null || trace.IdProperty != propertyId)
                {
                    return NotFound($"Trace with ID '{id}' not found for property '{propertyId}'");
                }

                return Ok(trace);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error updating trace {TraceId}", id);
                return UnprocessableEntity(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating trace {TraceId} for property {PropertyId}", id, propertyId);
                return StatusCode(500, new { error = "An error occurred while updating the trace" });
            }
        }

        /// <summary>
        /// Delete a trace
        /// </summary>
        /// <param name="propertyId">Property ID</param>
        /// <param name="id">Trace ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string propertyId, string id)
        {
            try
            {
                // Verificar que el trace pertenezca a la propiedad
                var trace = await _traceService.GetTraceByIdAsync(id);
                if (trace == null || trace.IdProperty != propertyId)
                {
                    return NotFound($"Trace with ID '{id}' not found for property '{propertyId}'");
                }

                await _traceService.DeleteTraceAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting trace {TraceId} for property {PropertyId}", id, propertyId);
                return StatusCode(500, new { error = "An error occurred while deleting the trace" });
            }
        }

        /// <summary>
        /// Get traces by date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="pageNumber">Page number (starting from 1, default: 1)</param>
        /// <param name="pageSize">Items per page (max 100, default: 10)</param>
        /// <returns>Paginated list of traces in the date range</returns>
        [HttpGet("~/api/traces/by-date/paginated")]
        [ProducesResponseType(typeof(PaginatedResponseDto<PropertyTraceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<PropertyTraceDto>>> GetByDateRangePaginated(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
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

                var result = await _traceService.GetTracesByDateRangePaginatedAsync(startDate, endDate, paginationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting traces by date range with pagination");
                return StatusCode(500, new { error = "An error occurred while retrieving traces" });
            }
        }

        /// <summary>
        /// Get traces by value range
        /// </summary>
        /// <param name="minValue">Minimum value</param>
        /// <param name="maxValue">Maximum value</param>
        /// <param name="pageNumber">Page number (starting from 1, default: 1)</param>
        /// <param name="pageSize">Items per page (max 100, default: 10)</param>
        /// <returns>Paginated list of traces in the value range</returns>
        [HttpGet("~/api/traces/by-value/paginated")]
        [ProducesResponseType(typeof(PaginatedResponseDto<PropertyTraceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResponseDto<PropertyTraceDto>>> GetByValueRangePaginated(
            [FromQuery] decimal minValue,
            [FromQuery] decimal maxValue,
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

                var result = await _traceService.GetTracesByValueRangePaginatedAsync(minValue, maxValue, paginationDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting traces by value range with pagination");
                return StatusCode(500, new { error = "An error occurred while retrieving traces" });
            }
        }
    }
}
