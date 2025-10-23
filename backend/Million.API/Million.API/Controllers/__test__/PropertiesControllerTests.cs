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
    /// Unit tests for PropertiesController using NUnit and Moq
    /// </summary>
    [TestFixture]
    public class PropertiesControllerTests
    {
        private Mock<IPropertyService>? _mockPropertyService;
        private Mock<ILogger<PropertiesController>>? _mockLogger;
        private PropertiesController? _controller;

        [SetUp]
        public void Setup()
        {
            _mockPropertyService = new Mock<IPropertyService>();
            _mockLogger = new Mock<ILogger<PropertiesController>>();
            _controller = new PropertiesController(_mockPropertyService.Object, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockPropertyService = null;
            _mockLogger = null;
            _controller = null;
        }

        #region Helper Methods

        private PropertyDto CreateTestPropertyDto(string id = "1", string name = "Test Property")
        {
            return new PropertyDto
            {
                IdProperty = id,
                IdOwner = "owner1",
                Name = name,
                Address = "123 Test St",
                Price = 250000,
                CodeInternal = "CODE001",
                Year = 2020,
                Image = "image1.jpg",
                OwnerName = "Owner Name"
            };
        }

        private PropertyDetailDto CreateTestPropertyDetailDto(string id = "1")
        {
            return new PropertyDetailDto
            {
                IdProperty = id,
                IdOwner = "owner1",
                Name = "Test Property",
                Address = "123 Test St",
                Price = 250000,
                CodeInternal = "CODE001",
                Year = 2020,
                Images = new List<PropertyImageDto>
                {
                    new PropertyImageDto
                    {
                        IdPropertyImage = "img1",
                        IdProperty = id,
                        File = "image1.jpg",
                        Enabled = true
                    }
                }
            };
        }

        #endregion

        #region Search Tests

        [Test]
        public async Task Search_ReturnsOkResult_WithListOfProperties()
        {
            // Arrange
            var properties = new List<PropertyDto>
            {
                CreateTestPropertyDto("1", "Property 1"),
                CreateTestPropertyDto("2", "Property 2")
            };

            _mockPropertyService!
                .Setup(service => service.SearchPropertiesAsync(It.IsAny<PropertyFilterDto>()))
                .ReturnsAsync(properties);

            // Act
            var result = await _controller!.Search(null, null, null, null);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedProperties = okResult.Value as IEnumerable<PropertyDto>;
            Assert.That(returnedProperties, Is.Not.Null);
            Assert.That(returnedProperties!.Count(), Is.EqualTo(2));

            _mockPropertyService.Verify(service => service.SearchPropertiesAsync(It.IsAny<PropertyFilterDto>()), Times.Once);
        }

        [Test]
        public async Task Search_WithFilters_ReturnsFilteredProperties()
        {
            // Arrange
            var properties = new List<PropertyDto>
            {
                CreateTestPropertyDto("1", "Modern House")
            };

            _mockPropertyService!
                .Setup(service => service.SearchPropertiesAsync(It.IsAny<PropertyFilterDto>()))
                .ReturnsAsync(properties);

            // Act
            var result = await _controller!.Search("Modern", "Main St", 100000, 300000);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var returnedProperties = okResult!.Value as IEnumerable<PropertyDto>;
            Assert.That(returnedProperties!.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task Search_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockPropertyService!
                .Setup(service => service.SearchPropertiesAsync(It.IsAny<PropertyFilterDto>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.Search(null, null, null, null);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region SearchPaginated Tests

        [Test]
        public async Task SearchPaginated_ReturnsOkResult_WithPaginatedData()
        {
            // Arrange
            var properties = new List<PropertyDto> { CreateTestPropertyDto() };
            var paginatedResponse = new PaginatedResponseDto<PropertyDto>(properties, 1, 1, 10);

            _mockPropertyService!
                .Setup(service => service.SearchPropertiesPaginatedAsync(
                    It.IsAny<PropertyFilterDto>(), 
                    It.IsAny<PaginationRequestDto>()))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller!.SearchPaginated(null, null, null, null, 1, 10);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedData = okResult.Value as PaginatedResponseDto<PropertyDto>;
            Assert.That(returnedData, Is.Not.Null);
            Assert.That(returnedData!.TotalRecords, Is.EqualTo(1));

            _mockPropertyService.Verify(service => service.SearchPropertiesPaginatedAsync(
                It.IsAny<PropertyFilterDto>(), 
                It.IsAny<PaginationRequestDto>()), Times.Once);
        }

        [Test]
        public async Task SearchPaginated_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockPropertyService!
                .Setup(service => service.SearchPropertiesPaginatedAsync(
                    It.IsAny<PropertyFilterDto>(), 
                    It.IsAny<PaginationRequestDto>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.SearchPaginated(null, null, null, null, 1, 10);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region GetById Tests

        [Test]
        public async Task GetById_ReturnsOkResult_WhenPropertyExists()
        {
            // Arrange
            var property = CreateTestPropertyDetailDto();

            _mockPropertyService!
                .Setup(service => service.GetPropertyByIdAsync("1"))
                .ReturnsAsync(property);

            // Act
            var result = await _controller!.GetById("1");

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedProperty = okResult.Value as PropertyDetailDto;
            Assert.That(returnedProperty, Is.Not.Null);
            Assert.That(returnedProperty!.IdProperty, Is.EqualTo("1"));

            _mockPropertyService.Verify(service => service.GetPropertyByIdAsync("1"), Times.Once);
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenPropertyDoesNotExist()
        {
            // Arrange
            _mockPropertyService!
                .Setup(service => service.GetPropertyByIdAsync("999"))
                .ReturnsAsync((PropertyDetailDto?)null);

            // Act
            var result = await _controller!.GetById("999");

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));

            _mockPropertyService.Verify(service => service.GetPropertyByIdAsync("999"), Times.Once);
        }

        [Test]
        public async Task GetById_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockPropertyService!
                .Setup(service => service.GetPropertyByIdAsync("1"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.GetById("1");

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region Create Tests

        [Test]
        public async Task Create_ReturnsCreatedAtAction_WithCreatedProperty()
        {
            // Arrange
            var createDto = new CreatePropertyDto
            {
                Name = "New Property",
                Address = "456 New St",
                Price = 300000,
                CodeInternal = "NEW001",
                Year = 2023,
                IdOwner = "owner1"
            };

            var createdProperty = CreateTestPropertyDto("123", "New Property");

            _mockPropertyService!
                .Setup(service => service.CreatePropertyAsync(It.IsAny<CreatePropertyDto>()))
                .ReturnsAsync(createdProperty);

            // Act
            var result = await _controller!.Create(createDto);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult!.StatusCode, Is.EqualTo(201));

            var returnedProperty = createdResult.Value as PropertyDto;
            Assert.That(returnedProperty, Is.Not.Null);
            Assert.That(returnedProperty!.Name, Is.EqualTo("New Property"));

            _mockPropertyService.Verify(service => service.CreatePropertyAsync(It.IsAny<CreatePropertyDto>()), Times.Once);
        }

        [Test]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = new CreatePropertyDto();
            _controller!.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockPropertyService!.Verify(service => service.CreatePropertyAsync(It.IsAny<CreatePropertyDto>()), Times.Never);
        }

        [Test]
        public async Task Create_ReturnsBadRequest_WhenInvalidOperationException()
        {
            // Arrange
            var createDto = new CreatePropertyDto
            {
                Name = "Property",
                Address = "Address",
                Price = 100000,
                CodeInternal = "DUP001",
                Year = 2020,
                IdOwner = "owner1"
            };

            _mockPropertyService!
                .Setup(service => service.CreatePropertyAsync(It.IsAny<CreatePropertyDto>()))
                .ThrowsAsync(new InvalidOperationException("Property with code 'DUP001' already exists"));

            // Act
            var result = await _controller!.Create(createDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task Create_ReturnsUnprocessableEntity_WhenArgumentException()
        {
            // Arrange
            var createDto = new CreatePropertyDto
            {
                Name = "Invalid Property",
                Address = "Address",
                Price = -100,
                CodeInternal = "CODE",
                Year = 2020,
                IdOwner = "owner1"
            };

            _mockPropertyService!
                .Setup(service => service.CreatePropertyAsync(It.IsAny<CreatePropertyDto>()))
                .ThrowsAsync(new ArgumentException("Invalid price"));

            // Act
            var result = await _controller!.Create(createDto);

            // Assert
            var unprocessableEntityResult = result.Result as UnprocessableEntityObjectResult;
            Assert.That(unprocessableEntityResult, Is.Not.Null);
            Assert.That(unprocessableEntityResult!.StatusCode, Is.EqualTo(422));
        }

        [Test]
        public async Task Create_ReturnsInternalServerError_WhenUnexpectedException()
        {
            // Arrange
            var createDto = new CreatePropertyDto
            {
                Name = "Property",
                Address = "Address",
                Price = 100000,
                CodeInternal = "CODE",
                Year = 2020,
                IdOwner = "owner1"
            };

            _mockPropertyService!
                .Setup(service => service.CreatePropertyAsync(It.IsAny<CreatePropertyDto>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller!.Create(createDto);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region Update Tests

        [Test]
        public async Task Update_ReturnsNoContent_WhenPropertyUpdated()
        {
            // Arrange
            var updateDto = new UpdatePropertyDto
            {
                Name = "Updated Property",
                Address = "789 Updated St",
                Price = 350000,
                CodeInternal = "UPD001",
                Year = 2022,
                IdOwner = "owner1"
            };

            _mockPropertyService!
                .Setup(service => service.UpdatePropertyAsync("1", It.IsAny<UpdatePropertyDto>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller!.Update("1", updateDto);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));

            _mockPropertyService.Verify(service => service.UpdatePropertyAsync("1", It.IsAny<UpdatePropertyDto>()), Times.Once);
        }

        [Test]
        public async Task Update_ReturnsNotFound_WhenPropertyDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdatePropertyDto
            {
                Name = "Updated Property",
                Address = "789 St",
                Price = 100000,
                CodeInternal = "CODE",
                Year = 2020,
                IdOwner = "owner1"
            };

            _mockPropertyService!
                .Setup(service => service.UpdatePropertyAsync("999", It.IsAny<UpdatePropertyDto>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller!.Update("999", updateDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task Update_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var updateDto = new UpdatePropertyDto();
            _controller!.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Update("1", updateDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockPropertyService!.Verify(service => service.UpdatePropertyAsync(It.IsAny<string>(), It.IsAny<UpdatePropertyDto>()), Times.Never);
        }

        [Test]
        public async Task Update_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var updateDto = new UpdatePropertyDto
            {
                Name = "Updated Property",
                Address = "789 St",
                Price = 100000,
                CodeInternal = "CODE",
                Year = 2020,
                IdOwner = "owner1"
            };

            _mockPropertyService!
                .Setup(service => service.UpdatePropertyAsync("1", It.IsAny<UpdatePropertyDto>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.Update("1", updateDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_ReturnsNoContent_WhenPropertyDeleted()
        {
            // Arrange
            _mockPropertyService!
                .Setup(service => service.DeletePropertyAsync("1"))
                .ReturnsAsync(true);

            // Act
            var result = await _controller!.Delete("1");

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));

            _mockPropertyService.Verify(service => service.DeletePropertyAsync("1"), Times.Once);
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenPropertyDoesNotExist()
        {
            // Arrange
            _mockPropertyService!
                .Setup(service => service.DeletePropertyAsync("999"))
                .ReturnsAsync(false);

            // Act
            var result = await _controller!.Delete("999");

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));

            _mockPropertyService.Verify(service => service.DeletePropertyAsync("999"), Times.Once);
        }

        [Test]
        public async Task Delete_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockPropertyService!
                .Setup(service => service.DeletePropertyAsync("1"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.Delete("1");

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion
    }
}
