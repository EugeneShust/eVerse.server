using Core.Mongo;
using API.Server.Models;
using MongoDB.Driver;

namespace API.Server.Repository
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(IMongoDatabase database) : base(database)
        {
        }

        public async Task<Account> GetByFirebaseUidAsync(string firebaseUid)
        {
            return await Collection.Find(a => a.FirebaseUid == firebaseUid).FirstOrDefaultAsync();
        }
    }
}
