using Million.API.Domain;
using Million.API.DTOs;
using Million.API.Repository;

namespace Million.API.Services
{
    public class PropertyTraceService
    {
        private readonly PropertyTraceRepository _traceRepository;
        private readonly PropertyRepository _propertyRepository;

        public PropertyTraceService(
            PropertyTraceRepository traceRepository,
            PropertyRepository propertyRepository)
        {
            _traceRepository = traceRepository;
            _propertyRepository = propertyRepository;
        }

        /// <summary>
        /// Get all traces for a property
        /// </summary>
        public async Task<IEnumerable<PropertyTraceDto>> GetTracesByPropertyIdAsync(string propertyId)
        {
            var traces = await _traceRepository.GetByPropertyIdAsync(propertyId);
            return traces.Select(MapToDto);
        }

        /// <summary>
        /// Get trace by ID
        /// </summary>
        public async Task<PropertyTraceDto?> GetTraceByIdAsync(string id)
        {
            var trace = await _traceRepository.GetByIdAsync(id);
            return trace != null ? MapToDto(trace) : null;
        }

        /// <summary>
        /// Create new trace
        /// </summary>
        public async Task<PropertyTraceDto> CreateTraceAsync(CreatePropertyTraceDto createDto)
        {
            // Validar que la propiedad exista
            var property = await _propertyRepository.GetByIdAsync(createDto.IdProperty);
            if (property == null)
            {
                throw new InvalidOperationException($"Property with ID '{createDto.IdProperty}' not found");
            }

            var trace = new PropertyTrace
            {
                IdProperty = createDto.IdProperty,
                DateSale = createDto.DateSale,
                Name = createDto.Name,
                Value = createDto.Value,
                Tax = createDto.Tax
            };

            await _traceRepository.CreateAsync(trace);
            return MapToDto(trace);
        }

        /// <summary>
        /// Update existing trace
        /// </summary>
        public async Task<PropertyTraceDto?> UpdateTraceAsync(string id, UpdatePropertyTraceDto updateDto)
        {
            var existingTrace = await _traceRepository.GetByIdAsync(id);
            if (existingTrace == null) return null;

            existingTrace.DateSale = updateDto.DateSale;
            existingTrace.Name = updateDto.Name;
            existingTrace.Value = updateDto.Value;
            existingTrace.Tax = updateDto.Tax;

            await _traceRepository.UpdateAsync(id, existingTrace);
            return MapToDto(existingTrace);
        }

        /// <summary>
        /// Delete trace
        /// </summary>
        public async Task<bool> DeleteTraceAsync(string id)
        {
            var trace = await _traceRepository.GetByIdAsync(id);
            if (trace == null) return false;

            await _traceRepository.DeleteAsync(id);
            return true;
        }

        /// <summary>
        /// Get traces by date range
        /// </summary>
        public async Task<IEnumerable<PropertyTraceDto>> GetTracesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var traces = await _traceRepository.GetByDateRangeAsync(startDate, endDate);
            return traces.Select(MapToDto);
        }

        /// <summary>
        /// Get traces by value range
        /// </summary>
        public async Task<IEnumerable<PropertyTraceDto>> GetTracesByValueRangeAsync(decimal minValue, decimal maxValue)
        {
            var traces = await _traceRepository.GetByValueRangeAsync(minValue, maxValue);
            return traces.Select(MapToDto);
        }

        private static PropertyTraceDto MapToDto(PropertyTrace trace)
        {
            return new PropertyTraceDto
            {
                IdPropertyTrace = trace.IdPropertyTrace,
                IdProperty = trace.IdProperty,
                DateSale = trace.DateSale,
                Name = trace.Name,
                Value = trace.Value,
                Tax = trace.Tax
            };
        }
    }
}
