using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Core.Mongo;

namespace API.Server.Models
{
    public class Account: ModelBase
    {
        public required string FirebaseUid { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
