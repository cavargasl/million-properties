using Million.API.Domain;
using Million.API.DTOs;
using Million.API.Repository;
using Moq;
using NUnit.Framework;

namespace Million.API.Services.Tests
{
    /// <summary>
    /// Unit tests for OwnerService using NUnit and Moq
    /// </summary>
    [TestFixture]
    public class OwnerServiceTests
    {
        private Mock<IOwnerRepository>? _mockOwnerRepository;
        private IOwnerService? _ownerService;

        [SetUp]
        public void Setup()
        {
            // Create a mock of IOwnerRepository
            _mockOwnerRepository = new Mock<IOwnerRepository>();
            _ownerService = new OwnerService(_mockOwnerRepository!.Object);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            _mockOwnerRepository = null;
            _ownerService = null;
        }

        #region GetAllOwnersAsync Tests

        [Test]
        public async Task GetAllOwnersAsync_ReturnsListOfOwners()
        {
            // Arrange
            var owners = new List<Owner>
            {
                new Owner
                {
                    IdOwner = "1",
                    Name = "Owner 1",
                    Address = "Address 1",
                    Photo = null,
                    Birthday = new DateTime(1980, 1, 1)
                },
                new Owner
                {
                    IdOwner = "2",
                    Name = "Owner 2",
                    Address = "Address 2",
                    Photo = null,
                    Birthday = new DateTime(1990, 5, 15)
                }
            };

            _mockOwnerRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(owners);

            // Act
            var result = await _ownerService!.GetAllOwnersAsync();

            // Assert
            var ownersList = result.ToList();
            Assert.That(ownersList, Has.Count.EqualTo(2));
            Assert.That(ownersList[0].Name, Is.EqualTo("Owner 1"));
            Assert.That(ownersList[1].Name, Is.EqualTo("Owner 2"));
            
            _mockOwnerRepository!.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllOwnersAsync_ReturnsEmptyList_WhenNoOwnersExist()
        {
            // Arrange
            _mockOwnerRepository
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Owner>());

            // Act
            var result = await _ownerService!.GetAllOwnersAsync();

            // Assert
            Assert.That(result, Is.Empty);
            _mockOwnerRepository!.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        #endregion

        #region GetAllOwnersPaginatedAsync Tests

        [Test]
        public async Task GetAllOwnersPaginatedAsync_ReturnsPaginatedResult()
        {
            // Arrange
            var owners = new List<Owner>
            {
                new Owner
                {
                    IdOwner = "1",
                    Name = "Owner 1",
                    Address = "Address 1",
                    Photo = null,
                    Birthday = new DateTime(1980, 1, 1)
                }
            };

            var paginationDto = new PaginationRequestDto
            {
                PageNumber = 1,
                PageSize = 10
            };

            _mockOwnerRepository
                .Setup(repo => repo.GetPaginatedAsync(1, 10))
                .ReturnsAsync((owners, 1L));

            // Act
            var result = await _ownerService!.GetAllOwnersPaginatedAsync(paginationDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.PageSize, Is.EqualTo(10));
            Assert.That(result.TotalRecords, Is.EqualTo(1));
            Assert.That(result.Data.Count(), Is.EqualTo(1));
            
            _mockOwnerRepository!.Verify(repo => repo.GetPaginatedAsync(1, 10), Times.Once);
        }

        #endregion

        #region GetOwnerByIdAsync Tests

        [Test]
        public async Task GetOwnerByIdAsync_ReturnsOwner_WhenOwnerExists()
        {
            // Arrange
            var owner = new Owner
            {
                IdOwner = "1",
                Name = "Test Owner",
                Address = "123 Test St",
                Photo = "https://example.com/photo.jpg",
                Birthday = new DateTime(1980, 1, 1)
            };

            _mockOwnerRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(owner);

            // Act
            var result = await _ownerService!.GetOwnerByIdAsync("1");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IdOwner, Is.EqualTo("1"));
            Assert.That(result.Name, Is.EqualTo("Test Owner"));
            
            _mockOwnerRepository!.Verify(repo => repo.GetByIdAsync("1"), Times.Once);
        }

        [Test]
        public async Task GetOwnerByIdAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _mockOwnerRepository
                .Setup(repo => repo.GetByIdAsync("999"))
                .ReturnsAsync((Owner?)null);

            // Act
            var result = await _ownerService!.GetOwnerByIdAsync("999");

            // Assert
            Assert.That(result, Is.Null);
            _mockOwnerRepository!.Verify(repo => repo.GetByIdAsync("999"), Times.Once);
        }

        #endregion

        #region CreateOwnerAsync Tests

        [Test]
        public async Task CreateOwnerAsync_AddsNewOwner()
        {
            // Arrange
            var createDto = new CreateOwnerDto
            {
                Name = "New Owner",
                Address = "789 New St",
                Photo = "https://example.com/photo.jpg",
                Birthday = new DateTime(1985, 3, 10)
            };

            _mockOwnerRepository
                .Setup(repo => repo.ExistsByNameAsync("New Owner"))
                .ReturnsAsync(false);

            _mockOwnerRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<Owner>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _ownerService!.CreateOwnerAsync(createDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("New Owner"));
            Assert.That(result.Address, Is.EqualTo("789 New St"));
            
            _mockOwnerRepository!.Verify(repo => repo.ExistsByNameAsync("New Owner"), Times.Once);
            _mockOwnerRepository!.Verify(repo => repo.CreateAsync(It.IsAny<Owner>()), Times.Once);
        }

        [Test]
        public void CreateOwnerAsync_ThrowsException_WhenDuplicateName()
        {
            // Arrange
            var createDto = new CreateOwnerDto
            {
                Name = "Duplicate Owner",
                Address = "789 St",
                Photo = null,
                Birthday = new DateTime(1985, 3, 10)
            };

            _mockOwnerRepository
                .Setup(repo => repo.ExistsByNameAsync("Duplicate Owner"))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _ownerService!.CreateOwnerAsync(createDto)
            );

            Assert.That(exception!.Message, Is.EqualTo("Owner with name 'Duplicate Owner' already exists"));
            _mockOwnerRepository!.Verify(repo => repo.ExistsByNameAsync("Duplicate Owner"), Times.Once);
            _mockOwnerRepository!.Verify(repo => repo.CreateAsync(It.IsAny<Owner>()), Times.Never);
        }

        #endregion

        #region UpdateOwnerAsync Tests

        [Test]
        public async Task UpdateOwnerAsync_ReturnsUpdatedOwner_WhenOwnerExists()
        {
            // Arrange
            var existingOwner = new Owner
            {
                IdOwner = "1",
                Name = "Old Name",
                Address = "Old Address",
                Photo = null,
                Birthday = new DateTime(1980, 1, 1)
            };

            var updateDto = new UpdateOwnerDto
            {
                Name = "Updated Name",
                Address = "Updated Address",
                Photo = "https://example.com/new.jpg",
                Birthday = new DateTime(1985, 5, 15)
            };

            _mockOwnerRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(existingOwner);

            _mockOwnerRepository
                .Setup(repo => repo.UpdateAsync("1", It.IsAny<Owner>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _ownerService!.UpdateOwnerAsync("1", updateDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Updated Name"));
            Assert.That(result.Address, Is.EqualTo("Updated Address"));
            
            _mockOwnerRepository!.Verify(repo => repo.GetByIdAsync("1"), Times.Once);
            _mockOwnerRepository!.Verify(repo => repo.UpdateAsync("1", It.IsAny<Owner>()), Times.Once);
        }

        [Test]
        public async Task UpdateOwnerAsync_ReturnsNull_WhenOwnerNotFound()
        {
            // Arrange
            var updateDto = new UpdateOwnerDto
            {
                Name = "Updated Name",
                Address = "Updated Address",
                Photo = null,
                Birthday = new DateTime(1985, 5, 15)
            };

            _mockOwnerRepository
                .Setup(repo => repo.GetByIdAsync("999"))
                .ReturnsAsync((Owner?)null);

            // Act
            var result = await _ownerService!.UpdateOwnerAsync("999", updateDto);

            // Assert
            Assert.That(result, Is.Null);
            _mockOwnerRepository!.Verify(repo => repo.GetByIdAsync("999"), Times.Once);
            _mockOwnerRepository!.Verify(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<Owner>()), Times.Never);
        }

        #endregion

        #region DeleteOwnerAsync Tests

        [Test]
        public async Task DeleteOwnerAsync_ReturnsTrue_WhenOwnerDeleted()
        {
            // Arrange
            var owner = new Owner
            {
                IdOwner = "1",
                Name = "Test Owner",
                Address = "123 St",
                Photo = null,
                Birthday = new DateTime(1980, 1, 1)
            };

            _mockOwnerRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(owner);

            _mockOwnerRepository
                .Setup(repo => repo.DeleteAsync("1"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _ownerService!.DeleteOwnerAsync("1");

            // Assert
            Assert.That(result, Is.True);
            _mockOwnerRepository!.Verify(repo => repo.GetByIdAsync("1"), Times.Once);
            _mockOwnerRepository!.Verify(repo => repo.DeleteAsync("1"), Times.Once);
        }

        [Test]
        public async Task DeleteOwnerAsync_ReturnsFalse_WhenOwnerNotFound()
        {
            // Arrange
            _mockOwnerRepository
                .Setup(repo => repo.GetByIdAsync("999"))
                .ReturnsAsync((Owner?)null);

            // Act
            var result = await _ownerService!.DeleteOwnerAsync("999");

            // Assert
            Assert.That(result, Is.False);
            _mockOwnerRepository!.Verify(repo => repo.GetByIdAsync("999"), Times.Once);
            _mockOwnerRepository!.Verify(repo => repo.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region SearchByNameAsync Tests

        [Test]
        public async Task SearchByNameAsync_ReturnsMatchingOwners()
        {
            // Arrange
            var owners = new List<Owner>
            {
                new Owner
                {
                    IdOwner = "1",
                    Name = "John Doe",
                    Address = "123 St",
                    Photo = null,
                    Birthday = new DateTime(1980, 1, 1)
                },
                new Owner
                {
                    IdOwner = "2",
                    Name = "John Smith",
                    Address = "456 Ave",
                    Photo = null,
                    Birthday = new DateTime(1990, 5, 15)
                }
            };

            _mockOwnerRepository
                .Setup(repo => repo.FindByNameAsync("John"))
                .ReturnsAsync(owners);

            // Act
            var result = await _ownerService!.SearchByNameAsync("John");

            // Assert
            var ownersList = result.ToList();
            Assert.That(ownersList, Has.Count.EqualTo(2));
            Assert.That(ownersList.All(o => o.Name.Contains("John")), Is.True);
            
            _mockOwnerRepository!.Verify(repo => repo.FindByNameAsync("John"), Times.Once);
        }

        [Test]
        public async Task SearchByNameAsync_ReturnsEmpty_WhenNoMatches()
        {
            // Arrange
            _mockOwnerRepository
                .Setup(repo => repo.FindByNameAsync("NonExistent"))
                .ReturnsAsync(new List<Owner>());

            // Act
            var result = await _ownerService!.SearchByNameAsync("NonExistent");

            // Assert
            Assert.That(result, Is.Empty);
            _mockOwnerRepository!.Verify(repo => repo.FindByNameAsync("NonExistent"), Times.Once);
        }

        #endregion

        #region SearchByNamePaginatedAsync Tests

        [Test]
        public async Task SearchByNamePaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            var owners = new List<Owner>
            {
                new Owner
                {
                    IdOwner = "1",
                    Name = "John Doe",
                    Address = "123 St",
                    Photo = null,
                    Birthday = new DateTime(1980, 1, 1)
                }
            };

            var paginationDto = new PaginationRequestDto
            {
                PageNumber = 1,
                PageSize = 10
            };

            _mockOwnerRepository
                .Setup(repo => repo.FindByNamePaginatedAsync("John", 1, 10))
                .ReturnsAsync((owners, 1L));

            // Act
            var result = await _ownerService!.SearchByNamePaginatedAsync("John", paginationDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalRecords, Is.EqualTo(1));
            Assert.That(result.Data.Count(), Is.EqualTo(1));
            
            _mockOwnerRepository!.Verify(repo => repo.FindByNamePaginatedAsync("John", 1, 10), Times.Once);
        }

        #endregion

        #region SearchByAddressAsync Tests

        [Test]
        public async Task SearchByAddressAsync_ReturnsMatchingOwners()
        {
            // Arrange
            var owners = new List<Owner>
            {
                new Owner
                {
                    IdOwner = "1",
                    Name = "John Doe",
                    Address = "123 Main Street",
                    Photo = null,
                    Birthday = new DateTime(1980, 1, 1)
                }
            };

            _mockOwnerRepository
                .Setup(repo => repo.FindByAddressAsync("Main"))
                .ReturnsAsync(owners);

            // Act
            var result = await _ownerService!.SearchByAddressAsync("Main");

            // Assert
            var ownersList = result.ToList();
            Assert.That(ownersList, Has.Count.EqualTo(1));
            Assert.That(ownersList[0].Address, Does.Contain("Main"));
            
            _mockOwnerRepository!.Verify(repo => repo.FindByAddressAsync("Main"), Times.Once);
        }

        #endregion

        #region SearchByAddressPaginatedAsync Tests

        [Test]
        public async Task SearchByAddressPaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            var owners = new List<Owner>
            {
                new Owner
                {
                    IdOwner = "1",
                    Name = "John Doe",
                    Address = "123 Main Street",
                    Photo = null,
                    Birthday = new DateTime(1980, 1, 1)
                }
            };

            var paginationDto = new PaginationRequestDto
            {
                PageNumber = 1,
                PageSize = 10
            };

            _mockOwnerRepository
                .Setup(repo => repo.FindByAddressPaginatedAsync("Main", 1, 10))
                .ReturnsAsync((owners, 1L));

            // Act
            var result = await _ownerService!.SearchByAddressPaginatedAsync("Main", paginationDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalRecords, Is.EqualTo(1));
            Assert.That(result.Data.Count(), Is.EqualTo(1));
            
            _mockOwnerRepository!.Verify(repo => repo.FindByAddressPaginatedAsync("Main", 1, 10), Times.Once);
        }

        #endregion

        #region GetByAgeRangeAsync Tests

        [Test]
        public async Task GetByAgeRangeAsync_ReturnsOwnersInAgeRange()
        {
            // Arrange
            var owners = new List<Owner>
            {
                new Owner
                {
                    IdOwner = "1",
                    Name = "John Doe",
                    Address = "123 St",
                    Photo = null,
                    Birthday = new DateTime(1980, 1, 1)
                },
                new Owner
                {
                    IdOwner = "2",
                    Name = "Jane Smith",
                    Address = "456 Ave",
                    Photo = null,
                    Birthday = new DateTime(1985, 5, 15)
                }
            };

            _mockOwnerRepository
                .Setup(repo => repo.FindByAgeRangeAsync(30, 50))
                .ReturnsAsync(owners);

            // Act
            var result = await _ownerService!.GetByAgeRangeAsync(30, 50);

            // Assert
            var ownersList = result.ToList();
            Assert.That(ownersList, Has.Count.EqualTo(2));
            
            _mockOwnerRepository!.Verify(repo => repo.FindByAgeRangeAsync(30, 50), Times.Once);
        }

        #endregion

        #region GetByAgeRangePaginatedAsync Tests

        [Test]
        public async Task GetByAgeRangePaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            var owners = new List<Owner>
            {
                new Owner
                {
                    IdOwner = "1",
                    Name = "John Doe",
                    Address = "123 St",
                    Photo = null,
                    Birthday = new DateTime(1980, 1, 1)
                }
            };

            var paginationDto = new PaginationRequestDto
            {
                PageNumber = 1,
                PageSize = 10
            };

            _mockOwnerRepository
                .Setup(repo => repo.FindByAgeRangePaginatedAsync(30, 50, 1, 10))
                .ReturnsAsync((owners, 1L));

            // Act
            var result = await _ownerService!.GetByAgeRangePaginatedAsync(30, 50, paginationDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalRecords, Is.EqualTo(1));
            Assert.That(result.Data.Count(), Is.EqualTo(1));
            
            _mockOwnerRepository!.Verify(repo => repo.FindByAgeRangePaginatedAsync(30, 50, 1, 10), Times.Once);
        }

        #endregion
    }
}
