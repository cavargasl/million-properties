using ASP.MongoDb.API.Repository;
using Microsoft.Extensions.Options;
using Million.API.Domain;
using Million.API.Settings;
using MongoDB.Driver;

namespace Million.API.Repository
{
    public class PropertyImageRepository : Repository<PropertyImage>
    {
        public PropertyImageRepository(IOptions<MongoDbSettings> settings) : base(settings)
        {
        }

        public async Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(string propertyId)
        {
            var filter = Builders<PropertyImage>.Filter.Eq(x => x.IdProperty, propertyId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<(IEnumerable<PropertyImage> Items, long TotalCount)> GetByPropertyIdPaginatedAsync(
            string propertyId,
            int pageNumber,
            int pageSize)
        {
            var filter = Builders<PropertyImage>.Filter.Eq(x => x.IdProperty, propertyId);
            var totalCount = await _collection.CountDocumentsAsync(filter);

            var items = await _collection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<PropertyImage>> GetEnabledByPropertyIdAsync(string propertyId)
        {
            var filter = Builders<PropertyImage>.Filter.And(
                Builders<PropertyImage>.Filter.Eq(x => x.IdProperty, propertyId),
                Builders<PropertyImage>.Filter.Eq(x => x.Enabled, true)
            );
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task DisableAllForPropertyAsync(string propertyId)
        {
            var filter = Builders<PropertyImage>.Filter.Eq(x => x.IdProperty, propertyId);
            var update = Builders<PropertyImage>.Update.Set(x => x.Enabled, false);
            await _collection.UpdateManyAsync(filter, update);
        }

        public async Task DeleteAllForPropertyAsync(string propertyId)
        {
            var filter = Builders<PropertyImage>.Filter.Eq(x => x.IdProperty, propertyId);
            await _collection.DeleteManyAsync(filter);
        }
    }
}
