using ASP.MongoDb.API.Repository;
using Microsoft.Extensions.Options;
using Million.API.Domain;
using Million.API.Settings;
using MongoDB.Driver;

namespace Million.API.Repository
{
    public class OwnerRepository : Repository<Owner>
    {
        public OwnerRepository(IOptions<MongoDbSettings> settings) : base(settings)
        {
        }

        public async Task<IEnumerable<Owner>> FindByNameAsync(string name)
        {
            var filter = Builders<Owner>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(name, "i"));
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<(IEnumerable<Owner> Items, long TotalCount)> FindByNamePaginatedAsync(
            string name, 
            int pageNumber, 
            int pageSize)
        {
            var filter = Builders<Owner>.Filter.Regex(x => x.Name, new MongoDB.Bson.BsonRegularExpression(name, "i"));
            var totalCount = await _collection.CountDocumentsAsync(filter);

            var items = await _collection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Owner>> FindByAddressAsync(string address)
        {
            var filter = Builders<Owner>.Filter.Regex(x => x.Address, new MongoDB.Bson.BsonRegularExpression(address, "i"));
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<(IEnumerable<Owner> Items, long TotalCount)> FindByAddressPaginatedAsync(
            string address, 
            int pageNumber, 
            int pageSize)
        {
            var filter = Builders<Owner>.Filter.Regex(x => x.Address, new MongoDB.Bson.BsonRegularExpression(address, "i"));
            var totalCount = await _collection.CountDocumentsAsync(filter);

            var items = await _collection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Owner>> FindByAgeRangeAsync(int minAge, int maxAge)
        {
            var minDate = DateTime.Now.AddYears(-maxAge);
            var maxDate = DateTime.Now.AddYears(-minAge);
            
            var filter = Builders<Owner>.Filter.And(
                Builders<Owner>.Filter.Gte(x => x.Birthday, minDate),
                Builders<Owner>.Filter.Lte(x => x.Birthday, maxDate)
            );
            
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<(IEnumerable<Owner> Items, long TotalCount)> FindByAgeRangePaginatedAsync(
            int minAge, 
            int maxAge, 
            int pageNumber, 
            int pageSize)
        {
            var minDate = DateTime.Now.AddYears(-maxAge);
            var maxDate = DateTime.Now.AddYears(-minAge);

            var filter = Builders<Owner>.Filter.And(
                Builders<Owner>.Filter.Gte(x => x.Birthday, minDate),
                Builders<Owner>.Filter.Lte(x => x.Birthday, maxDate)
            );

            var totalCount = await _collection.CountDocumentsAsync(filter);

            var items = await _collection.Find(filter)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            var filter = Builders<Owner>.Filter.Eq(x => x.Name, name);
            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }
    }
}
