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
    /// Integration tests for PropertyRepository
    /// Note: These tests require a running MongoDB instance or use MongoDB in-memory testing
    /// </summary>
    [TestFixture]
    public class PropertyRepositoryTests
    {
        private IPropertyRepository? _propertyRepository;
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
            _propertyRepository = new PropertyRepository(_settings);

            // Clean up the test database before each test
            CleanupDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            CleanupDatabase();
            _propertyRepository = null;
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

        private async Task<Property> CreateTestProperty(
            string name = "Test Property",
            string address = "123 Test St",
            decimal price = 100000,
            string codeInternal = "TEST001")
        {
            var property = new Property
            {
                Name = name,
                Address = address,
                Price = price,
                CodeInternal = codeInternal,
                Year = 2020,
                IdOwner = ObjectId.GenerateNewId().ToString()
            };

            await _propertyRepository!.CreateAsync(property);
            return property;
        }

        #endregion

        #region GetAllAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetAllAsync_ReturnsAllProperties()
        {
            // Arrange
            await CreateTestProperty("Property 1", "Address 1", 100000, "CODE001");
            await CreateTestProperty("Property 2", "Address 2", 200000, "CODE002");
            await CreateTestProperty("Property 3", "Address 3", 300000, "CODE003");

            // Act
            var result = await _propertyRepository!.GetAllAsync();

            // Assert
            var properties = result.ToList();
            Assert.That(properties, Has.Count.EqualTo(3));
            Assert.That(properties.Any(p => p.Name == "Property 1"), Is.True);
            Assert.That(properties.Any(p => p.Name == "Property 2"), Is.True);
            Assert.That(properties.Any(p => p.Name == "Property 3"), Is.True);
        }

        [Test]
        [Category("Integration")]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoPropertiesExist()
        {
            // Act
            var result = await _propertyRepository!.GetAllAsync();

            // Assert
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region GetByIdAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetByIdAsync_ReturnsProperty_WhenPropertyExists()
        {
            // Arrange
            var property = await CreateTestProperty("Test Property", "456 Main St", 250000, "TEST123");

            // Act
            var result = await _propertyRepository!.GetByIdAsync(property.IdProperty);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IdProperty, Is.EqualTo(property.IdProperty));
            Assert.That(result.Name, Is.EqualTo("Test Property"));
            Assert.That(result.Price, Is.EqualTo(250000));
        }

        [Test]
        [Category("Integration")]
        public async Task GetByIdAsync_ReturnsNull_WhenPropertyDoesNotExist()
        {
            // Arrange
            var fakeId = ObjectId.GenerateNewId().ToString();

            // Act
            var result = await _propertyRepository!.GetByIdAsync(fakeId);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region CreateAsync Tests

        [Test]
        [Category("Integration")]
        public async Task CreateAsync_AddsPropertyToDatabase()
        {
            // Arrange
            var property = new Property
            {
                Name = "New Property",
                Address = "789 New Ave",
                Price = 350000,
                CodeInternal = "NEW001",
                Year = 2023,
                IdOwner = ObjectId.GenerateNewId().ToString()
            };

            // Act
            await _propertyRepository!.CreateAsync(property);

            // Assert
            var allProperties = await _propertyRepository.GetAllAsync();
            Assert.That(allProperties.Count(), Is.EqualTo(1));
            
            var createdProperty = allProperties.First();
            Assert.That(createdProperty.Name, Is.EqualTo("New Property"));
            Assert.That(createdProperty.Price, Is.EqualTo(350000));
            Assert.That(createdProperty.IdProperty, Is.Not.Null);
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        [Category("Integration")]
        public async Task UpdateAsync_UpdatesExistingProperty()
        {
            // Arrange
            var property = await CreateTestProperty("Original Name", "Original Address", 100000, "ORIG001");
            
            property.Name = "Updated Name";
            property.Address = "Updated Address";
            property.Price = 150000;

            // Act
            await _propertyRepository!.UpdateAsync(property.IdProperty, property);

            // Assert
            var updatedProperty = await _propertyRepository.GetByIdAsync(property.IdProperty);
            Assert.That(updatedProperty, Is.Not.Null);
            Assert.That(updatedProperty!.Name, Is.EqualTo("Updated Name"));
            Assert.That(updatedProperty.Address, Is.EqualTo("Updated Address"));
            Assert.That(updatedProperty.Price, Is.EqualTo(150000));
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        [Category("Integration")]
        public async Task DeleteAsync_RemovesPropertyFromDatabase()
        {
            // Arrange
            var property = await CreateTestProperty("To Delete", "Delete Address", 100000, "DEL001");

            // Act
            await _propertyRepository!.DeleteAsync(property.IdProperty);

            // Assert
            var deletedProperty = await _propertyRepository.GetByIdAsync(property.IdProperty);
            Assert.That(deletedProperty, Is.Null);
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
                await CreateTestProperty($"Property {i}", $"Address {i}", 100000 + i * 1000, $"CODE{i:000}");
            }

            // Act
            var (items, totalCount) = await _propertyRepository!.GetPaginatedAsync(1, 10);

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
                await CreateTestProperty($"Property {i}", $"Address {i}", 100000 + i * 1000, $"CODE{i:000}");
            }

            // Act
            var (items, totalCount) = await _propertyRepository!.GetPaginatedAsync(2, 10);

            // Assert
            Assert.That(totalCount, Is.EqualTo(15));
            Assert.That(items.Count(), Is.EqualTo(5)); // Remaining items
        }

        #endregion

        #region SearchPropertiesAsync Tests

        [Test]
        [Category("Integration")]
        public async Task SearchPropertiesAsync_ByName_ReturnsMatchingProperties()
        {
            // Arrange
            await CreateTestProperty("Modern House", "Address 1", 100000, "CODE001");
            await CreateTestProperty("Modern Apartment", "Address 2", 200000, "CODE002");
            await CreateTestProperty("Classic Villa", "Address 3", 300000, "CODE003");

            // Act
            var result = await _propertyRepository!.SearchPropertiesAsync(name: "Modern");

            // Assert
            var properties = result.ToList();
            Assert.That(properties, Has.Count.EqualTo(2));
            Assert.That(properties.All(p => p.Name.Contains("Modern")), Is.True);
        }

        [Test]
        [Category("Integration")]
        public async Task SearchPropertiesAsync_ByAddress_ReturnsMatchingProperties()
        {
            // Arrange
            await CreateTestProperty("Property 1", "123 Main Street", 100000, "CODE001");
            await CreateTestProperty("Property 2", "456 Main Avenue", 200000, "CODE002");
            await CreateTestProperty("Property 3", "789 Oak Street", 300000, "CODE003");

            // Act
            var result = await _propertyRepository!.SearchPropertiesAsync(address: "Main");

            // Assert
            var properties = result.ToList();
            Assert.That(properties, Has.Count.EqualTo(2));
            Assert.That(properties.All(p => p.Address.Contains("Main")), Is.True);
        }

        [Test]
        [Category("Integration")]
        public async Task SearchPropertiesAsync_ByPriceRange_ReturnsMatchingProperties()
        {
            // Arrange
            await CreateTestProperty("Property 1", "Address 1", 100000, "CODE001");
            await CreateTestProperty("Property 2", "Address 2", 200000, "CODE002");
            await CreateTestProperty("Property 3", "Address 3", 300000, "CODE003");
            await CreateTestProperty("Property 4", "Address 4", 400000, "CODE004");

            // Act
            var result = await _propertyRepository!.SearchPropertiesAsync(minPrice: 150000, maxPrice: 350000);

            // Assert
            var properties = result.ToList();
            Assert.That(properties, Has.Count.EqualTo(2));
            Assert.That(properties.All(p => p.Price >= 150000 && p.Price <= 350000), Is.True);
        }

        [Test]
        [Category("Integration")]
        public async Task SearchPropertiesAsync_WithMultipleFilters_ReturnsMatchingProperties()
        {
            // Arrange
            await CreateTestProperty("Modern House", "123 Main Street", 150000, "CODE001");
            await CreateTestProperty("Modern Apartment", "456 Oak Avenue", 250000, "CODE002");
            await CreateTestProperty("Classic House", "789 Main Street", 350000, "CODE003");

            // Act
            var result = await _propertyRepository!.SearchPropertiesAsync(
                name: "Modern",
                address: "Main",
                minPrice: 100000,
                maxPrice: 200000
            );

            // Assert
            var properties = result.ToList();
            Assert.That(properties, Has.Count.EqualTo(1));
            Assert.That(properties[0].Name, Is.EqualTo("Modern House"));
        }

        #endregion

        #region SearchPropertiesPaginatedAsync Tests

        [Test]
        [Category("Integration")]
        public async Task SearchPropertiesPaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            for (int i = 1; i <= 15; i++)
            {
                await CreateTestProperty($"Modern Property {i}", $"Address {i}", 100000 + i * 1000, $"CODE{i:000}");
            }

            // Act
            var (items, totalCount) = await _propertyRepository!.SearchPropertiesPaginatedAsync(
                name: "Modern",
                pageNumber: 1,
                pageSize: 10
            );

            // Assert
            Assert.That(totalCount, Is.EqualTo(15));
            Assert.That(items.Count(), Is.EqualTo(10));
        }

        #endregion

        #region GetByOwnerIdAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetByOwnerIdAsync_ReturnsPropertiesForOwner()
        {
            // Arrange
            var ownerId = ObjectId.GenerateNewId().ToString();
            var property1 = new Property
            {
                Name = "Property 1",
                Address = "Address 1",
                Price = 100000,
                CodeInternal = "CODE001",
                Year = 2020,
                IdOwner = ownerId
            };
            var property2 = new Property
            {
                Name = "Property 2",
                Address = "Address 2",
                Price = 200000,
                CodeInternal = "CODE002",
                Year = 2021,
                IdOwner = ownerId
            };

            await _propertyRepository!.CreateAsync(property1);
            await _propertyRepository.CreateAsync(property2);
            await CreateTestProperty("Other Property", "Other Address", 300000, "CODE003");

            // Act
            var result = await _propertyRepository.GetByOwnerIdAsync(ownerId);

            // Assert
            var properties = result.ToList();
            Assert.That(properties, Has.Count.EqualTo(2));
            Assert.That(properties.All(p => p.IdOwner == ownerId), Is.True);
        }

        #endregion

        #region GetByOwnerIdPaginatedAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetByOwnerIdPaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            var ownerId = ObjectId.GenerateNewId().ToString();
            for (int i = 1; i <= 15; i++)
            {
                var property = new Property
                {
                    Name = $"Property {i}",
                    Address = $"Address {i}",
                    Price = 100000 + i * 1000,
                    CodeInternal = $"CODE{i:000}",
                    Year = 2020,
                    IdOwner = ownerId
                };
                await _propertyRepository!.CreateAsync(property);
            }

            // Act
            var (items, totalCount) = await _propertyRepository!.GetByOwnerIdPaginatedAsync(ownerId, 1, 10);

            // Assert
            Assert.That(totalCount, Is.EqualTo(15));
            Assert.That(items.Count(), Is.EqualTo(10));
        }

        #endregion

        #region ExistsByCodeAsync Tests

        [Test]
        [Category("Integration")]
        public async Task ExistsByCodeAsync_ReturnsTrue_WhenPropertyExists()
        {
            // Arrange
            await CreateTestProperty("Test Property", "Address", 100000, "UNIQUE001");

            // Act
            var result = await _propertyRepository!.ExistsByCodeAsync("UNIQUE001");

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        [Category("Integration")]
        public async Task ExistsByCodeAsync_ReturnsFalse_WhenPropertyDoesNotExist()
        {
            // Act
            var result = await _propertyRepository!.ExistsByCodeAsync("NONEXISTENT");

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion
    }
}
