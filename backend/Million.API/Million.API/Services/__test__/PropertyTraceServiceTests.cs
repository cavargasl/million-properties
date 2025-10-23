using Million.API.Domain;
using Million.API.DTOs;
using Million.API.Repository;
using Moq;
using NUnit.Framework;

namespace Million.API.Services.Tests
{
    /// <summary>
    /// Unit tests for PropertyTraceService using NUnit and Moq
    /// </summary>
    [TestFixture]
    public class PropertyTraceServiceTests
    {
        private Mock<IPropertyTraceRepository>? _mockTraceRepository;
        private Mock<IPropertyRepository>? _mockPropertyRepository;
        private IPropertyTraceService? _traceService;

        [SetUp]
        public void Setup()
        {
            _mockTraceRepository = new Mock<IPropertyTraceRepository>();
            _mockPropertyRepository = new Mock<IPropertyRepository>();
            _traceService = new PropertyTraceService(
                _mockTraceRepository.Object,
                _mockPropertyRepository.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            _mockTraceRepository = null;
            _mockPropertyRepository = null;
            _traceService = null;
        }

        #region GetTracesByPropertyIdAsync Tests

        [Test]
        public async Task GetTracesByPropertyIdAsync_ReturnsTraces()
        {
            // Arrange
            var traces = new List<PropertyTrace>
            {
                new PropertyTrace
                {
                    IdPropertyTrace = "1",
                    IdProperty = "prop1",
                    DateSale = DateTime.Now,
                    Name = "Sale 1",
                    Value = 100000,
                    Tax = 5000
                },
                new PropertyTrace
                {
                    IdPropertyTrace = "2",
                    IdProperty = "prop1",
                    DateSale = DateTime.Now,
                    Name = "Sale 2",
                    Value = 200000,
                    Tax = 10000
                }
            };

            _mockTraceRepository
                .Setup(repo => repo.GetByPropertyIdAsync("prop1"))
                .ReturnsAsync(traces);

            // Act
            var result = await _traceService!.GetTracesByPropertyIdAsync("prop1");

            // Assert
            var traceList = result.ToList();
            Assert.That(traceList, Has.Count.EqualTo(2));
            Assert.That(traceList[0].Name, Is.EqualTo("Sale 1"));
            Assert.That(traceList[1].Name, Is.EqualTo("Sale 2"));
        }

        [Test]
        public async Task GetTracesByPropertyIdAsync_ReturnsEmpty_WhenNoTraces()
        {
            // Arrange
            _mockTraceRepository
                .Setup(repo => repo.GetByPropertyIdAsync("prop1"))
                .ReturnsAsync(new List<PropertyTrace>());

            // Act
            var result = await _traceService!.GetTracesByPropertyIdAsync("prop1");

            // Assert
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region GetTracesByPropertyIdPaginatedAsync Tests

        [Test]
        public async Task GetTracesByPropertyIdPaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            var traces = new List<PropertyTrace>
            {
                new PropertyTrace
                {
                    IdPropertyTrace = "1",
                    IdProperty = "prop1",
                    DateSale = DateTime.Now,
                    Name = "Sale 1",
                    Value = 100000,
                    Tax = 5000
                }
            };

            var paginationDto = new PaginationRequestDto { PageNumber = 1, PageSize = 10 };

            _mockTraceRepository
                .Setup(repo => repo.GetByPropertyIdPaginatedAsync("prop1", 1, 10))
                .ReturnsAsync((traces, 1L));

            // Act
            var result = await _traceService!.GetTracesByPropertyIdPaginatedAsync("prop1", paginationDto);

            // Assert
            Assert.That(result.TotalRecords, Is.EqualTo(1));
            Assert.That(result.Data.Count(), Is.EqualTo(1));
        }

        #endregion

        #region GetTraceByIdAsync Tests

        [Test]
        public async Task GetTraceByIdAsync_ReturnsTrace_WhenTraceExists()
        {
            // Arrange
            var trace = new PropertyTrace
            {
                IdPropertyTrace = "1",
                IdProperty = "prop1",
                DateSale = DateTime.Now,
                Name = "Test Sale",
                Value = 150000,
                Tax = 7500
            };

            _mockTraceRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(trace);

            // Act
            var result = await _traceService!.GetTraceByIdAsync("1");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Test Sale"));
            Assert.That(result.Value, Is.EqualTo(150000));
        }

        [Test]
        public async Task GetTraceByIdAsync_ReturnsNull_WhenTraceNotFound()
        {
            // Arrange
            _mockTraceRepository
                .Setup(repo => repo.GetByIdAsync("999"))
                .ReturnsAsync((PropertyTrace?)null);

            // Act
            var result = await _traceService!.GetTraceByIdAsync("999");

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region CreateTraceAsync Tests

        [Test]
        public async Task CreateTraceAsync_CreatesNewTrace()
        {
            // Arrange
            var createDto = new CreatePropertyTraceDto
            {
                IdProperty = "prop1",
                DateSale = DateTime.Now,
                Name = "New Sale",
                Value = 250000,
                Tax = 12500
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

            _mockTraceRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<PropertyTrace>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _traceService!.CreateTraceAsync(createDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("New Sale"));
            Assert.That(result.Value, Is.EqualTo(250000));
            Assert.That(result.IdProperty, Is.EqualTo("prop1"));
            
            _mockTraceRepository.Verify(repo => repo.CreateAsync(It.IsAny<PropertyTrace>()), Times.Once);
        }

        [Test]
        public void CreateTraceAsync_ThrowsException_WhenPropertyNotFound()
        {
            // Arrange
            var createDto = new CreatePropertyTraceDto
            {
                IdProperty = "invalid",
                DateSale = DateTime.Now,
                Name = "Sale",
                Value = 100000,
                Tax = 5000
            };

            _mockPropertyRepository
                .Setup(repo => repo.GetByIdAsync("invalid"))
                .ReturnsAsync((Property?)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _traceService!.CreateTraceAsync(createDto)
            );

            Assert.That(exception!.Message, Does.Contain("not found"));
            _mockTraceRepository.Verify(repo => repo.CreateAsync(It.IsAny<PropertyTrace>()), Times.Never);
        }

        #endregion

        #region UpdateTraceAsync Tests

        [Test]
        public async Task UpdateTraceAsync_UpdatesTrace_WhenTraceExists()
        {
            // Arrange
            var existingTrace = new PropertyTrace
            {
                IdPropertyTrace = "1",
                IdProperty = "prop1",
                DateSale = DateTime.Now,
                Name = "Old Sale",
                Value = 100000,
                Tax = 5000
            };

            var updateDto = new UpdatePropertyTraceDto
            {
                DateSale = DateTime.Now.AddDays(1),
                Name = "Updated Sale",
                Value = 150000,
                Tax = 7500
            };

            _mockTraceRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(existingTrace);

            _mockTraceRepository
                .Setup(repo => repo.UpdateAsync("1", It.IsAny<PropertyTrace>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _traceService!.UpdateTraceAsync("1", updateDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Updated Sale"));
            Assert.That(result.Value, Is.EqualTo(150000));
            Assert.That(result.Tax, Is.EqualTo(7500));
            
            _mockTraceRepository.Verify(repo => repo.UpdateAsync("1", It.IsAny<PropertyTrace>()), Times.Once);
        }

        [Test]
        public async Task UpdateTraceAsync_ReturnsNull_WhenTraceNotFound()
        {
            // Arrange
            var updateDto = new UpdatePropertyTraceDto
            {
                DateSale = DateTime.Now,
                Name = "Sale",
                Value = 100000,
                Tax = 5000
            };

            _mockTraceRepository
                .Setup(repo => repo.GetByIdAsync("999"))
                .ReturnsAsync((PropertyTrace?)null);

            // Act
            var result = await _traceService!.UpdateTraceAsync("999", updateDto);

            // Assert
            Assert.That(result, Is.Null);
            _mockTraceRepository.Verify(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<PropertyTrace>()), Times.Never);
        }

        #endregion

        #region DeleteTraceAsync Tests

        [Test]
        public async Task DeleteTraceAsync_ReturnsTrue_WhenTraceDeleted()
        {
            // Arrange
            var trace = new PropertyTrace
            {
                IdPropertyTrace = "1",
                IdProperty = "prop1",
                DateSale = DateTime.Now,
                Name = "Sale",
                Value = 100000,
                Tax = 5000
            };

            _mockTraceRepository
                .Setup(repo => repo.GetByIdAsync("1"))
                .ReturnsAsync(trace);

            _mockTraceRepository
                .Setup(repo => repo.DeleteAsync("1"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _traceService!.DeleteTraceAsync("1");

            // Assert
            Assert.That(result, Is.True);
            _mockTraceRepository.Verify(repo => repo.DeleteAsync("1"), Times.Once);
        }

        [Test]
        public async Task DeleteTraceAsync_ReturnsFalse_WhenTraceNotFound()
        {
            // Arrange
            _mockTraceRepository
                .Setup(repo => repo.GetByIdAsync("999"))
                .ReturnsAsync((PropertyTrace?)null);

            // Act
            var result = await _traceService!.DeleteTraceAsync("999");

            // Assert
            Assert.That(result, Is.False);
            _mockTraceRepository.Verify(repo => repo.DeleteAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region GetTracesByDateRangeAsync Tests

        [Test]
        public async Task GetTracesByDateRangeAsync_ReturnsTracesInRange()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 12, 31);

            var traces = new List<PropertyTrace>
            {
                new PropertyTrace
                {
                    IdPropertyTrace = "1",
                    IdProperty = "prop1",
                    DateSale = new DateTime(2024, 6, 15),
                    Name = "Mid Year Sale",
                    Value = 100000,
                    Tax = 5000
                }
            };

            _mockTraceRepository
                .Setup(repo => repo.GetByDateRangeAsync(startDate, endDate))
                .ReturnsAsync(traces);

            // Act
            var result = await _traceService!.GetTracesByDateRangeAsync(startDate, endDate);

            // Assert
            var traceList = result.ToList();
            Assert.That(traceList, Has.Count.EqualTo(1));
            Assert.That(traceList[0].Name, Is.EqualTo("Mid Year Sale"));
        }

        #endregion

        #region GetTracesByDateRangePaginatedAsync Tests

        [Test]
        public async Task GetTracesByDateRangePaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 12, 31);

            var traces = new List<PropertyTrace>
            {
                new PropertyTrace
                {
                    IdPropertyTrace = "1",
                    IdProperty = "prop1",
                    DateSale = new DateTime(2024, 6, 15),
                    Name = "Sale",
                    Value = 100000,
                    Tax = 5000
                }
            };

            var paginationDto = new PaginationRequestDto { PageNumber = 1, PageSize = 10 };

            _mockTraceRepository
                .Setup(repo => repo.GetByDateRangePaginatedAsync(startDate, endDate, 1, 10))
                .ReturnsAsync((traces, 1L));

            // Act
            var result = await _traceService!.GetTracesByDateRangePaginatedAsync(startDate, endDate, paginationDto);

            // Assert
            Assert.That(result.TotalRecords, Is.EqualTo(1));
            Assert.That(result.Data.Count(), Is.EqualTo(1));
        }

        #endregion

        #region GetTracesByValueRangeAsync Tests

        [Test]
        public async Task GetTracesByValueRangeAsync_ReturnsTracesInRange()
        {
            // Arrange
            var traces = new List<PropertyTrace>
            {
                new PropertyTrace
                {
                    IdPropertyTrace = "1",
                    IdProperty = "prop1",
                    DateSale = DateTime.Now,
                    Name = "Mid Value Sale",
                    Value = 150000,
                    Tax = 7500
                }
            };

            _mockTraceRepository
                .Setup(repo => repo.GetByValueRangeAsync(100000, 200000))
                .ReturnsAsync(traces);

            // Act
            var result = await _traceService!.GetTracesByValueRangeAsync(100000, 200000);

            // Assert
            var traceList = result.ToList();
            Assert.That(traceList, Has.Count.EqualTo(1));
            Assert.That(traceList[0].Name, Is.EqualTo("Mid Value Sale"));
        }

        #endregion

        #region GetTracesByValueRangePaginatedAsync Tests

        [Test]
        public async Task GetTracesByValueRangePaginatedAsync_ReturnsPaginatedResults()
        {
            // Arrange
            var traces = new List<PropertyTrace>
            {
                new PropertyTrace
                {
                    IdPropertyTrace = "1",
                    IdProperty = "prop1",
                    DateSale = DateTime.Now,
                    Name = "Sale",
                    Value = 150000,
                    Tax = 7500
                }
            };

            var paginationDto = new PaginationRequestDto { PageNumber = 1, PageSize = 10 };

            _mockTraceRepository
                .Setup(repo => repo.GetByValueRangePaginatedAsync(100000, 200000, 1, 10))
                .ReturnsAsync((traces, 1L));

            // Act
            var result = await _traceService!.GetTracesByValueRangePaginatedAsync(100000, 200000, paginationDto);

            // Assert
            Assert.That(result.TotalRecords, Is.EqualTo(1));
            Assert.That(result.Data.Count(), Is.EqualTo(1));
        }

        #endregion
    }
}
