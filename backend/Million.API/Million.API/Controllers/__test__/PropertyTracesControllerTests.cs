using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Million.API.Controllers;
using Million.API.DTOs;
using Million.API.Services;
using Moq;
using NUnit.Framework;

namespace Million.API.Controllers.Tests
{
    /// <summary>
    /// Unit tests for PropertyTracesController using NUnit and Moq
    /// </summary>
    [TestFixture]
    public class PropertyTracesControllerTests
    {
        private Mock<IPropertyTraceService>? _mockTraceService;
        private Mock<ILogger<PropertyTracesController>>? _mockLogger;
        private PropertyTracesController? _controller;

        [SetUp]
        public void Setup()
        {
            _mockTraceService = new Mock<IPropertyTraceService>();
            _mockLogger = new Mock<ILogger<PropertyTracesController>>();
            _controller = new PropertyTracesController(_mockTraceService.Object, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockTraceService = null;
            _mockLogger = null;
            _controller = null;
        }

        #region Helper Methods

        private PropertyTraceDto CreateTestTraceDto(string id = "1", string propertyId = "prop1")
        {
            return new PropertyTraceDto
            {
                IdPropertyTrace = id,
                IdProperty = propertyId,
                DateSale = DateTime.Now,
                Name = "Test Sale",
                Value = 150000,
                Tax = 7500
            };
        }

        #endregion

        #region GetByPropertyId Tests

        [Test]
        public async Task GetByPropertyId_ReturnsOkResult_WithListOfTraces()
        {
            // Arrange
            var traces = new List<PropertyTraceDto>
            {
                CreateTestTraceDto("1", "prop1"),
                CreateTestTraceDto("2", "prop1")
            };

            _mockTraceService!
                .Setup(service => service.GetTracesByPropertyIdAsync("prop1"))
                .ReturnsAsync(traces);

            // Act
            var result = await _controller!.GetByPropertyId("prop1");

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedTraces = okResult.Value as IEnumerable<PropertyTraceDto>;
            Assert.That(returnedTraces, Is.Not.Null);
            Assert.That(returnedTraces!.Count(), Is.EqualTo(2));

            _mockTraceService.Verify(service => service.GetTracesByPropertyIdAsync("prop1"), Times.Once);
        }

        [Test]
        public async Task GetByPropertyId_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockTraceService!
                .Setup(service => service.GetTracesByPropertyIdAsync("prop1"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.GetByPropertyId("prop1");

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region GetByPropertyIdPaginated Tests

        [Test]
        public async Task GetByPropertyIdPaginated_ReturnsOkResult_WithPaginatedData()
        {
            // Arrange
            var traces = new List<PropertyTraceDto> { CreateTestTraceDto() };
            var paginatedResponse = new PaginatedResponseDto<PropertyTraceDto>(traces, 1, 1, 10);

            _mockTraceService!
                .Setup(service => service.GetTracesByPropertyIdPaginatedAsync("prop1", It.IsAny<PaginationRequestDto>()))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller!.GetByPropertyIdPaginated("prop1", 1, 10);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedData = okResult.Value as PaginatedResponseDto<PropertyTraceDto>;
            Assert.That(returnedData, Is.Not.Null);
            Assert.That(returnedData!.TotalRecords, Is.EqualTo(1));

            _mockTraceService.Verify(service => service.GetTracesByPropertyIdPaginatedAsync("prop1", It.IsAny<PaginationRequestDto>()), Times.Once);
        }

        [Test]
        public async Task GetByPropertyIdPaginated_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockTraceService!
                .Setup(service => service.GetTracesByPropertyIdPaginatedAsync("prop1", It.IsAny<PaginationRequestDto>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.GetByPropertyIdPaginated("prop1", 1, 10);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region GetById Tests

        [Test]
        public async Task GetById_ReturnsOkResult_WhenTraceExists()
        {
            // Arrange
            var trace = CreateTestTraceDto("1", "prop1");

            _mockTraceService!
                .Setup(service => service.GetTraceByIdAsync("1"))
                .ReturnsAsync(trace);

            // Act
            var result = await _controller!.GetById("prop1", "1");

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedTrace = okResult.Value as PropertyTraceDto;
            Assert.That(returnedTrace, Is.Not.Null);
            Assert.That(returnedTrace!.IdPropertyTrace, Is.EqualTo("1"));

            _mockTraceService.Verify(service => service.GetTraceByIdAsync("1"), Times.Once);
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenTraceDoesNotExist()
        {
            // Arrange
            _mockTraceService!
                .Setup(service => service.GetTraceByIdAsync("999"))
                .ReturnsAsync((PropertyTraceDto?)null);

            // Act
            var result = await _controller!.GetById("prop1", "999");

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));

            _mockTraceService.Verify(service => service.GetTraceByIdAsync("999"), Times.Once);
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenTraceBelongsToDifferentProperty()
        {
            // Arrange
            var trace = CreateTestTraceDto("1", "prop2");

            _mockTraceService!
                .Setup(service => service.GetTraceByIdAsync("1"))
                .ReturnsAsync(trace);

            // Act
            var result = await _controller!.GetById("prop1", "1");

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetById_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockTraceService!
                .Setup(service => service.GetTraceByIdAsync("1"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.GetById("prop1", "1");

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region Create Tests

        [Test]
        public async Task Create_ReturnsCreatedAtAction_WithCreatedTrace()
        {
            // Arrange
            var createDto = new CreatePropertyTraceDto
            {
                IdProperty = "prop1",
                DateSale = DateTime.Now,
                Name = "New Sale",
                Value = 250000,
                Tax = 12500
            };

            var createdTrace = CreateTestTraceDto("123", "prop1");

            _mockTraceService!
                .Setup(service => service.CreateTraceAsync(It.IsAny<CreatePropertyTraceDto>()))
                .ReturnsAsync(createdTrace);

            // Act
            var result = await _controller!.Create("prop1", createDto);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult!.StatusCode, Is.EqualTo(201));

            var returnedTrace = createdResult.Value as PropertyTraceDto;
            Assert.That(returnedTrace, Is.Not.Null);

            _mockTraceService.Verify(service => service.CreateTraceAsync(It.IsAny<CreatePropertyTraceDto>()), Times.Once);
        }

        [Test]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = new CreatePropertyTraceDto();
            _controller!.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Create("prop1", createDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockTraceService!.Verify(service => service.CreateTraceAsync(It.IsAny<CreatePropertyTraceDto>()), Times.Never);
        }

        [Test]
        public async Task Create_ReturnsBadRequest_WhenInvalidOperationException()
        {
            // Arrange
            var createDto = new CreatePropertyTraceDto
            {
                IdProperty = "invalid",
                DateSale = DateTime.Now,
                Name = "Sale",
                Value = 100000,
                Tax = 5000
            };

            _mockTraceService!
                .Setup(service => service.CreateTraceAsync(It.IsAny<CreatePropertyTraceDto>()))
                .ThrowsAsync(new InvalidOperationException("Property not found"));

            // Act
            var result = await _controller!.Create("invalid", createDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task Create_ReturnsUnprocessableEntity_WhenArgumentException()
        {
            // Arrange
            var createDto = new CreatePropertyTraceDto
            {
                IdProperty = "prop1",
                DateSale = DateTime.Now,
                Name = "Invalid Sale",
                Value = -100,
                Tax = 5000
            };

            _mockTraceService!
                .Setup(service => service.CreateTraceAsync(It.IsAny<CreatePropertyTraceDto>()))
                .ThrowsAsync(new ArgumentException("Invalid value"));

            // Act
            var result = await _controller!.Create("prop1", createDto);

            // Assert
            var unprocessableEntityResult = result.Result as UnprocessableEntityObjectResult;
            Assert.That(unprocessableEntityResult, Is.Not.Null);
            Assert.That(unprocessableEntityResult!.StatusCode, Is.EqualTo(422));
        }

        [Test]
        public async Task Create_ReturnsInternalServerError_WhenUnexpectedException()
        {
            // Arrange
            var createDto = new CreatePropertyTraceDto
            {
                IdProperty = "prop1",
                DateSale = DateTime.Now,
                Name = "Sale",
                Value = 100000,
                Tax = 5000
            };

            _mockTraceService!
                .Setup(service => service.CreateTraceAsync(It.IsAny<CreatePropertyTraceDto>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller!.Create("prop1", createDto);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region Update Tests

        [Test]
        public async Task Update_ReturnsOkResult_WhenTraceUpdated()
        {
            // Arrange
            var updateDto = new UpdatePropertyTraceDto
            {
                DateSale = DateTime.Now.AddDays(1),
                Name = "Updated Sale",
                Value = 300000,
                Tax = 15000
            };

            var updatedTrace = CreateTestTraceDto("1", "prop1");

            _mockTraceService!
                .Setup(service => service.UpdateTraceAsync("1", It.IsAny<UpdatePropertyTraceDto>()))
                .ReturnsAsync(updatedTrace);

            // Act
            var result = await _controller!.Update("prop1", "1", updateDto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedTrace = okResult.Value as PropertyTraceDto;
            Assert.That(returnedTrace, Is.Not.Null);

            _mockTraceService.Verify(service => service.UpdateTraceAsync("1", It.IsAny<UpdatePropertyTraceDto>()), Times.Once);
        }

        [Test]
        public async Task Update_ReturnsNotFound_WhenTraceDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdatePropertyTraceDto
            {
                DateSale = DateTime.Now,
                Name = "Updated Sale",
                Value = 100000,
                Tax = 5000
            };

            _mockTraceService!
                .Setup(service => service.UpdateTraceAsync("999", It.IsAny<UpdatePropertyTraceDto>()))
                .ReturnsAsync((PropertyTraceDto?)null);

            // Act
            var result = await _controller!.Update("prop1", "999", updateDto);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var updateDto = new UpdatePropertyTraceDto();
            _controller!.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Update("prop1", "1", updateDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockTraceService!.Verify(service => service.UpdateTraceAsync(It.IsAny<string>(), It.IsAny<UpdatePropertyTraceDto>()), Times.Never);
        }

        [Test]
        public async Task Update_ReturnsUnprocessableEntity_WhenArgumentException()
        {
            // Arrange
            var updateDto = new UpdatePropertyTraceDto
            {
                DateSale = DateTime.Now,
                Name = "Invalid Sale",
                Value = -100,
                Tax = 5000
            };

            _mockTraceService!
                .Setup(service => service.UpdateTraceAsync("1", It.IsAny<UpdatePropertyTraceDto>()))
                .ThrowsAsync(new ArgumentException("Invalid value"));

            // Act
            var result = await _controller!.Update("prop1", "1", updateDto);

            // Assert
            var unprocessableEntityResult = result.Result as UnprocessableEntityObjectResult;
            Assert.That(unprocessableEntityResult, Is.Not.Null);
            Assert.That(unprocessableEntityResult!.StatusCode, Is.EqualTo(422));
        }

        [Test]
        public async Task Update_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var updateDto = new UpdatePropertyTraceDto
            {
                DateSale = DateTime.Now,
                Name = "Updated Sale",
                Value = 100000,
                Tax = 5000
            };

            _mockTraceService!
                .Setup(service => service.UpdateTraceAsync("1", It.IsAny<UpdatePropertyTraceDto>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.Update("prop1", "1", updateDto);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_ReturnsNoContent_WhenTraceDeleted()
        {
            // Arrange
            var trace = CreateTestTraceDto("1", "prop1");

            _mockTraceService!
                .Setup(service => service.GetTraceByIdAsync("1"))
                .ReturnsAsync(trace);

            _mockTraceService
                .Setup(service => service.DeleteTraceAsync("1"))
                .ReturnsAsync(true);

            // Act
            var result = await _controller!.Delete("prop1", "1");

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));

            _mockTraceService.Verify(service => service.DeleteTraceAsync("1"), Times.Once);
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenTraceDoesNotExist()
        {
            // Arrange
            _mockTraceService!
                .Setup(service => service.GetTraceByIdAsync("999"))
                .ReturnsAsync((PropertyTraceDto?)null);

            // Act
            var result = await _controller!.Delete("prop1", "999");

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));

            _mockTraceService.Verify(service => service.DeleteTraceAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenTraceBelongsToDifferentProperty()
        {
            // Arrange
            var trace = CreateTestTraceDto("1", "prop2");

            _mockTraceService!
                .Setup(service => service.GetTraceByIdAsync("1"))
                .ReturnsAsync(trace);

            // Act
            var result = await _controller!.Delete("prop1", "1");

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task Delete_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var trace = CreateTestTraceDto("1", "prop1");

            _mockTraceService!
                .Setup(service => service.GetTraceByIdAsync("1"))
                .ReturnsAsync(trace);

            _mockTraceService
                .Setup(service => service.DeleteTraceAsync("1"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.Delete("prop1", "1");

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region GetByDateRangePaginated Tests

        [Test]
        public async Task GetByDateRangePaginated_ReturnsOkResult_WithPaginatedData()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 12, 31);
            var traces = new List<PropertyTraceDto> { CreateTestTraceDto() };
            var paginatedResponse = new PaginatedResponseDto<PropertyTraceDto>(traces, 1, 1, 10);

            _mockTraceService!
                .Setup(service => service.GetTracesByDateRangePaginatedAsync(startDate, endDate, It.IsAny<PaginationRequestDto>()))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller!.GetByDateRangePaginated(startDate, endDate, 1, 10);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedData = okResult.Value as PaginatedResponseDto<PropertyTraceDto>;
            Assert.That(returnedData, Is.Not.Null);
            Assert.That(returnedData!.TotalRecords, Is.EqualTo(1));

            _mockTraceService.Verify(service => service.GetTracesByDateRangePaginatedAsync(startDate, endDate, It.IsAny<PaginationRequestDto>()), Times.Once);
        }

        [Test]
        public async Task GetByDateRangePaginated_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 12, 31);

            _mockTraceService!
                .Setup(service => service.GetTracesByDateRangePaginatedAsync(startDate, endDate, It.IsAny<PaginationRequestDto>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.GetByDateRangePaginated(startDate, endDate, 1, 10);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region GetByValueRangePaginated Tests

        [Test]
        public async Task GetByValueRangePaginated_ReturnsOkResult_WithPaginatedData()
        {
            // Arrange
            var traces = new List<PropertyTraceDto> { CreateTestTraceDto() };
            var paginatedResponse = new PaginatedResponseDto<PropertyTraceDto>(traces, 1, 1, 10);

            _mockTraceService!
                .Setup(service => service.GetTracesByValueRangePaginatedAsync(100000, 200000, It.IsAny<PaginationRequestDto>()))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller!.GetByValueRangePaginated(100000, 200000, 1, 10);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedData = okResult.Value as PaginatedResponseDto<PropertyTraceDto>;
            Assert.That(returnedData, Is.Not.Null);
            Assert.That(returnedData!.TotalRecords, Is.EqualTo(1));

            _mockTraceService.Verify(service => service.GetTracesByValueRangePaginatedAsync(100000, 200000, It.IsAny<PaginationRequestDto>()), Times.Once);
        }

        [Test]
        public async Task GetByValueRangePaginated_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockTraceService!
                .Setup(service => service.GetTracesByValueRangePaginatedAsync(100000, 200000, It.IsAny<PaginationRequestDto>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.GetByValueRangePaginated(100000, 200000, 1, 10);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion
    }
}
