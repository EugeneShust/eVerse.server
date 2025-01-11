using Core.Mongo;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace API.Server.Models
{
    public class Verse : ModelBase
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public string AuthorId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Logo { get; set; }
        public string Map { get; set; }

        public List<Event> Events { get; set; } = [];
        public List<Location> Locations { get; set; } = [];
        public List<Category> Categories { get; set; } = [];
        public List<Presenter> Presenters { get; set; } = [];

    }

    public class Event
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string EventId { get; set; } = ObjectId.GenerateNewId().ToString();

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public string LocationId { get; set; }
        public string CategoryId { get; set; }
        public List<string> PresenterIds { get; set; } = [];
    }
    
    public class Location
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string LocationId { get; set; } = ObjectId.GenerateNewId().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public LocationCoords Coords { get; set; } 
    }

    public class Presenter
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PresenterId { get; set; } = ObjectId.GenerateNewId().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; } = ObjectId.GenerateNewId().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class LocationCoords
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int X { get; set; } 
        public int Y { get; set; }
    }
}
