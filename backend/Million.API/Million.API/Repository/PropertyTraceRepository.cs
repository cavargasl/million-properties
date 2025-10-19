using Million.API.Domain;
using Million.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ASP.MongoDb.API.Repository;

namespace Million.API.Repository
{
    public class PropertyTraceRepository : Repository<PropertyTrace>
    {
        public PropertyTraceRepository(IOptions<MongoDbSettings> settings) : base(settings)
        {
        }

        /// <summary>
        /// Get all traces for a specific property
        /// </summary>
        public async Task<IEnumerable<PropertyTrace>> GetByPropertyIdAsync(string propertyId)
        {
            var filter = Builders<PropertyTrace>.Filter.Eq(x => x.IdProperty, propertyId);
            return await _collection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Get traces by date range
        /// </summary>
        public async Task<IEnumerable<PropertyTrace>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<PropertyTrace>.Filter.And(
                Builders<PropertyTrace>.Filter.Gte(x => x.DateSale, startDate),
                Builders<PropertyTrace>.Filter.Lte(x => x.DateSale, endDate)
            );
            return await _collection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Get traces by value range
        /// </summary>
        public async Task<IEnumerable<PropertyTrace>> GetByValueRangeAsync(decimal minValue, decimal maxValue)
        {
            var filter = Builders<PropertyTrace>.Filter.And(
                Builders<PropertyTrace>.Filter.Gte(x => x.Value, minValue),
                Builders<PropertyTrace>.Filter.Lte(x => x.Value, maxValue)
            );
            return await _collection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Delete all traces for a specific property
        /// </summary>
        public async Task DeleteAllForPropertyAsync(string propertyId)
        {
            var filter = Builders<PropertyTrace>.Filter.Eq(x => x.IdProperty, propertyId);
            await _collection.DeleteManyAsync(filter);
        }
    }
}
