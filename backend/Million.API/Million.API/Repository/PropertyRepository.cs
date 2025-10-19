using ASP.MongoDb.API.Repository;
using Microsoft.Extensions.Options;
using Million.API.Domain;
using Million.API.Settings;
using MongoDB.Driver;

namespace Million.API.Repository
{

    public class PropertyRepository : Repository<Property>
    {
        public PropertyRepository(IOptions<MongoDbSettings> settings) : base(settings)
        {
        }

        public async Task<IEnumerable<Property>> SearchPropertiesAsync(
            string? name = null, 
            string? address = null, 
            decimal? minPrice = null, 
            decimal? maxPrice = null)
        {
            var filter = BuildSearchFilter(name, address, minPrice, maxPrice);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<(IEnumerable<Property> Items, long TotalCount)> SearchPropertiesPaginatedAsync(
            string? name = null,
            string? address = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var filter = BuildSearchFilter(name, address, minPrice, maxPrice);
            var totalCount = await _collection.CountDocumentsAsync(filter);

            var items = await _collection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        private FilterDefinition<Property> BuildSearchFilter(
            string? name = null,
            string? address = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            var filters = new List<FilterDefinition<Property>>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                filters.Add(Builders<Property>.Filter.Regex(
                    x => x.Name,
                    new MongoDB.Bson.BsonRegularExpression(name, "i")
                ));
            }

            if (!string.IsNullOrWhiteSpace(address))
            {
                filters.Add(Builders<Property>.Filter.Regex(
                    x => x.Address,
                    new MongoDB.Bson.BsonRegularExpression(address, "i")
                ));
            }

            if (minPrice.HasValue)
            {
                filters.Add(Builders<Property>.Filter.Gte(x => x.Price, minPrice.Value));
            }

            if (maxPrice.HasValue)
            {
                filters.Add(Builders<Property>.Filter.Lte(x => x.Price, maxPrice.Value));
            }

            return filters.Count > 0
                ? Builders<Property>.Filter.And(filters)
                : Builders<Property>.Filter.Empty;
        }

        public async Task<(IEnumerable<Property> Items, long TotalCount)> GetByOwnerIdPaginatedAsync(
            string ownerId, 
            int pageNumber, 
            int pageSize)
        {
            var filter = Builders<Property>.Filter.Eq(x => x.IdOwner, ownerId);
            var totalCount = await _collection.CountDocumentsAsync(filter);

            var items = await _collection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Property>> GetByOwnerIdAsync(string ownerId)
        {
            var filter = Builders<Property>.Filter.Eq(x => x.IdOwner, ownerId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var filter = Builders<Property>.Filter.And(
                Builders<Property>.Filter.Gte(x => x.Price, minPrice),
                Builders<Property>.Filter.Lte(x => x.Price, maxPrice)
            );
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<bool> ExistsByCodeAsync(string codeInternal)
        {
            var filter = Builders<Property>.Filter.Eq(x => x.CodeInternal, codeInternal);
            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        public async Task<IEnumerable<Property>> GetByYearAsync(int year)
        {
            var filter = Builders<Property>.Filter.Gte(x => x.Year, year);
            return await _collection.Find(filter).ToListAsync();
        }
    }
}
