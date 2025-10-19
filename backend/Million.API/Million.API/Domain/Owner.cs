using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Million.API.Domain
{
    /// <summary>
    /// Owner entity - represents property owners in the system
    /// </summary>
    public class Owner
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdOwner { get; set; } = string.Empty;

        [BsonElement("name")]
        [Required]
        public string Name { get; set; } = string.Empty;

        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("photo")]
        public string? Photo { get; set; }

        [BsonElement("birthday")]
        [Required]
        public DateTime Birthday { get; set; }
       
    }

}
