using Core.Mongo;
using API.Server.Models;

namespace API.Server.Repository
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetByFirebaseUidAsync(string firebaseUid);
    }
}
