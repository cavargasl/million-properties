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
    /// Integration tests for PropertyTraceRepository
    /// Note: These tests require a running MongoDB instance or use MongoDB in-memory testing
    /// </summary>
    [TestFixture]
    public class PropertyTraceRepositoryTests
    {
        private IPropertyTraceRepository? _traceRepository;
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
            _traceRepository = new PropertyTraceRepository(_settings);

            // Clean up the test database before each test
            CleanupDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after each test
            CleanupDatabase();
            _traceRepository = null;
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

        private async Task<PropertyTrace> CreateTestTrace(
            string? propertyId = null,
            DateTime? dateSale = null,
            string name = "Test Sale",
            decimal value = 100000,
            decimal tax = 5000)
        {
            var trace = new PropertyTrace
            {
                IdProperty = propertyId ?? ObjectId.GenerateNewId().ToString(),
                DateSale = dateSale ?? DateTime.Now,
                Name = name,
                Value = value,
                Tax = tax
            };

            await _traceRepository!.CreateAsync(trace);
            return trace;
        }

        #endregion

        #region GetAllAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetAllAsync_ReturnsAllTraces()
        {
            // Arrange
            await CreateTestTrace(name: "Sale 1");
            await CreateTestTrace(name: "Sale 2");
            await CreateTestTrace(name: "Sale 3");

            // Act
            var result = await _traceRepository!.GetAllAsync();

            // Assert
            var traces = result.ToList();
            Assert.That(traces, Has.Count.EqualTo(3));
        }

        [Test]
        [Category("Integration")]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoTracesExist()
        {
            // Act
            var result = await _traceRepository!.GetAllAsync();

            // Assert
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region GetByIdAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetByIdAsync_ReturnsTrace_WhenTraceExists()
        {
            // Arrange
            var trace = await CreateTestTrace(name: "Test Sale", value: 250000);

            // Act
            var result = await _traceRepository!.GetByIdAsync(trace.IdPropertyTrace);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IdPropertyTrace, Is.EqualTo(trace.IdPropertyTrace));
            Assert.That(result.Name, Is.EqualTo("Test Sale"));
            Assert.That(result.Value, Is.EqualTo(250000));
        }

        [Test]
        [Category("Integration")]
        public async Task GetByIdAsync_ReturnsNull_WhenTraceDoesNotExist()
        {
            // Arrange
            var fakeId = ObjectId.GenerateNewId().ToString();

            // Act
            var result = await _traceRepository!.GetByIdAsync(fakeId);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region CreateAsync Tests

        [Test]
        [Category("Integration")]
        public async Task CreateAsync_AddsTraceToDatabase()
        {
            // Arrange
            var trace = new PropertyTrace
            {
                IdProperty = ObjectId.GenerateNewId().ToString(),
                DateSale = DateTime.Now,
                Name = "New Sale",
                Value = 350000,
                Tax = 17500
            };

            // Act
            await _traceRepository!.CreateAsync(trace);

            // Assert
            var allTraces = await _traceRepository.GetAllAsync();
            Assert.That(allTraces.Count(), Is.EqualTo(1));
            
            var createdTrace = allTraces.First();
            Assert.That(createdTrace.Name, Is.EqualTo("New Sale"));
            Assert.That(createdTrace.Value, Is.EqualTo(350000));
            Assert.That(createdTrace.IdPropertyTrace, Is.Not.Null);
        }

        #endregion

        #region UpdateAsync Tests

        [Test]
        [Category("Integration")]
        public async Task UpdateAsync_UpdatesExistingTrace()
        {
            // Arrange
            var trace = await CreateTestTrace(name: "Original Sale", value: 100000);
            
            trace.Name = "Updated Sale";
            trace.Value = 150000;
            trace.Tax = 7500;

            // Act
            await _traceRepository!.UpdateAsync(trace.IdPropertyTrace, trace);

            // Assert
            var updatedTrace = await _traceRepository.GetByIdAsync(trace.IdPropertyTrace);
            Assert.That(updatedTrace, Is.Not.Null);
            Assert.That(updatedTrace!.Name, Is.EqualTo("Updated Sale"));
            Assert.That(updatedTrace.Value, Is.EqualTo(150000));
            Assert.That(updatedTrace.Tax, Is.EqualTo(7500));
        }

        #endregion

        #region DeleteAsync Tests

        [Test]
        [Category("Integration")]
        public async Task DeleteAsync_RemovesTraceFromDatabase()
        {
            // Arrange
            var trace = await CreateTestTrace(name: "To Delete");

            // Act
            await _traceRepository!.DeleteAsync(trace.IdPropertyTrace);

            // Assert
            var deletedTrace = await _traceRepository.GetByIdAsync(trace.IdPropertyTrace);
            Assert.That(deletedTrace, Is.Null);
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
                await CreateTestTrace(name: $"Sale {i}");
            }

            // Act
            var (items, totalCount) = await _traceRepository!.GetPaginatedAsync(1, 10);

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
                await CreateTestTrace(name: $"Sale {i}");
            }

            // Act
            var (items, totalCount) = await _traceRepository!.GetPaginatedAsync(2, 10);

            // Assert
            Assert.That(totalCount, Is.EqualTo(15));
            Assert.That(items.Count(), Is.EqualTo(5)); // Remaining items
        }

        #endregion

        #region GetByPropertyIdAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetByPropertyIdAsync_ReturnsTracesForProperty()
        {
            // Arrange
            var propertyId = ObjectId.GenerateNewId().ToString();
            await CreateTestTrace(propertyId, name: "Sale 1");
            await CreateTestTrace(propertyId, name: "Sale 2");
            await CreateTestTrace(propertyId, name: "Sale 3");
            await CreateTestTrace(name: "Other Sale"); // Different property

            // Act
            var result = await _traceRepository!.GetByPropertyIdAsync(propertyId);

            // Assert
            var traces = result.ToList();
            Assert.That(traces, Has.Count.EqualTo(3));
            Assert.That(traces.All(t => t.IdProperty == propertyId), Is.True);
        }

        [Test]
        [Category("Integration")]
        public async Task GetByPropertyIdAsync_ReturnsEmpty_WhenNoTracesForProperty()
        {
            // Arrange
            var propertyId = ObjectId.GenerateNewId().ToString();

            // Act
            var result = await _traceRepository!.GetByPropertyIdAsync(propertyId);

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
                await CreateTestTrace(propertyId, name: $"Sale {i}");
            }

            // Act
            var (items, totalCount) = await _traceRepository!.GetByPropertyIdPaginatedAsync(propertyId, 1, 10);

            // Assert
            Assert.That(totalCount, Is.EqualTo(15));
            Assert.That(items.Count(), Is.EqualTo(10));
        }

        #endregion

        #region GetByDateRangeAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetByDateRangeAsync_ReturnsTracesInDateRange()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 12, 31);

            await CreateTestTrace(dateSale: new DateTime(2024, 6, 15), name: "Mid Year Sale");
            await CreateTestTrace(dateSale: new DateTime(2023, 12, 31), name: "Last Year Sale");
            await CreateTestTrace(dateSale: new DateTime(2025, 1, 1), name: "Next Year Sale");

            // Act
            var result = await _traceRepository!.GetByDateRangeAsync(startDate, endDate);

            // Assert
            var traces = result.ToList();
            Assert.That(traces, Has.Count.EqualTo(1));
            Assert.That(traces[0].Name, Is.EqualTo("Mid Year Sale"));
        }

        #endregion

        #region GetByDateRangePaginatedAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetByDateRangePaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 12, 31);

            for (int i = 1; i <= 15; i++)
            {
                await CreateTestTrace(dateSale: new DateTime(2024, i % 12 + 1, 1), name: $"Sale {i}");
            }

            // Act
            var (items, totalCount) = await _traceRepository!.GetByDateRangePaginatedAsync(startDate, endDate, 1, 10);

            // Assert
            Assert.That(totalCount, Is.EqualTo(15));
            Assert.That(items.Count(), Is.EqualTo(10));
        }

        #endregion

        #region GetByValueRangeAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetByValueRangeAsync_ReturnsTracesInValueRange()
        {
            // Arrange
            await CreateTestTrace(name: "Low Value", value: 50000);
            await CreateTestTrace(name: "Mid Value", value: 150000);
            await CreateTestTrace(name: "High Value", value: 300000);

            // Act
            var result = await _traceRepository!.GetByValueRangeAsync(100000, 200000);

            // Assert
            var traces = result.ToList();
            Assert.That(traces, Has.Count.EqualTo(1));
            Assert.That(traces[0].Name, Is.EqualTo("Mid Value"));
        }

        #endregion

        #region GetByValueRangePaginatedAsync Tests

        [Test]
        [Category("Integration")]
        public async Task GetByValueRangePaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            for (int i = 1; i <= 15; i++)
            {
                await CreateTestTrace(name: $"Sale {i}", value: 100000 + i * 10000);
            }

            // Act
            var (items, totalCount) = await _traceRepository!.GetByValueRangePaginatedAsync(100000, 300000, 1, 10);

            // Assert
            Assert.That(totalCount, Is.EqualTo(15));
            Assert.That(items.Count(), Is.EqualTo(10));
        }

        #endregion
    }
}
