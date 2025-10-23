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
    /// Integration tests for PropertyImageRepository
    /// Note: These tests require a running MongoDB instance or use MongoDB in-memory testing
    /// </summary>
    [TestFixture]
    public class PropertyImageRepositoryTests
    {
        private IPropertyImageRepository? _imageRepository;
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
            _imageRepository = new PropertyImageRepository(_settings);

            // Clean up the test database before each test
            CleanupDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            CleanupDatabase();
            _imageRepository = null;
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

        private async Task<PropertyImage> CreateTestImage(
            string? propertyId = null,
            string file = "https://example.com/image.jpg",
            bool enabled = true)
        {
            var image = new PropertyImage
            {
                IdProperty = propertyId ?? ObjectId.GenerateNewId().ToString(),
                File = file,
                Enabled = enabled
            };

            await _imageRepository!.CreateAsync(image);
            return image;
        }

        #endregion

        #region GetAllAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetAllAsync_ReturnsAllImages()
        {
            // Arrange
            await CreateTestImage(file: "image1.jpg");
            await CreateTestImage(file: "image2.jpg");
            await CreateTestImage(file: "image3.jpg");

            // Act
            var result = await _imageRepository!.GetAllAsync();

            // Assert
            var images = result.ToList();
            Assert.That(images, Has.Count.EqualTo(3));
        }

        [Test]
        [Category("Integration")]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoImagesExist()
        {
            // Act
            var result = await _imageRepository!.GetAllAsync();

            // Assert
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region GetByIdAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetByIdAsync_ReturnsImage_WhenImageExists()
        {
            // Arrange
            var image = await CreateTestImage(file: "test-image.jpg");

            // Act
            var result = await _imageRepository!.GetByIdAsync(image.IdPropertyImage);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IdPropertyImage, Is.EqualTo(image.IdPropertyImage));
            Assert.That(result.File, Is.EqualTo("test-image.jpg"));
        }

        [Test]
        [Category("Integration")]
        public async Task GetByIdAsync_ReturnsNull_WhenImageDoesNotExist()
        {
            // Arrange
            var fakeId = ObjectId.GenerateNewId().ToString();

            // Act
            var result = await _imageRepository!.GetByIdAsync(fakeId);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region CreateAsync Tests

        [Test]
        [Category("Integration")]
        public async Task CreateAsync_AddsImageToDatabase()
        {
            // Arrange
            var image = new PropertyImage
            {
                IdProperty = ObjectId.GenerateNewId().ToString(),
                File = "new-image.jpg",
                Enabled = true
            };

            // Act
            await _imageRepository!.CreateAsync(image);

            // Assert
            var allImages = await _imageRepository.GetAllAsync();
            Assert.That(allImages.Count(), Is.EqualTo(1));
            
            var createdImage = allImages.First();
            Assert.That(createdImage.File, Is.EqualTo("new-image.jpg"));
            Assert.That(createdImage.IdPropertyImage, Is.Not.Null);
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        [Category("Integration")]
        public async Task UpdateAsync_UpdatesExistingImage()
        {
            // Arrange
            var image = await CreateTestImage(file: "original.jpg", enabled: true);
            
            image.File = "updated.jpg";
            image.Enabled = false;

            // Act
            await _imageRepository!.UpdateAsync(image.IdPropertyImage, image);

            // Assert
            var updatedImage = await _imageRepository.GetByIdAsync(image.IdPropertyImage);
            Assert.That(updatedImage, Is.Not.Null);
            Assert.That(updatedImage!.File, Is.EqualTo("updated.jpg"));
            Assert.That(updatedImage.Enabled, Is.False);
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        [Category("Integration")]
        public async Task DeleteAsync_RemovesImageFromDatabase()
        {
            // Arrange
            var image = await CreateTestImage(file: "to-delete.jpg");

            // Act
            await _imageRepository!.DeleteAsync(image.IdPropertyImage);

            // Assert
            var deletedImage = await _imageRepository.GetByIdAsync(image.IdPropertyImage);
            Assert.That(deletedImage, Is.Null);
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
                await CreateTestImage(file: $"image{i}.jpg");
            }

            // Act
            var (items, totalCount) = await _imageRepository!.GetPaginatedAsync(1, 10);

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
                await CreateTestImage(file: $"image{i}.jpg");
            }

            // Act
            var (items, totalCount) = await _imageRepository!.GetPaginatedAsync(2, 10);

            // Assert
            Assert.That(totalCount, Is.EqualTo(15));
            Assert.That(items.Count(), Is.EqualTo(5)); // Remaining items
        }

        #endregion

        #region GetByPropertyIdAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetByPropertyIdAsync_ReturnsImagesForProperty()
        {
            // Arrange
            var propertyId = ObjectId.GenerateNewId().ToString();
            await CreateTestImage(propertyId, "image1.jpg");
            await CreateTestImage(propertyId, "image2.jpg");
            await CreateTestImage(propertyId, "image3.jpg");
            await CreateTestImage(file: "other-image.jpg"); // Different property

            // Act
            var result = await _imageRepository!.GetByPropertyIdAsync(propertyId);

            // Assert
            var images = result.ToList();
            Assert.That(images, Has.Count.EqualTo(3));
            Assert.That(images.All(img => img.IdProperty == propertyId), Is.True);
        }

        [Test]
        [Category("Integration")]
        public async Task GetByPropertyIdAsync_ReturnsEmpty_WhenNoImagesForProperty()
        {
            // Arrange
            var propertyId = ObjectId.GenerateNewId().ToString();

            // Act
            var result = await _imageRepository!.GetByPropertyIdAsync(propertyId);

            // Assert
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region GetByPropertyIdPaginatedAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetByPropertyIdPaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            var propertyId = ObjectId.GenerateNewId().ToString();
            for (int i = 1; i <= 15; i++)
            {
                await CreateTestImage(propertyId, $"image{i}.jpg");
            }

            // Act
            var (items, totalCount) = await _imageRepository!.GetByPropertyIdPaginatedAsync(propertyId, 1, 10);

            // Assert
            Assert.That(totalCount, Is.EqualTo(15));
            Assert.That(items.Count(), Is.EqualTo(10));
        }

        #endregion

        #region GetEnabledByPropertyIdAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetEnabledByPropertyIdAsync_ReturnsOnlyEnabledImages()
        {
            // Arrange
            var propertyId = ObjectId.GenerateNewId().ToString();
            await CreateTestImage(propertyId, "enabled1.jpg", enabled: true);
            await CreateTestImage(propertyId, "enabled2.jpg", enabled: true);
            await CreateTestImage(propertyId, "disabled1.jpg", enabled: false);
            await CreateTestImage(propertyId, "disabled2.jpg", enabled: false);

            // Act
            var result = await _imageRepository!.GetEnabledByPropertyIdAsync(propertyId);

            // Assert
            var images = result.ToList();
            Assert.That(images, Has.Count.EqualTo(2));
            Assert.That(images.All(img => img.Enabled), Is.True);
        }

        #endregion

        #region DisableAllForPropertyAsync Tests

        [Test]
        [Category("Integration")]
        public async Task DisableAllForPropertyAsync_DisablesAllImages()
        {
            // Arrange
            var propertyId = ObjectId.GenerateNewId().ToString();
            await CreateTestImage(propertyId, "image1.jpg", enabled: true);
            await CreateTestImage(propertyId, "image2.jpg", enabled: true);
            await CreateTestImage(propertyId, "image3.jpg", enabled: true);

            // Act
            await _imageRepository!.DisableAllForPropertyAsync(propertyId);

            // Assert
            var images = await _imageRepository.GetByPropertyIdAsync(propertyId);
            Assert.That(images.All(img => !img.Enabled), Is.True);
        }

        #endregion

        #region DeleteAllForPropertyAsync Tests

        [Test]
        [Category("Integration")]
        public async Task DeleteAllForPropertyAsync_DeletesAllImages()
        {
            // Arrange
            var propertyId = ObjectId.GenerateNewId().ToString();
            await CreateTestImage(propertyId, "image1.jpg");
            await CreateTestImage(propertyId, "image2.jpg");
            await CreateTestImage(propertyId, "image3.jpg");
            
            var otherPropertyId = ObjectId.GenerateNewId().ToString();
            await CreateTestImage(otherPropertyId, "other-image.jpg");

            // Act
            await _imageRepository!.DeleteAllForPropertyAsync(propertyId);

            // Assert
            var imagesForProperty = await _imageRepository.GetByPropertyIdAsync(propertyId);
            var allImages = await _imageRepository.GetAllAsync();
            
            Assert.That(imagesForProperty, Is.Empty);
            Assert.That(allImages.Count(), Is.EqualTo(1)); // Only other property's image remains
        }

        #endregion
    }
}
