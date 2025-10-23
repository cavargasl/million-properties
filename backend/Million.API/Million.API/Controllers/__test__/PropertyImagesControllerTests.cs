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
    /// Unit tests for PropertyImagesController using NUnit and Moq
    /// </summary>
    [TestFixture]
    public class PropertyImagesControllerTests
    {
        private Mock<IPropertyImageService>? _mockImageService;
        private Mock<ILogger<PropertyImagesController>>? _mockLogger;
        private PropertyImagesController? _controller;

        [SetUp]
        public void Setup()
        {
            _mockImageService = new Mock<IPropertyImageService>();
            _mockLogger = new Mock<ILogger<PropertyImagesController>>();
            _controller = new PropertyImagesController(_mockImageService.Object, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockImageService = null;
            _mockLogger = null;
            _controller = null;
        }

        #region Helper Methods

        private PropertyImageDto CreateTestImageDto(string id = "1", string propertyId = "prop1")
        {
            return new PropertyImageDto
            {
                IdPropertyImage = id,
                IdProperty = propertyId,
                File = "image.jpg",
                Enabled = true
            };
        }

        #endregion

        #region GetByPropertyId Tests

        [Test]
        public async Task GetByPropertyId_ReturnsOkResult_WithListOfImages()
        {
            // Arrange
            var images = new List<PropertyImageDto>
            {
                CreateTestImageDto("1", "prop1"),
                CreateTestImageDto("2", "prop1")
            };

            _mockImageService!
                .Setup(service => service.GetImagesByPropertyIdAsync("prop1"))
                .ReturnsAsync(images);

            // Act
            var result = await _controller!.GetByPropertyId("prop1");

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedImages = okResult.Value as IEnumerable<PropertyImageDto>;
            Assert.That(returnedImages, Is.Not.Null);
            Assert.That(returnedImages!.Count(), Is.EqualTo(2));

            _mockImageService.Verify(service => service.GetImagesByPropertyIdAsync("prop1"), Times.Once);
        }

        [Test]
        public async Task GetByPropertyId_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockImageService!
                .Setup(service => service.GetImagesByPropertyIdAsync("prop1"))
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
            var images = new List<PropertyImageDto> { CreateTestImageDto() };
            var paginatedResponse = new PaginatedResponseDto<PropertyImageDto>(images, 1, 1, 10);

            _mockImageService!
                .Setup(service => service.GetImagesByPropertyIdPaginatedAsync("prop1", It.IsAny<PaginationRequestDto>()))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller!.GetByPropertyIdPaginated("prop1", 1, 10);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedData = okResult.Value as PaginatedResponseDto<PropertyImageDto>;
            Assert.That(returnedData, Is.Not.Null);
            Assert.That(returnedData!.TotalRecords, Is.EqualTo(1));

            _mockImageService.Verify(service => service.GetImagesByPropertyIdPaginatedAsync("prop1", It.IsAny<PaginationRequestDto>()), Times.Once);
        }

        [Test]
        public async Task GetByPropertyIdPaginated_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockImageService!
                .Setup(service => service.GetImagesByPropertyIdPaginatedAsync("prop1", It.IsAny<PaginationRequestDto>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.GetByPropertyIdPaginated("prop1", 1, 10);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region AddImage Tests

        [Test]
        public async Task AddImage_ReturnsCreatedAtAction_WithCreatedImage()
        {
            // Arrange
            var createDto = new CreatePropertyImageDto
            {
                IdProperty = "prop1",
                File = "new-image.jpg",
                Enabled = true
            };

            var createdImage = CreateTestImageDto("123", "prop1");

            _mockImageService!
                .Setup(service => service.AddImageAsync(It.IsAny<CreatePropertyImageDto>()))
                .ReturnsAsync(createdImage);

            // Act
            var result = await _controller!.AddImage("prop1", createDto);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult!.StatusCode, Is.EqualTo(201));

            var returnedImage = createdResult.Value as PropertyImageDto;
            Assert.That(returnedImage, Is.Not.Null);

            _mockImageService.Verify(service => service.AddImageAsync(It.IsAny<CreatePropertyImageDto>()), Times.Once);
        }

        [Test]
        public async Task AddImage_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createDto = new CreatePropertyImageDto();
            _controller!.ModelState.AddModelError("File", "File is required");

            // Act
            var result = await _controller.AddImage("prop1", createDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockImageService!.Verify(service => service.AddImageAsync(It.IsAny<CreatePropertyImageDto>()), Times.Never);
        }

        [Test]
        public async Task AddImage_ReturnsBadRequest_WhenInvalidOperationException()
        {
            // Arrange
            var createDto = new CreatePropertyImageDto
            {
                IdProperty = "invalid",
                File = "image.jpg",
                Enabled = true
            };

            _mockImageService!
                .Setup(service => service.AddImageAsync(It.IsAny<CreatePropertyImageDto>()))
                .ThrowsAsync(new InvalidOperationException("Property not found"));

            // Act
            var result = await _controller!.AddImage("invalid", createDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task AddImage_ReturnsInternalServerError_WhenUnexpectedException()
        {
            // Arrange
            var createDto = new CreatePropertyImageDto
            {
                IdProperty = "prop1",
                File = "image.jpg",
                Enabled = true
            };

            _mockImageService!
                .Setup(service => service.AddImageAsync(It.IsAny<CreatePropertyImageDto>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller!.AddImage("prop1", createDto);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region AddImagesBulk Tests

        [Test]
        public async Task AddImagesBulk_ReturnsCreatedAtAction_WithCreatedImages()
        {
            // Arrange
            var imageDtos = new List<CreatePropertyImageDto>
            {
                new CreatePropertyImageDto { IdProperty = "prop1", File = "image1.jpg", Enabled = true },
                new CreatePropertyImageDto { IdProperty = "prop1", File = "image2.jpg", Enabled = true }
            };

            var createdImages = new List<PropertyImageDto>
            {
                CreateTestImageDto("1", "prop1"),
                CreateTestImageDto("2", "prop1")
            };

            _mockImageService!
                .Setup(service => service.AddImagesBulkAsync("prop1", It.IsAny<List<CreatePropertyImageDto>>()))
                .ReturnsAsync(createdImages);

            // Act
            var result = await _controller!.AddImagesBulk("prop1", imageDtos);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult!.StatusCode, Is.EqualTo(201));

            _mockImageService.Verify(service => service.AddImagesBulkAsync("prop1", It.IsAny<List<CreatePropertyImageDto>>()), Times.Once);
        }

        [Test]
        public async Task AddImagesBulk_ReturnsBadRequest_WhenListIsEmpty()
        {
            // Arrange
            var imageDtos = new List<CreatePropertyImageDto>();

            // Act
            var result = await _controller!.AddImagesBulk("prop1", imageDtos);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockImageService!.Verify(service => service.AddImagesBulkAsync(It.IsAny<string>(), It.IsAny<List<CreatePropertyImageDto>>()), Times.Never);
        }

        [Test]
        public async Task AddImagesBulk_ReturnsBadRequest_WhenTooManyImages()
        {
            // Arrange
            var imageDtos = new List<CreatePropertyImageDto>();
            for (int i = 0; i < 11; i++)
            {
                imageDtos.Add(new CreatePropertyImageDto { IdProperty = "prop1", File = $"image{i}.jpg", Enabled = true });
            }

            // Act
            var result = await _controller!.AddImagesBulk("prop1", imageDtos);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockImageService!.Verify(service => service.AddImagesBulkAsync(It.IsAny<string>(), It.IsAny<List<CreatePropertyImageDto>>()), Times.Never);
        }

        [Test]
        public async Task AddImagesBulk_ReturnsBadRequest_WhenInvalidOperationException()
        {
            // Arrange
            var imageDtos = new List<CreatePropertyImageDto>
            {
                new CreatePropertyImageDto { IdProperty = "invalid", File = "image.jpg", Enabled = true }
            };

            _mockImageService!
                .Setup(service => service.AddImagesBulkAsync("invalid", It.IsAny<List<CreatePropertyImageDto>>()))
                .ThrowsAsync(new InvalidOperationException("Property not found"));

            // Act
            var result = await _controller!.AddImagesBulk("invalid", imageDtos);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));
        }

        #endregion

        #region GetById Tests

        [Test]
        public async Task GetById_ReturnsOkResult_WhenImageExists()
        {
            // Arrange
            var image = CreateTestImageDto("1", "prop1");

            _mockImageService!
                .Setup(service => service.GetImageByIdAsync("1"))
                .ReturnsAsync(image);

            // Act
            var result = await _controller!.GetById("prop1", "1");

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedImage = okResult.Value as PropertyImageDto;
            Assert.That(returnedImage, Is.Not.Null);
            Assert.That(returnedImage!.IdPropertyImage, Is.EqualTo("1"));

            _mockImageService.Verify(service => service.GetImageByIdAsync("1"), Times.Once);
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenImageDoesNotExist()
        {
            // Arrange
            _mockImageService!
                .Setup(service => service.GetImageByIdAsync("999"))
                .ReturnsAsync((PropertyImageDto?)null);

            // Act
            var result = await _controller!.GetById("prop1", "999");

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));

            _mockImageService.Verify(service => service.GetImageByIdAsync("999"), Times.Once);
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenImageBelongsToDifferentProperty()
        {
            // Arrange
            var image = CreateTestImageDto("1", "prop2");

            _mockImageService!
                .Setup(service => service.GetImageByIdAsync("1"))
                .ReturnsAsync(image);

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
            _mockImageService!
                .Setup(service => service.GetImageByIdAsync("1"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.GetById("prop1", "1");

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region Update Tests

        [Test]
        public async Task Update_ReturnsOkResult_WhenImageUpdated()
        {
            // Arrange
            var updateDto = new UpdatePropertyImageDto
            {
                File = "updated-image.jpg",
                Enabled = false
            };

            var updatedImage = CreateTestImageDto("1", "prop1");

            _mockImageService!
                .Setup(service => service.UpdateImageAsync("1", It.IsAny<UpdatePropertyImageDto>()))
                .ReturnsAsync(updatedImage);

            // Act
            var result = await _controller!.Update("prop1", "1", updateDto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.StatusCode, Is.EqualTo(200));

            var returnedImage = okResult.Value as PropertyImageDto;
            Assert.That(returnedImage, Is.Not.Null);

            _mockImageService.Verify(service => service.UpdateImageAsync("1", It.IsAny<UpdatePropertyImageDto>()), Times.Once);
        }

        [Test]
        public async Task Update_ReturnsNotFound_WhenImageDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdatePropertyImageDto
            {
                File = "updated-image.jpg",
                Enabled = false
            };

            _mockImageService!
                .Setup(service => service.UpdateImageAsync("999", It.IsAny<UpdatePropertyImageDto>()))
                .ReturnsAsync((PropertyImageDto?)null);

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
            var updateDto = new UpdatePropertyImageDto();
            _controller!.ModelState.AddModelError("File", "File is required");

            // Act
            var result = await _controller.Update("prop1", "1", updateDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult!.StatusCode, Is.EqualTo(400));

            _mockImageService!.Verify(service => service.UpdateImageAsync(It.IsAny<string>(), It.IsAny<UpdatePropertyImageDto>()), Times.Never);
        }

        [Test]
        public async Task Update_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var updateDto = new UpdatePropertyImageDto
            {
                File = "updated-image.jpg",
                Enabled = false
            };

            _mockImageService!
                .Setup(service => service.UpdateImageAsync("1", It.IsAny<UpdatePropertyImageDto>()))
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
        public async Task Delete_ReturnsNoContent_WhenImageDeleted()
        {
            // Arrange
            var image = CreateTestImageDto("1", "prop1");

            _mockImageService!
                .Setup(service => service.GetImageByIdAsync("1"))
                .ReturnsAsync(image);

            _mockImageService
                .Setup(service => service.DeleteImageAsync("1"))
                .ReturnsAsync(true);

            // Act
            var result = await _controller!.Delete("prop1", "1");

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));

            _mockImageService.Verify(service => service.DeleteImageAsync("1"), Times.Once);
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenImageDoesNotExist()
        {
            // Arrange
            _mockImageService!
                .Setup(service => service.GetImageByIdAsync("999"))
                .ReturnsAsync((PropertyImageDto?)null);

            // Act
            var result = await _controller!.Delete("prop1", "999");

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));

            _mockImageService.Verify(service => service.DeleteImageAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenImageBelongsToDifferentProperty()
        {
            // Arrange
            var image = CreateTestImageDto("1", "prop2");

            _mockImageService!
                .Setup(service => service.GetImageByIdAsync("1"))
                .ReturnsAsync(image);

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
            var image = CreateTestImageDto("1", "prop1");

            _mockImageService!
                .Setup(service => service.GetImageByIdAsync("1"))
                .ReturnsAsync(image);

            _mockImageService
                .Setup(service => service.DeleteImageAsync("1"))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.Delete("prop1", "1");

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion

        #region Toggle Tests

        [Test]
        public async Task Toggle_ReturnsNoContent_WhenImageToggled()
        {
            // Arrange
            var image = CreateTestImageDto("1", "prop1");
            var toggleDto = new TogglePropertyImageDto { Enabled = false };

            _mockImageService!
                .Setup(service => service.GetImageByIdAsync("1"))
                .ReturnsAsync(image);

            _mockImageService
                .Setup(service => service.ToggleImageAsync("1", false))
                .ReturnsAsync(true);

            // Act
            var result = await _controller!.Toggle("prop1", "1", toggleDto);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.That(noContentResult, Is.Not.Null);
            Assert.That(noContentResult!.StatusCode, Is.EqualTo(204));

            _mockImageService.Verify(service => service.ToggleImageAsync("1", false), Times.Once);
        }

        [Test]
        public async Task Toggle_ReturnsNotFound_WhenImageDoesNotExist()
        {
            // Arrange
            var toggleDto = new TogglePropertyImageDto { Enabled = true };

            _mockImageService!
                .Setup(service => service.GetImageByIdAsync("999"))
                .ReturnsAsync((PropertyImageDto?)null);

            // Act
            var result = await _controller!.Toggle("prop1", "999", toggleDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult!.StatusCode, Is.EqualTo(404));

            _mockImageService.Verify(service => service.ToggleImageAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public async Task Toggle_ReturnsInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var image = CreateTestImageDto("1", "prop1");
            var toggleDto = new TogglePropertyImageDto { Enabled = true };

            _mockImageService!
                .Setup(service => service.GetImageByIdAsync("1"))
                .ReturnsAsync(image);

            _mockImageService
                .Setup(service => service.ToggleImageAsync("1", true))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller!.Toggle("prop1", "1", toggleDto);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.That(statusCodeResult, Is.Not.Null);
            Assert.That(statusCodeResult!.StatusCode, Is.EqualTo(500));
        }

        #endregion
    }
}
