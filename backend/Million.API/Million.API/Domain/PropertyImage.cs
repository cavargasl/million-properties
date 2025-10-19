using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Million.API.Domain
{
    public class PropertyImage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdPropertyImage { get; set; } = string.Empty;

        [BsonElement("idProperty")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdProperty { get; set; } = string.Empty;

        [BsonElement("file")]
        public string File { get; set; } = string.Empty;

        [BsonElement("enabled")]
        public bool Enabled { get; set; } = true;

        // Navigation property (not stored in MongoDB)
        [BsonIgnore]
        public Property? Property { get; set; }
    }
}
