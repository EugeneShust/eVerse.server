using Core.Mongo;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using static API.Server.Models.BaseItem;

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


        public List<Category> Categories { get; set; } = [];
        public List<Location> Locations { get; set; } = [];
        public List<Presenter> Presenters { get; set; } = [];
        public List<Event> Events { get; set; } = [];

        public List<Participant> Participants { get; set; } = [];
    }

    public class Participant
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public bool IsActive { get; set; } = true;
    }

    public class BaseItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Event : BaseItem
    {
        public string LocationId { get; set; }
        public string CategoryId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<string> PresenterIds { get; set; } = [];
    }

    public class Location : BaseItem
    {
        public LocationCoords Coords { get; set; }
    }

    public class Presenter : BaseItem { }

    public class Category : BaseItem { }

    public class LocationCoords
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
