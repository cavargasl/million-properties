using Microsoft.AspNetCore.Mvc;
using Million.API.Services;

namespace Million.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DatabaseController : ControllerBase
    {
        private readonly DataSeederService _seederService;
        private readonly ILogger<DatabaseController> _logger;

        public DatabaseController(DataSeederService seederService, ILogger<DatabaseController> logger)
        {
            _seederService = seederService;
            _logger = logger;
        }

        /// <summary>
        /// Seeds the database with initial mock data
        /// </summary>
        /// <remarks>
        /// This endpoint populates the database with sample data including:
        /// - 8 Owners
        /// - 15 Properties
        /// - Multiple Property Images (2-4 per property)
        /// - Multiple Property Traces (1-3 per property)
        /// 
        /// If data already exists, the operation will be skipped.
        /// </remarks>
        /// <returns>Success message with seeding results</returns>
        [HttpPost("seed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> SeedDatabase()
        {
            try
            {
                _logger.LogInformation("Seed database endpoint called");
                await _seederService.SeedDataAsync();
                
                return Ok(new
                {
                    success = true,
                    message = "Database seeded successfully!",
                    data = new
                    {
                        owners = 8,
                        properties = 15,
                        images = "2-4 per property",
                        traces = "1-3 per property"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding database");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error seeding database",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Deletes all data from the database and reseeds with fresh mock data
        /// </summary>
        /// <remarks>
        /// ⚠️ WARNING: This will delete ALL data in the database!
        /// Use only for development and testing purposes.
        /// </remarks>
        /// <returns>Success message with seeding results</returns>
        [HttpPost("reset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<object>> ResetDatabase()
        {
            try
            {
                _logger.LogWarning("Reset database endpoint called - ALL DATA WILL BE DELETED");
                await _seederService.ResetAndSeedAsync();
                
                return Ok(new
                {
                    success = true,
                    message = "Database reset and seeded successfully!",
                    data = new
                    {
                        owners = 8,
                        properties = 15,
                        images = "2-4 per property",
                        traces = "1-3 per property"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting database");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error resetting database",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<object> HealthCheck()
        {
            return Ok(new
            {
                success = true,
                message = "Database controller is healthy",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
