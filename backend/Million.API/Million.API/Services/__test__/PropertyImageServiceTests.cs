using Million.API.Domain;
using Million.API.DTOs;
using Million.API.Repository;
using Moq;
using NUnit.Framework;

namespace Million.API.Services.Tests
{
    /// <summary>
    /// Unit tests for PropertyImageService using NUnit and Moq
    /// </summary>
    [TestFixture]
    public class PropertyImageServiceTests
    {
        private Mock<IPropertyImageRepository>? _mockImageRepository;
        private Mock<IPropertyRepository>? _mockPropertyRepository;
        private IPropertyImageService? _imageService;

        [SetUp]
        public void Setup()
        {
            _mockImageRepository = new Mock<IPropertyImageRepository>();
            _mockPropertyRepository = new Mock<IPropertyRepository>();
            _imageService = new PropertyImageService(
                _mockImageRepository.Object,
                _mockPropertyRepository.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _mockImageRepository = null;
            _mockPropertyRepository = null;
            _imageService = null;
        }

        #region GetImagesByPropertyIdAsync Tests

        [Test]
        public async Task GetImagesByPropertyIdAsync_ReturnsImages()
        {
            // Arrange
            var images = new List<PropertyImage>
            {
                new PropertyImage
                {
                    IdPropertyImage = "1",
                    IdProperty = "prop1",
                    File = "image1.jpg",
                    Enabled = true
                },
                new PropertyImage
                {
                    IdPropertyImage = "2",
                    IdProperty = "prop1",
                    File = "image2.jpg",
                    Enabled = true
                }
            };

            _mockImageRepository
                .Setup(repo => repo.GetByPropertyIdAsync("prop1"))
                .ReturnsAsync(images);

            // Act
            var result = await _imageService!.GetImagesByPropertyIdAsync("prop1");

            // Assert
            var imageList = result.ToList();
            Assert.That(imageList, Has.Count.EqualTo(2));
            Assert.That(imageList[0].File, Is.EqualTo("image1.jpg"));
            Assert.That(imageList[1].File, Is.EqualTo("image2.jpg"));
        }

        [Test]
        public async Task GetImagesByPropertyIdAsync_ReturnsEmpty_WhenNoImages()
        {
            // Arrange
            _mockImageRepository
                .Setup(repo => repo.GetByPropertyIdAsync("prop1"))
                .ReturnsAsync(new List<PropertyImage>());

            // Act
            var result = await _imageService!.GetImagesByPropertyIdAsync("prop1");

            // Assert
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region GetImagesByPropertyIdPaginatedAsync Tests

        [Test]
        public async Task GetImagesByPropertyIdPaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            var images = new List<PropertyImage>
            {
                new PropertyImage
                {
                    IdPropertyImage = "1",
                    IdProperty = "prop1",
                    File = "image1.jpg",
                    Enabled = true
                }
            };

            var paginationDto = new PaginationRequestDto { PageNumber = 1, PageSize = 10 };

            _mockImageRepository
                .Setup(repo => repo.GetByPropertyIdPaginatedAsync("prop1", 1, 10))
                .ReturnsAsync((images, 1L));

            // Act
            var result = await _imageService!.GetImagesByPropertyIdPaginatedAsync("prop1", paginationDto);

            // Assert
            Assert.That(result.TotalRecords, Is.EqualTo(1));
            Assert.That(result.Data.Count(), Is.EqualTo(1));
        }

        #endregion

        #region GetImageByIdAsync Tests

        [Test]
        public async Task GetImageByIdAsync_ReturnsImage_WhenImageExists()
        {
            // Arrange
            var image = new PropertyImage
            {
                IdPropertyImage = "1",
                IdProperty = "prop1",
                File = "image1.jpg",
                Enabled = true
            };

            _mockImageRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(image);

            // Act
            var result = await _imageService!.GetImageByIdAsync("1");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.File, Is.EqualTo("image1.jpg"));
        }

        [Test]
        public async Task GetImageByIdAsync_ReturnsNull_WhenImageNotFound()
        {
            // Arrange
            _mockImageRepository
                .Setup(repo => repo.GetByIdAsync("999"))
                .ReturnsAsync((PropertyImage?)null);

            // Act
            var result = await _imageService!.GetImageByIdAsync("999");

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region AddImageAsync Tests

        [Test]
        public async Task AddImageAsync_AddsNewImage()
        {
            // Arrange
            var createDto = new CreatePropertyImageDto
            {
                IdProperty = "prop1",
                File = "new-image.jpg",
                Enabled = true
            };

            var property = new Property
            {
                IdProperty = "prop1",
                Name = "Property",
                Address = "Address",
                Price = 100000,
                CodeInternal = "CODE001",
                Year = 2020,
                IdOwner = "owner1"
            };

            _mockPropertyRepository
                .Setup(repo => repo.GetByIdAsync("prop1"))
                .ReturnsAsync(property);

            _mockImageRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<PropertyImage>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _imageService!.AddImageAsync(createDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.File, Is.EqualTo("new-image.jpg"));
            Assert.That(result.IdProperty, Is.EqualTo("prop1"));
            
            _mockImageRepository.Verify(repo => repo.CreateAsync(It.IsAny<PropertyImage>()), Times.Once);
        }

        [Test]
        public void AddImageAsync_ThrowsException_WhenPropertyNotFound()
        {
            // Arrange
            var createDto = new CreatePropertyImageDto
            {
                IdProperty = "invalid",
                File = "image.jpg",
                Enabled = true
            };

            _mockPropertyRepository
                .Setup(repo => repo.GetByIdAsync("invalid"))
                .ReturnsAsync((Property?)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _imageService!.AddImageAsync(createDto)
            );

            Assert.That(exception!.Message, Does.Contain("not found"));
            _mockImageRepository.Verify(repo => repo.CreateAsync(It.IsAny<PropertyImage>()), Times.Never);
        }

        #endregion

        #region AddImagesBulkAsync Tests

        [Test]
        public async Task AddImagesBulkAsync_AddsMultipleImages()
        {
            // Arrange
            var imageDtos = new List<CreatePropertyImageDto>
            {
                new CreatePropertyImageDto { IdProperty = "prop1", File = "image1.jpg", Enabled = true },
                new CreatePropertyImageDto { IdProperty = "prop1", File = "image2.jpg", Enabled = true }
            };

            var property = new Property
            {
                IdProperty = "prop1",
                Name = "Property",
                Address = "Address",
                Price = 100000,
                CodeInternal = "CODE001",
                Year = 2020,
                IdOwner = "owner1"
            };

            _mockPropertyRepository
                .Setup(repo => repo.GetByIdAsync("prop1"))
                .ReturnsAsync(property);

            _mockImageRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<PropertyImage>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _imageService!.AddImagesBulkAsync("prop1", imageDtos);

            // Assert
            var imageList = result.ToList();
            Assert.That(imageList, Has.Count.EqualTo(2));
            _mockImageRepository.Verify(repo => repo.CreateAsync(It.IsAny<PropertyImage>()), Times.Exactly(2));
        }

        [Test]
        public void AddImagesBulkAsync_ThrowsException_WhenPropertyNotFound()
        {
            // Arrange
            var imageDtos = new List<CreatePropertyImageDto>
            {
                new CreatePropertyImageDto { IdProperty = "invalid", File = "image.jpg", Enabled = true }
            };

            _mockPropertyRepository
                .Setup(repo => repo.GetByIdAsync("invalid"))
                .ReturnsAsync((Property?)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _imageService!.AddImagesBulkAsync("invalid", imageDtos)
            );

            Assert.That(exception!.Message, Does.Contain("not found"));
        }

        #endregion

        #region UpdateImageAsync Tests

        [Test]
        public async Task UpdateImageAsync_UpdatesImage_WhenImageExists()
        {
            // Arrange
            var existingImage = new PropertyImage
            {
                IdPropertyImage = "1",
                IdProperty = "prop1",
                File = "old-image.jpg",
                Enabled = true
            };

            var updateDto = new UpdatePropertyImageDto
            {
                File = "updated-image.jpg",
                Enabled = false
            };

            _mockImageRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(existingImage);

            _mockImageRepository
                .Setup(repo => repo.UpdateAsync("1", It.IsAny<PropertyImage>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _imageService!.UpdateImageAsync("1", updateDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.File, Is.EqualTo("updated-image.jpg"));
            Assert.That(result.Enabled, Is.False);
            
            _mockImageRepository.Verify(repo => repo.UpdateAsync("1", It.IsAny<PropertyImage>()), Times.Once);
        }

        [Test]
        public async Task UpdateImageAsync_ReturnsNull_WhenImageNotFound()
        {
            // Arrange
            var updateDto = new UpdatePropertyImageDto
            {
                File = "image.jpg",
                Enabled = true
            };

            _mockImageRepository
                .Setup(repo => repo.GetByIdAsync("999"))
                .ReturnsAsync((PropertyImage?)null);

            // Act
            var result = await _imageService!.UpdateImageAsync("999", updateDto);

            // Assert
            Assert.That(result, Is.Null);
            _mockImageRepository.Verify(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<PropertyImage>()), Times.Never);
        }

        #endregion

        #region DeleteImageAsync Tests

        [Test]
        public async Task DeleteImageAsync_ReturnsTrue_WhenImageDeleted()
        {
            // Arrange
            var image = new PropertyImage
            {
                IdPropertyImage = "1",
                IdProperty = "prop1",
                File = "image.jpg",
                Enabled = true
            };

            _mockImageRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(image);

            _mockImageRepository
                .Setup(repo => repo.DeleteAsync("1"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _imageService!.DeleteImageAsync("1");

            // Assert
            Assert.That(result, Is.True);
            _mockImageRepository.Verify(repo => repo.DeleteAsync("1"), Times.Once);
        }

        [Test]
        public async Task DeleteImageAsync_ReturnsFalse_WhenImageNotFound()
        {
            // Arrange
            _mockImageRepository
                .Setup(repo => repo.GetByIdAsync("999"))
                .ReturnsAsync((PropertyImage?)null);

            // Act
            var result = await _imageService!.DeleteImageAsync("999");

            // Assert
            Assert.That(result, Is.False);
            _mockImageRepository.Verify(repo => repo.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region ToggleImageAsync Tests

        [Test]
        public async Task ToggleImageAsync_TogglesImageStatus()
        {
            // Arrange
            var image = new PropertyImage
            {
                IdPropertyImage = "1",
                IdProperty = "prop1",
                File = "image.jpg",
                Enabled = true
            };

            _mockImageRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(image);

            _mockImageRepository
                .Setup(repo => repo.UpdateAsync("1", It.IsAny<PropertyImage>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _imageService!.ToggleImageAsync("1", false);

            // Assert
            Assert.That(result, Is.True);
            _mockImageRepository.Verify(repo => repo.UpdateAsync("1", It.Is<PropertyImage>(img => img.Enabled == false)), Times.Once);
        }

        [Test]
        public async Task ToggleImageAsync_ReturnsFalse_WhenImageNotFound()
        {
            // Arrange
            _mockImageRepository
                .Setup(repo => repo.GetByIdAsync("999"))
                .ReturnsAsync((PropertyImage?)null);

            // Act
            var result = await _imageService!.ToggleImageAsync("999", true);

            // Assert
            Assert.That(result, Is.False);
            _mockImageRepository.Verify(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<PropertyImage>()), Times.Never);
        }

        #endregion
    }
}
