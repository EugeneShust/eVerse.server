using Core.Mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.Server.Models
{
    public class User : ModelBase
    {
        public string Avatar { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Verses { get; set; } = [];

        public List<Favorite> Favorites { get; set; } = [];
    }

    public class Favorite
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Events { get; set; } = [];
    }
}
