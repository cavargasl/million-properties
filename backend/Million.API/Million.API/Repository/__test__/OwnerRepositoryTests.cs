using Microsoft.Extensions.Options;
using Million.API.Domain;
using Million.API.Repository;
using Million.API.Settings;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;

namespace Million.API.Repository.Tests
{
    /// <summary>
    /// Integration tests for OwnerRepository
    /// Note: These tests require a running MongoDB instance or use MongoDB in-memory testing
    /// For unit tests with mocks, see the comments below
    /// </summary>
    [TestFixture]
    public class OwnerRepositoryTests
    {
        private IOwnerRepository? _ownerRepository;
        private IOptions<MongoDbSettings>? _settings;
        private const string TestDatabaseName = "MillionPropertiesTestDb";
        private const string TestConnectionString = "mongodb://localhost:27017";

        [SetUp]
        public void Setup()
        {
            // Configure MongoDB settings for testing
            var mongoSettings = new MongoDbSettings
            {
                ConnectionString = TestConnectionString,
                DatabaseName = TestDatabaseName
            };

            _settings = Options.Create(mongoSettings);
            _ownerRepository = new OwnerRepository(_settings);

            // Clean up the test database before each test
            CleanupDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            CleanupDatabase();
            _ownerRepository = null;
            _settings = null;
        }

        private void CleanupDatabase()
        {
            try
            {
                var client = new MongoClient(TestConnectionString);
                client.DropDatabase(TestDatabaseName);
            }
            catch (Exception)
            {
                // Ignore cleanup errors
            }
        }

        #region Helper Methods

        private async Task<Owner> CreateTestOwner(string name = "Test Owner", string address = "123 Test St")
        {
            var owner = new Owner
            {
                Name = name,
                Address = address,
                Photo = "https://example.com/photo.jpg",
                Birthday = new DateTime(1980, 1, 1)
            };

            await _ownerRepository!.CreateAsync(owner);
            return owner;
        }

        #endregion

        #region GetAllAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetAllAsync_ReturnsAllOwners()
        {
            // Arrange
            await CreateTestOwner("Owner 1", "Address 1");
            await CreateTestOwner("Owner 2", "Address 2");
            await CreateTestOwner("Owner 3", "Address 3");

            // Act
            var result = await _ownerRepository!.GetAllAsync();

            // Assert
            var owners = result.ToList();
            Assert.That(owners, Has.Count.EqualTo(3));
            Assert.That(owners.Any(o => o.Name == "Owner 1"), Is.True);
            Assert.That(owners.Any(o => o.Name == "Owner 2"), Is.True);
            Assert.That(owners.Any(o => o.Name == "Owner 3"), Is.True);
        }

        [Test]
        [Category("Integration")]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoOwnersExist()
        {
            // Act
            var result = await _ownerRepository!.GetAllAsync();

            // Assert
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region GetByIdAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetByIdAsync_ReturnsOwner_WhenOwnerExists()
        {
            // Arrange
            var owner = await CreateTestOwner("John Doe", "456 Main St");

            // Act
            var result = await _ownerRepository!.GetByIdAsync(owner.IdOwner);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IdOwner, Is.EqualTo(owner.IdOwner));
            Assert.That(result.Name, Is.EqualTo("John Doe"));
            Assert.That(result.Address, Is.EqualTo("456 Main St"));
        }

        [Test]
        [Category("Integration")]
        public async Task GetByIdAsync_ReturnsNull_WhenOwnerDoesNotExist()
        {
            // Arrange
            var fakeId = ObjectId.GenerateNewId().ToString();

            // Act
            var result = await _ownerRepository!.GetByIdAsync(fakeId);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region CreateAsync Tests

        [Test]
        [Category("Integration")]
        public async Task CreateAsync_AddsOwnerToDatabase()
        {
            // Arrange
            var owner = new Owner
            {
                Name = "New Owner",
                Address = "789 New Ave",
                Photo = "https://example.com/photo.jpg",
                Birthday = new DateTime(1990, 5, 15)
            };

            // Act
            await _ownerRepository!.CreateAsync(owner);

            // Assert
            var allOwners = await _ownerRepository.GetAllAsync();
            Assert.That(allOwners.Count(), Is.EqualTo(1));
            
            var createdOwner = allOwners.First();
            Assert.That(createdOwner.Name, Is.EqualTo("New Owner"));
            Assert.That(createdOwner.Address, Is.EqualTo("789 New Ave"));
            Assert.That(createdOwner.IdOwner, Is.Not.Null);
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        [Category("Integration")]
        public async Task UpdateAsync_UpdatesExistingOwner()
        {
            // Arrange
            var owner = await CreateTestOwner("Original Name", "Original Address");
            
            owner.Name = "Updated Name";
            owner.Address = "Updated Address";

            // Act
            await _ownerRepository!.UpdateAsync(owner.IdOwner, owner);

            // Assert
            var updatedOwner = await _ownerRepository.GetByIdAsync(owner.IdOwner);
            Assert.That(updatedOwner, Is.Not.Null);
            Assert.That(updatedOwner!.Name, Is.EqualTo("Updated Name"));
            Assert.That(updatedOwner.Address, Is.EqualTo("Updated Address"));
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        [Category("Integration")]
        public async Task DeleteAsync_RemovesOwnerFromDatabase()
        {
            // Arrange
            var owner = await CreateTestOwner("To Delete", "Delete Address");

            // Act
            await _ownerRepository!.DeleteAsync(owner.IdOwner);

            // Assert
            var deletedOwner = await _ownerRepository.GetByIdAsync(owner.IdOwner);
            Assert.That(deletedOwner, Is.Null);
        }

        #endregion

        #region GetPaginatedAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetPaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            for (int i = 1; i <= 15; i++)
            {
                await CreateTestOwner($"Owner {i}", $"Address {i}");
            }

            // Act
            var (items, totalCount) = await _ownerRepository!.GetPaginatedAsync(1, 10);

            // Assert
            Assert.That(totalCount, Is.EqualTo(15));
            Assert.That(items.Count(), Is.EqualTo(10));
        }

        [Test]
        [Category("Integration")]
        public async Task GetPaginatedAsync_ReturnsSecondPage()
        {
            // Arrange
            for (int i = 1; i <= 15; i++)
            {
                await CreateTestOwner($"Owner {i}", $"Address {i}");
            }

            // Act
            var (items, totalCount) = await _ownerRepository!.GetPaginatedAsync(2, 10);

            // Assert
            Assert.That(totalCount, Is.EqualTo(15));
            Assert.That(items.Count(), Is.EqualTo(5)); // Remaining items
        }

        #endregion

        #region FindByNameAsync Tests

        [Test]
        [Category("Integration")]
        public async Task FindByNameAsync_ReturnsMatchingOwners()
        {
            // Arrange
            await CreateTestOwner("John Doe", "Address 1");
            await CreateTestOwner("John Smith", "Address 2");
            await CreateTestOwner("Jane Doe", "Address 3");

            // Act
            var result = await _ownerRepository!.FindByNameAsync("John");

            // Assert
            var owners = result.ToList();
            Assert.That(owners, Has.Count.EqualTo(2));
            Assert.That(owners.All(o => o.Name.Contains("John")), Is.True);
        }

        [Test]
        [Category("Integration")]
        public async Task FindByNameAsync_IsCaseInsensitive()
        {
            // Arrange
            await CreateTestOwner("John Doe", "Address 1");

            // Act
            var result = await _ownerRepository!.FindByNameAsync("john");

            // Assert
            var owners = result.ToList();
            Assert.That(owners, Has.Count.EqualTo(1));
            Assert.That(owners[0].Name, Is.EqualTo("John Doe"));
        }

        #endregion

        #region FindByNamePaginatedAsync Tests

        [Test]
        [Category("Integration")]
        public async Task FindByNamePaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            for (int i = 1; i <= 15; i++)
            {
                await CreateTestOwner($"Test Owner {i}", $"Address {i}");
            }

            // Act
            var (items, totalCount) = await _ownerRepository!.FindByNamePaginatedAsync("Test", 1, 10);

            // Assert
            Assert.That(totalCount, Is.EqualTo(15));
            Assert.That(items.Count(), Is.EqualTo(10));
        }

        #endregion

        #region FindByAddressAsync Tests

        [Test]
        [Category("Integration")]
        public async Task FindByAddressAsync_ReturnsMatchingOwners()
        {
            // Arrange
            await CreateTestOwner("Owner 1", "123 Main Street");
            await CreateTestOwner("Owner 2", "456 Main Avenue");
            await CreateTestOwner("Owner 3", "789 Oak Street");

            // Act
            var result = await _ownerRepository!.FindByAddressAsync("Main");

            // Assert
            var owners = result.ToList();
            Assert.That(owners, Has.Count.EqualTo(2));
            Assert.That(owners.All(o => o.Address.Contains("Main")), Is.True);
        }

        #endregion

        #region FindByAddressPaginatedAsync Tests

        [Test]
        [Category("Integration")]
        public async Task FindByAddressPaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            for (int i = 1; i <= 15; i++)
            {
                await CreateTestOwner($"Owner {i}", $"Main Street {i}");
            }

            // Act
            var (items, totalCount) = await _ownerRepository!.FindByAddressPaginatedAsync("Main", 1, 10);

            // Assert
            Assert.That(totalCount, Is.EqualTo(15));
            Assert.That(items.Count(), Is.EqualTo(10));
        }

        #endregion

        #region FindByAgeRangeAsync Tests

        [Test]
        [Category("Integration")]
        public async Task FindByAgeRangeAsync_ReturnsOwnersInAgeRange()
        {
            // Arrange
            var now = DateTime.Now;
            
            // 25 years old
            var owner1 = new Owner
            {
                Name = "Owner 25",
                Address = "Address 1",
                Birthday = now.AddYears(-25)
            };
            
            // 35 years old
            var owner2 = new Owner
            {
                Name = "Owner 35",
                Address = "Address 2",
                Birthday = now.AddYears(-35)
            };
            
            // 45 years old
            var owner3 = new Owner
            {
                Name = "Owner 45",
                Address = "Address 3",
                Birthday = now.AddYears(-45)
            };

            await _ownerRepository!.CreateAsync(owner1);
            await _ownerRepository.CreateAsync(owner2);
            await _ownerRepository.CreateAsync(owner3);

            // Act - Find owners between 30 and 40 years old
            var result = await _ownerRepository.FindByAgeRangeAsync(30, 40);

            // Assert
            var owners = result.ToList();
            Assert.That(owners, Has.Count.EqualTo(1));
            Assert.That(owners[0].Name, Is.EqualTo("Owner 35"));
        }

        #endregion

        #region FindByAgeRangePaginatedAsync Tests

        [Test]
        [Category("Integration")]
        public async Task FindByAgeRangePaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            var now = DateTime.Now;
            
            for (int i = 25; i <= 45; i++)
            {
                var owner = new Owner
                {
                    Name = $"Owner {i}",
                    Address = $"Address {i}",
                    Birthday = now.AddYears(-i)
                };
                await _ownerRepository!.CreateAsync(owner);
            }

            // Act - Find owners between 25 and 45 years old
            var (items, totalCount) = await _ownerRepository!.FindByAgeRangePaginatedAsync(25, 45, 1, 10);

            // Assert
            Assert.That(totalCount, Is.EqualTo(20)); // 25 to 45 inclusive
            Assert.That(items.Count(), Is.EqualTo(10));
        }

        #endregion

        #region ExistsByNameAsync Tests

        [Test]
        [Category("Integration")]
        public async Task ExistsByNameAsync_ReturnsTrue_WhenOwnerExists()
        {
            // Arrange
            await CreateTestOwner("Unique Name", "Address 1");

            // Act
            var result = await _ownerRepository!.ExistsByNameAsync("Unique Name");

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        [Category("Integration")]
        public async Task ExistsByNameAsync_ReturnsFalse_WhenOwnerDoesNotExist()
        {
            // Act
            var result = await _ownerRepository!.ExistsByNameAsync("Non Existent");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        [Category("Integration")]
        public async Task ExistsByNameAsync_IsExactMatch()
        {
            // Arrange
            await CreateTestOwner("John Doe", "Address 1");

            // Act
            var result = await _ownerRepository!.ExistsByNameAsync("John");

            // Assert
            Assert.That(result, Is.False); // Should be false because it's not an exact match
        }

        #endregion
    }
}
