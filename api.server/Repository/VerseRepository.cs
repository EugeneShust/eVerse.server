using API.Server.Models;
using Core.Mongo;
using MongoDB.Driver;

namespace API.Server.Repository
{
    public class VerseRepository : Repository<Verse>, IVerseRepository
    {
        public VerseRepository(IMongoDatabase database) : base(database)
        {
        }
    }
}