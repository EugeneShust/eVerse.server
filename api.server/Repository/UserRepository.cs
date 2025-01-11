using API.Server.Models;
using Core.Mongo;
using MongoDB.Driver;

namespace API.Server.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IMongoDatabase database) : base(database)
        {
        }
    }
}
