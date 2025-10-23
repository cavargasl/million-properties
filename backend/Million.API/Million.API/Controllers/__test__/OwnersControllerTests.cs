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
    /// Unit tests for OwnersController using NUnit and Moq
    /// </summary>
    [TestFixture]
    public class OwnersControllerTests
    {
        private Mock<IOwnerService>? _mockOwnerService;
        private Mock<ILogger<OwnersController>>? _mockLogger;
        private OwnersController? _controller;

        [SetUp]
        public void Setup()
        {
            _mockOwnerService = new Mock<IOwnerService>();
            _mockLogger = new Mock<ILogger<OwnersController>>();
            _controller = new OwnersController(_mockOwnerService.Object, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockOwnerService = null;
            _mockLogger = null;
            _controller = null;
        }

        #region Helper Methods

        private OwnerDto CreateTestOwnerDto(string id = "1", string name = "Test Owner")
        {
            return new OwnerDto
            {
                IdOwner = id,
                Name = name,
                Address = "123 Test St",
                Photo = "https://example.com/photo.jpg",
                Birthday = new DateTime(1980, 1, 1)
            };
        }

        #endregion

        #region GetAll Tests

        [Test]
        public async Task GetAll_ReturnsOkResult_WithListOfOwners()
        {
            // Arrange
            var owners = new List<OwnerDto>
            {
                CreateTestOwnerDto("1", "Owner 1"),
                CreateTestOwnerDto("2", "Owner 2")
            };

            _mockOwnerService!
                .Setup(service => service.GetAllOwnersAsync())
                .ReturnsAsync(owners);

            // Act
            var result = await _controller!.GetAll();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedOwners = okResult.Value as IEnumerable<OwnerDto>;
            Assert.That(returnedOwners, Is.Not.Null);
            Assert.That(returnedOwners!.Count(), Is.EqualTo(2));

            _mockOwnerService.Verify(service => service.GetAllOwnersAsync(), Times.Once);
        }

        [Test]
        public async Task GetAll_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockOwnerService!
                .Setup(service => service.GetAllOwnersAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.GetAll();

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region GetAllPaginated Tests

        [Test]
        public async Task GetAllPaginated_ReturnsOkResult_WithPaginatedData()
        {
            // Arrange
            var owners = new List<OwnerDto> { CreateTestOwnerDto() };
            var paginatedResponse = new PaginatedResponseDto<OwnerDto>(owners, 1, 1, 10);

            _mockOwnerService!
                .Setup(service => service.GetAllOwnersPaginatedAsync(It.IsAny<PaginationRequestDto>()))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller!.GetAllPaginated(1, 10);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedData = okResult.Value as PaginatedResponseDto<OwnerDto>;
            Assert.That(returnedData, Is.Not.Null);
            Assert.That(returnedData!.TotalRecords, Is.EqualTo(1));

            _mockOwnerService.Verify(service => service.GetAllOwnersPaginatedAsync(It.IsAny<PaginationRequestDto>()), Times.Once);
        }

        [Test]
        public async Task GetAllPaginated_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockOwnerService!
                .Setup(service => service.GetAllOwnersPaginatedAsync(It.IsAny<PaginationRequestDto>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.GetAllPaginated(1, 10);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region GetById Tests

        [Test]
        public async Task GetById_ReturnsOkResult_WhenOwnerExists()
        {
            // Arrange
            var owner = CreateTestOwnerDto();

            _mockOwnerService!
                .Setup(service => service.GetOwnerByIdAsync("1"))
                .ReturnsAsync(owner);

            // Act
            var result = await _controller!.GetById("1");

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedOwner = okResult.Value as OwnerDto;
            Assert.That(returnedOwner, Is.Not.Null);
            Assert.That(returnedOwner!.IdOwner, Is.EqualTo("1"));

            _mockOwnerService.Verify(service => service.GetOwnerByIdAsync("1"), Times.Once);
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenOwnerDoesNotExist()
        {
            // Arrange
            _mockOwnerService!
                .Setup(service => service.GetOwnerByIdAsync("999"))
                .ReturnsAsync((OwnerDto?)null);

            // Act
            var result = await _controller!.GetById("999");

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));

            _mockOwnerService.Verify(service => service.GetOwnerByIdAsync("999"), Times.Once);
        }

        [Test]
        public async Task GetById_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockOwnerService!
                .Setup(service => service.GetOwnerByIdAsync("1"))
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
        public async Task Create_ReturnsCreatedAtAction_WithCreatedOwner()
        {
            // Arrange
            var createDto = new CreateOwnerDto
            {
                Name = "New Owner",
                Address = "456 New St",
                Photo = "https://example.com/photo.jpg",
                Birthday = new DateTime(1990, 5, 15)
            };

            var createdOwner = CreateTestOwnerDto("123", "New Owner");

            _mockOwnerService!
                .Setup(service => service.CreateOwnerAsync(It.IsAny<CreateOwnerDto>()))
                .ReturnsAsync(createdOwner);

            // Act
            var result = await _controller!.Create(createDto);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult!.StatusCode, Is.EqualTo(201));

            var returnedOwner = createdResult.Value as OwnerDto;
            Assert.That(returnedOwner, Is.Not.Null);
            Assert.That(returnedOwner!.Name, Is.EqualTo("New Owner"));

            _mockOwnerService.Verify(service => service.CreateOwnerAsync(It.IsAny<CreateOwnerDto>()), Times.Once);
        }

        [Test]
        public async Task Create_ReturnsBadRequest_WhenInvalidOperationException()
        {
            // Arrange
            var createDto = new CreateOwnerDto
            {
                Name = "Duplicate Owner",
                Address = "456 St",
                Birthday = new DateTime(1990, 5, 15)
            };

            _mockOwnerService!
                .Setup(service => service.CreateOwnerAsync(It.IsAny<CreateOwnerDto>()))
                .ThrowsAsync(new InvalidOperationException("Owner with name 'Duplicate Owner' already exists"));

            // Act
            var result = await _controller!.Create(createDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockOwnerService.Verify(service => service.CreateOwnerAsync(It.IsAny<CreateOwnerDto>()), Times.Once);
        }

        [Test]
        public async Task Create_ReturnsUnprocessableEntity_WhenArgumentException()
        {
            // Arrange
            var createDto = new CreateOwnerDto
            {
                Name = "Invalid Owner",
                Address = "456 St",
                Birthday = new DateTime(1990, 5, 15)
            };

            _mockOwnerService!
                .Setup(service => service.CreateOwnerAsync(It.IsAny<CreateOwnerDto>()))
                .ThrowsAsync(new ArgumentException("Invalid data"));

            // Act
            var result = await _controller!.Create(createDto);

            // Assert
            var unprocessableEntityResult = result.Result as UnprocessableEntityObjectResult;
            Assert.That(unprocessableEntityResult, Is.Not.Null);
            Assert.That(unprocessableEntityResult!.StatusCode, Is.EqualTo(422));

            _mockOwnerService.Verify(service => service.CreateOwnerAsync(It.IsAny<CreateOwnerDto>()), Times.Once);
        }

        [Test]
        public async Task Create_ReturnsInternalServerError_WhenUnexpectedException()
        {
            // Arrange
            var createDto = new CreateOwnerDto
            {
                Name = "New Owner",
                Address = "456 St",
                Birthday = new DateTime(1990, 5, 15)
            };

            _mockOwnerService!
                .Setup(service => service.CreateOwnerAsync(It.IsAny<CreateOwnerDto>()))
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
        public async Task Update_ReturnsOkResult_WhenOwnerUpdated()
        {
            // Arrange
            var updateDto = new UpdateOwnerDto
            {
                Name = "Updated Owner",
                Address = "789 Updated St",
                Photo = "https://example.com/updated.jpg",
                Birthday = new DateTime(1985, 3, 10)
            };

            var updatedOwner = CreateTestOwnerDto("1", "Updated Owner");

            _mockOwnerService!
                .Setup(service => service.UpdateOwnerAsync("1", It.IsAny<UpdateOwnerDto>()))
                .ReturnsAsync(updatedOwner);

            // Act
            var result = await _controller!.Update("1", updateDto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedOwner = okResult.Value as OwnerDto;
            Assert.That(returnedOwner, Is.Not.Null);
            Assert.That(returnedOwner!.Name, Is.EqualTo("Updated Owner"));

            _mockOwnerService.Verify(service => service.UpdateOwnerAsync("1", It.IsAny<UpdateOwnerDto>()), Times.Once);
        }

        [Test]
        public async Task Update_ReturnsNotFound_WhenOwnerDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateOwnerDto
            {
                Name = "Updated Owner",
                Address = "789 St",
                Birthday = new DateTime(1985, 3, 10)
            };

            _mockOwnerService!
                .Setup(service => service.UpdateOwnerAsync("999", It.IsAny<UpdateOwnerDto>()))
                .ReturnsAsync((OwnerDto?)null);

            // Act
            var result = await _controller!.Update("999", updateDto);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));

            _mockOwnerService.Verify(service => service.UpdateOwnerAsync("999", It.IsAny<UpdateOwnerDto>()), Times.Once);
        }

        [Test]
        public async Task Update_ReturnsUnprocessableEntity_WhenArgumentException()
        {
            // Arrange
            var updateDto = new UpdateOwnerDto
            {
                Name = "Invalid Owner",
                Address = "789 St",
                Birthday = new DateTime(1985, 3, 10)
            };

            _mockOwnerService!
                .Setup(service => service.UpdateOwnerAsync("1", It.IsAny<UpdateOwnerDto>()))
                .ThrowsAsync(new ArgumentException("Invalid data"));

            // Act
            var result = await _controller!.Update("1", updateDto);

            // Assert
            var unprocessableEntityResult = result.Result as UnprocessableEntityObjectResult;
            Assert.That(unprocessableEntityResult, Is.Not.Null);
            Assert.That(unprocessableEntityResult!.StatusCode, Is.EqualTo(422));
        }

        [Test]
        public async Task Update_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var updateDto = new UpdateOwnerDto
            {
                Name = "Updated Owner",
                Address = "789 St",
                Birthday = new DateTime(1985, 3, 10)
            };

            _mockOwnerService!
                .Setup(service => service.UpdateOwnerAsync("1", It.IsAny<UpdateOwnerDto>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.Update("1", updateDto);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_ReturnsNoContent_WhenOwnerDeleted()
        {
            // Arrange
            _mockOwnerService!
                .Setup(service => service.DeleteOwnerAsync("1"))
                .ReturnsAsync(true);

            // Act
            var result = await _controller!.Delete("1");

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));

            _mockOwnerService.Verify(service => service.DeleteOwnerAsync("1"), Times.Once);
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenOwnerDoesNotExist()
        {
            // Arrange
            _mockOwnerService!
                .Setup(service => service.DeleteOwnerAsync("999"))
                .ReturnsAsync(false);

            // Act
            var result = await _controller!.Delete("999");

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));

            _mockOwnerService.Verify(service => service.DeleteOwnerAsync("999"), Times.Once);
        }

        [Test]
        public async Task Delete_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockOwnerService!
                .Setup(service => service.DeleteOwnerAsync("1"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.Delete("1");

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region SearchByName Tests

        [Test]
        public async Task SearchByName_ReturnsOkResult_WithMatchingOwners()
        {
            // Arrange
            var owners = new List<OwnerDto>
            {
                CreateTestOwnerDto("1", "John Doe"),
                CreateTestOwnerDto("2", "John Smith")
            };

            _mockOwnerService!
                .Setup(service => service.SearchByNameAsync("John"))
                .ReturnsAsync(owners);

            // Act
            var result = await _controller!.SearchByName("John");

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedOwners = okResult.Value as IEnumerable<OwnerDto>;
            Assert.That(returnedOwners, Is.Not.Null);
            Assert.That(returnedOwners!.Count(), Is.EqualTo(2));

            _mockOwnerService.Verify(service => service.SearchByNameAsync("John"), Times.Once);
        }

        [Test]
        public async Task SearchByName_ReturnsBadRequest_WhenNameIsEmpty()
        {
            // Act
            var result = await _controller!.SearchByName("");

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockOwnerService!.Verify(service => service.SearchByNameAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task SearchByName_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockOwnerService!
                .Setup(service => service.SearchByNameAsync("John"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.SearchByName("John");

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region SearchByNamePaginated Tests

        [Test]
        public async Task SearchByNamePaginated_ReturnsOkResult_WithPaginatedData()
        {
            // Arrange
            var owners = new List<OwnerDto> { CreateTestOwnerDto("1", "John Doe") };
            var paginatedResponse = new PaginatedResponseDto<OwnerDto>(owners, 1, 1, 10);

            _mockOwnerService!
                .Setup(service => service.SearchByNamePaginatedAsync("John", It.IsAny<PaginationRequestDto>()))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller!.SearchByNamePaginated("John", 1, 10);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedData = okResult.Value as PaginatedResponseDto<OwnerDto>;
            Assert.That(returnedData, Is.Not.Null);
            Assert.That(returnedData!.TotalRecords, Is.EqualTo(1));

            _mockOwnerService.Verify(service => service.SearchByNamePaginatedAsync("John", It.IsAny<PaginationRequestDto>()), Times.Once);
        }

        [Test]
        public async Task SearchByNamePaginated_ReturnsBadRequest_WhenNameIsEmpty()
        {
            // Act
            var result = await _controller!.SearchByNamePaginated("", 1, 10);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockOwnerService!.Verify(service => service.SearchByNamePaginatedAsync(It.IsAny<string>(), It.IsAny<PaginationRequestDto>()), Times.Never);
        }

        #endregion

        #region SearchByAddress Tests

        [Test]
        public async Task SearchByAddress_ReturnsOkResult_WithMatchingOwners()
        {
            // Arrange
            var owners = new List<OwnerDto>
            {
                CreateTestOwnerDto("1", "Owner 1")
            };

            _mockOwnerService!
                .Setup(service => service.SearchByAddressAsync("Main"))
                .ReturnsAsync(owners);

            // Act
            var result = await _controller!.SearchByAddress("Main");

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedOwners = okResult.Value as IEnumerable<OwnerDto>;
            Assert.That(returnedOwners, Is.Not.Null);
            Assert.That(returnedOwners!.Count(), Is.EqualTo(1));

            _mockOwnerService.Verify(service => service.SearchByAddressAsync("Main"), Times.Once);
        }

        [Test]
        public async Task SearchByAddress_ReturnsBadRequest_WhenAddressIsEmpty()
        {
            // Act
            var result = await _controller!.SearchByAddress("");

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockOwnerService!.Verify(service => service.SearchByAddressAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region SearchByAddressPaginated Tests

        [Test]
        public async Task SearchByAddressPaginated_ReturnsOkResult_WithPaginatedData()
        {
            // Arrange
            var owners = new List<OwnerDto> { CreateTestOwnerDto() };
            var paginatedResponse = new PaginatedResponseDto<OwnerDto>(owners, 1, 1, 10);

            _mockOwnerService!
                .Setup(service => service.SearchByAddressPaginatedAsync("Main", It.IsAny<PaginationRequestDto>()))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller!.SearchByAddressPaginated("Main", 1, 10);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            _mockOwnerService.Verify(service => service.SearchByAddressPaginatedAsync("Main", It.IsAny<PaginationRequestDto>()), Times.Once);
        }

        [Test]
        public async Task SearchByAddressPaginated_ReturnsBadRequest_WhenAddressIsEmpty()
        {
            // Act
            var result = await _controller!.SearchByAddressPaginated("   ", 1, 10);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockOwnerService!.Verify(service => service.SearchByAddressPaginatedAsync(It.IsAny<string>(), It.IsAny<PaginationRequestDto>()), Times.Never);
        }

        #endregion

        #region GetByAgeRange Tests

        [Test]
        public async Task GetByAgeRange_ReturnsOkResult_WithMatchingOwners()
        {
            // Arrange
            var owners = new List<OwnerDto> { CreateTestOwnerDto() };

            _mockOwnerService!
                .Setup(service => service.GetByAgeRangeAsync(30, 50))
                .ReturnsAsync(owners);

            // Act
            var result = await _controller!.GetByAgeRange(30, 50);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedOwners = okResult.Value as IEnumerable<OwnerDto>;
            Assert.That(returnedOwners, Is.Not.Null);
            Assert.That(returnedOwners!.Count(), Is.EqualTo(1));

            _mockOwnerService.Verify(service => service.GetByAgeRangeAsync(30, 50), Times.Once);
        }

        [Test]
        public async Task GetByAgeRange_ReturnsBadRequest_WhenAgeRangeIsInvalid()
        {
            // Act
            var result = await _controller!.GetByAgeRange(50, 30); // minAge > maxAge

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockOwnerService!.Verify(service => service.GetByAgeRangeAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetByAgeRange_ReturnsBadRequest_WhenMinAgeIsNegative()
        {
            // Act
            var result = await _controller!.GetByAgeRange(-1, 50);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockOwnerService!.Verify(service => service.GetByAgeRangeAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        #endregion

        #region GetByAgeRangePaginated Tests

        [Test]
        public async Task GetByAgeRangePaginated_ReturnsOkResult_WithPaginatedData()
        {
            // Arrange
            var owners = new List<OwnerDto> { CreateTestOwnerDto() };
            var paginatedResponse = new PaginatedResponseDto<OwnerDto>(owners, 1, 1, 10);

            _mockOwnerService!
                .Setup(service => service.GetByAgeRangePaginatedAsync(30, 50, It.IsAny<PaginationRequestDto>()))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller!.GetByAgeRangePaginated(30, 50, 1, 10);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedData = okResult.Value as PaginatedResponseDto<OwnerDto>;
            Assert.That(returnedData, Is.Not.Null);
            Assert.That(returnedData!.TotalRecords, Is.EqualTo(1));

            _mockOwnerService.Verify(service => service.GetByAgeRangePaginatedAsync(30, 50, It.IsAny<PaginationRequestDto>()), Times.Once);
        }

        [Test]
        public async Task GetByAgeRangePaginated_ReturnsBadRequest_WhenAgeRangeIsInvalid()
        {
            // Act
            var result = await _controller!.GetByAgeRangePaginated(50, 30, 1, 10); // minAge > maxAge

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockOwnerService!.Verify(service => service.GetByAgeRangePaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<PaginationRequestDto>()), Times.Never);
        }

        [Test]
        public async Task GetByAgeRangePaginated_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockOwnerService!
                .Setup(service => service.GetByAgeRangePaginatedAsync(30, 50, It.IsAny<PaginationRequestDto>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.GetByAgeRangePaginated(30, 50, 1, 10);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion
    }
}
