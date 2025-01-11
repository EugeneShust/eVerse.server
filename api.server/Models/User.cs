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

        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> Favorites { get; set; } = [];
    }
}
