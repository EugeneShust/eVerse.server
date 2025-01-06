using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Core.Mongo
{
    public abstract class ModelBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
