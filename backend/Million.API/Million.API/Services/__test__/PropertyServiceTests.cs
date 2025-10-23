using Million.API.Domain;
using Million.API.DTOs;
using Million.API.Repository;
using Moq;
using NUnit.Framework;

namespace Million.API.Services.Tests
{
    /// <summary>
    /// Unit tests for PropertyService using NUnit and Moq
    /// </summary>
    [TestFixture]
    public class PropertyServiceTests
    {
        private Mock<IPropertyRepository>? _mockPropertyRepository;
        private Mock<IPropertyImageRepository>? _mockImageRepository;
        private Mock<IOwnerRepository>? _mockOwnerRepository;
        private IPropertyService? _propertyService;

        [SetUp]
        public void Setup()
        {
            _mockPropertyRepository = new Mock<IPropertyRepository>();
            _mockImageRepository = new Mock<IPropertyImageRepository>();
            _mockOwnerRepository = new Mock<IOwnerRepository>();
            _propertyService = new PropertyService(
                _mockPropertyRepository.Object,
                _mockImageRepository.Object,
                _mockOwnerRepository.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _mockPropertyRepository = null;
            _mockImageRepository = null;
            _mockOwnerRepository = null;
            _propertyService = null;
        }

        #region SearchPropertiesAsync Tests

        [Test]
        public async Task SearchPropertiesAsync_ReturnsProperties()
        {
            // Arrange
            var properties = new List<Property>
            {
                new Property
                {
                    IdProperty = "1",
                    Name = "Modern House",
                    Address = "123 Main St",
                    Price = 250000,
                    CodeInternal = "CODE001",
                    Year = 2020,
                    IdOwner = "owner1"
                }
            };

            var owner = new Owner
            {
                IdOwner = "owner1",
                Name = "John Doe",
                Address = "Address",
                Birthday = DateTime.Now
            };

            var filterDto = new PropertyFilterDto
            {
                Name = "Modern"
            };

            _mockPropertyRepository
                .Setup(repo => repo.SearchPropertiesAsync(It.IsAny<string>(), null, null, null))
                .ReturnsAsync(properties);

            _mockImageRepository
                .Setup(repo => repo.GetByPropertyIdAsync("1"))
                .ReturnsAsync(new List<PropertyImage>());

            _mockOwnerRepository
                .Setup(repo => repo.GetByIdAsync("owner1"))
                .ReturnsAsync(owner);

            // Act
            var result = await _propertyService!.SearchPropertiesAsync(filterDto);

            // Assert
            var propertyList = result.ToList();
            Assert.That(propertyList, Has.Count.EqualTo(1));
            Assert.That(propertyList[0].Name, Is.EqualTo("Modern House"));
            Assert.That(propertyList[0].OwnerName, Is.EqualTo("John Doe"));
        }

        #endregion

        #region SearchPropertiesPaginatedAsync Tests

        [Test]
        public async Task SearchPropertiesPaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            var properties = new List<Property>
            {
                new Property
                {
                    IdProperty = "1",
                    Name = "Property 1",
                    Address = "Address 1",
                    Price = 100000,
                    CodeInternal = "CODE001",
                    Year = 2020,
                    IdOwner = "owner1"
                }
            };

            var owner = new Owner { IdOwner = "owner1", Name = "Owner", Address = "Address", Birthday = DateTime.Now };

            var filterDto = new PropertyFilterDto();
            var paginationDto = new PaginationRequestDto { PageNumber = 1, PageSize = 10 };

            _mockPropertyRepository
                .Setup(repo => repo.SearchPropertiesPaginatedAsync(null, null, null, null, 1, 10))
                .ReturnsAsync((properties, 1L));

            _mockImageRepository
                .Setup(repo => repo.GetByPropertyIdAsync("1"))
                .ReturnsAsync(new List<PropertyImage>());

            _mockOwnerRepository
                .Setup(repo => repo.GetByIdAsync("owner1"))
                .ReturnsAsync(owner);

            // Act
            var result = await _propertyService!.SearchPropertiesPaginatedAsync(filterDto, paginationDto);

            // Assert
            Assert.That(result.TotalRecords, Is.EqualTo(1));
            Assert.That(result.Data.Count(), Is.EqualTo(1));
        }

        #endregion

        #region GetPropertyByIdAsync Tests

        [Test]
        public async Task GetPropertyByIdAsync_ReturnsPropertyDetail_WhenPropertyExists()
        {
            // Arrange
            var property = new Property
            {
                IdProperty = "1",
                Name = "Test Property",
                Address = "123 Test St",
                Price = 200000,
                CodeInternal = "TEST001",
                Year = 2021,
                IdOwner = "owner1"
            };

            var images = new List<PropertyImage>
            {
                new PropertyImage
                {
                    IdPropertyImage = "img1",
                    IdProperty = "1",
                    File = "image1.jpg",
                    Enabled = true
                }
            };

            _mockPropertyRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(property);

            _mockImageRepository
                .Setup(repo => repo.GetByPropertyIdAsync("1"))
                .ReturnsAsync(images);

            // Act
            var result = await _propertyService!.GetPropertyByIdAsync("1");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Test Property"));
            Assert.That(result.Images.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetPropertyByIdAsync_ReturnsNull_WhenPropertyNotFound()
        {
            // Arrange
            _mockPropertyRepository
                .Setup(repo => repo.GetByIdAsync("999"))
                .ReturnsAsync((Property?)null);

            // Act
            var result = await _propertyService!.GetPropertyByIdAsync("999");

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region CreatePropertyAsync Tests

        [Test]
        public async Task CreatePropertyAsync_CreatesNewProperty()
        {
            // Arrange
            var createDto = new CreatePropertyDto
            {
                Name = "New Property",
                Address = "789 New St",
                Price = 300000,
                CodeInternal = "NEW001",
                Year = 2023,
                IdOwner = "owner1"
            };

            var owner = new Owner
            {
                IdOwner = "owner1",
                Name = "Owner Name",
                Address = "Address",
                Birthday = DateTime.Now
            };

            _mockPropertyRepository
                .Setup(repo => repo.ExistsByCodeAsync("NEW001"))
                .ReturnsAsync(false);

            _mockPropertyRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<Property>()))
                .Returns(Task.CompletedTask);

            _mockOwnerRepository
                .Setup(repo => repo.GetByIdAsync("owner1"))
                .ReturnsAsync(owner);

            // Act
            var result = await _propertyService!.CreatePropertyAsync(createDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("New Property"));
            Assert.That(result.OwnerName, Is.EqualTo("Owner Name"));
            
            _mockPropertyRepository.Verify(repo => repo.CreateAsync(It.IsAny<Property>()), Times.Once);
        }

        [Test]
        public void CreatePropertyAsync_ThrowsException_WhenDuplicateCode()
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

            _mockPropertyRepository
                .Setup(repo => repo.ExistsByCodeAsync("DUP001"))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _propertyService!.CreatePropertyAsync(createDto)
            );

            Assert.That(exception!.Message, Does.Contain("already exists"));
            _mockPropertyRepository.Verify(repo => repo.CreateAsync(It.IsAny<Property>()), Times.Never);
        }

        #endregion

        #region UpdatePropertyAsync Tests

        [Test]
        public async Task UpdatePropertyAsync_ReturnsTrue_WhenPropertyUpdated()
        {
            // Arrange
            var existingProperty = new Property
            {
                IdProperty = "1",
                Name = "Old Name",
                Address = "Old Address",
                Price = 100000,
                CodeInternal = "CODE001",
                Year = 2020,
                IdOwner = "owner1"
            };

            var updateDto = new UpdatePropertyDto
            {
                Name = "Updated Name",
                Address = "Updated Address",
                Price = 150000,
                CodeInternal = "CODE001",
                Year = 2021,
                IdOwner = "owner1"
            };

            _mockPropertyRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(existingProperty);

            _mockPropertyRepository
                .Setup(repo => repo.UpdateAsync("1", It.IsAny<Property>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _propertyService!.UpdatePropertyAsync("1", updateDto);

            // Assert
            Assert.That(result, Is.True);
            _mockPropertyRepository.Verify(repo => repo.UpdateAsync("1", It.IsAny<Property>()), Times.Once);
        }

        [Test]
        public async Task UpdatePropertyAsync_ReturnsFalse_WhenPropertyNotFound()
        {
            // Arrange
            var updateDto = new UpdatePropertyDto
            {
                Name = "Updated",
                Address = "Address",
                Price = 100000,
                CodeInternal = "CODE",
                Year = 2020,
                IdOwner = "owner1"
            };

            _mockPropertyRepository
                .Setup(repo => repo.GetByIdAsync("999"))
                .ReturnsAsync((Property?)null);

            // Act
            var result = await _propertyService!.UpdatePropertyAsync("999", updateDto);

            // Assert
            Assert.That(result, Is.False);
            _mockPropertyRepository.Verify(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<Property>()), Times.Never);
        }

        #endregion

        #region DeletePropertyAsync Tests

        [Test]
        public async Task DeletePropertyAsync_ReturnsTrue_WhenPropertyDeleted()
        {
            // Arrange
            var property = new Property
            {
                IdProperty = "1",
                Name = "Property",
                Address = "Address",
                Price = 100000,
                CodeInternal = "CODE001",
                Year = 2020,
                IdOwner = "owner1"
            };

            _mockPropertyRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(property);

            _mockPropertyRepository
                .Setup(repo => repo.DeleteAsync("1"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _propertyService!.DeletePropertyAsync("1");

            // Assert
            Assert.That(result, Is.True);
            _mockPropertyRepository.Verify(repo => repo.DeleteAsync("1"), Times.Once);
        }

        [Test]
        public async Task DeletePropertyAsync_ReturnsFalse_WhenPropertyNotFound()
        {
            // Arrange
            _mockPropertyRepository
                .Setup(repo => repo.GetByIdAsync("999"))
                .ReturnsAsync((Property?)null);

            // Act
            var result = await _propertyService!.DeletePropertyAsync("999");

            // Assert
            Assert.That(result, Is.False);
            _mockPropertyRepository.Verify(repo => repo.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region GetPropertiesByOwnerAsync Tests

        [Test]
        public async Task GetPropertiesByOwnerAsync_ReturnsPropertiesForOwner()
        {
            // Arrange
            var properties = new List<Property>
            {
                new Property
                {
                    IdProperty = "1",
                    Name = "Property 1",
                    Address = "Address 1",
                    Price = 100000,
                    CodeInternal = "CODE001",
                    Year = 2020,
                    IdOwner = "owner1"
                },
                new Property
                {
                    IdProperty = "2",
                    Name = "Property 2",
                    Address = "Address 2",
                    Price = 200000,
                    CodeInternal = "CODE002",
                    Year = 2021,
                    IdOwner = "owner1"
                }
            };

            var owner = new Owner
            {
                IdOwner = "owner1",
                Name = "Owner",
                Address = "Address",
                Birthday = DateTime.Now
            };

            _mockPropertyRepository
                .Setup(repo => repo.GetByOwnerIdAsync("owner1"))
                .ReturnsAsync(properties);

            _mockImageRepository
                .Setup(repo => repo.GetByPropertyIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<PropertyImage>());

            _mockOwnerRepository
                .Setup(repo => repo.GetByIdAsync("owner1"))
                .ReturnsAsync(owner);

            // Act
            var result = await _propertyService!.GetPropertiesByOwnerAsync("owner1");

            // Assert
            var propertyList = result.ToList();
            Assert.That(propertyList, Has.Count.EqualTo(2));
            Assert.That(propertyList.All(p => p.IdOwner == "owner1"), Is.True);
        }

        #endregion

        #region GetPropertiesByOwnerPaginatedAsync Tests

        [Test]
        public async Task GetPropertiesByOwnerPaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            var properties = new List<Property>
            {
                new Property
                {
                    IdProperty = "1",
                    Name = "Property 1",
                    Address = "Address 1",
                    Price = 100000,
                    CodeInternal = "CODE001",
                    Year = 2020,
                    IdOwner = "owner1"
                }
            };

            var owner = new Owner { IdOwner = "owner1", Name = "Owner", Address = "Address", Birthday = DateTime.Now };

            var paginationDto = new PaginationRequestDto { PageNumber = 1, PageSize = 10 };

            _mockPropertyRepository
                .Setup(repo => repo.GetByOwnerIdPaginatedAsync("owner1", 1, 10))
                .ReturnsAsync((properties, 1L));

            _mockImageRepository
                .Setup(repo => repo.GetByPropertyIdAsync("1"))
                .ReturnsAsync(new List<PropertyImage>());

            _mockOwnerRepository
                .Setup(repo => repo.GetByIdAsync("owner1"))
                .ReturnsAsync(owner);

            // Act
            var result = await _propertyService!.GetPropertiesByOwnerPaginatedAsync("owner1", paginationDto);

            // Assert
            Assert.That(result.TotalRecords, Is.EqualTo(1));
            Assert.That(result.Data.Count(), Is.EqualTo(1));
        }

        #endregion
    }
}
