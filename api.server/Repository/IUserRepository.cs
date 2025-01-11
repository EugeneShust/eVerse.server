using API.Server.Models;
using Core.Mongo;

namespace API.Server.Repository
{
    public interface IUserRepository : IRepository<User>
    {
    }
}
