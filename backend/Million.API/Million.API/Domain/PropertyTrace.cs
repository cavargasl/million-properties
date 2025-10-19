using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Million.API.Domain
{
    public class PropertyTrace
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdPropertyTrace { get; set; } = string.Empty;

        [BsonElement("idProperty")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdProperty { get; set; } = string.Empty;

        [BsonElement("dateSale")]
        public DateTime DateSale { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("value")]
        public decimal Value { get; set; }

        [BsonElement("tax")]
        public decimal Tax { get; set; }

        // Navigation property (not stored in MongoDB)
        [BsonIgnore]
        public Property? Property { get; set; }
    }
}
