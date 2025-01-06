using MongoDB.Driver;
using System.Linq.Expressions;

namespace Core.Mongo
{
    public abstract class Repository<TModel> : IRepository
        where TModel : ModelBase
    {
        public Repository(IMongoDatabase database)
        {
            Collection = GetCollection(database);
        }

        private static IMongoCollection<TModel> GetCollection(IMongoDatabase database)
        { 
            return database.GetCollection<TModel>(GetCollectionName(), new MongoCollectionSettings
            {
                ReadPreference = ReadPreference.Primary
            });
        }

        private static string GetCollectionName()
        {
            return typeof(TModel).Name;
        }

        public IMongoCollection<TModel> Collection { get; }
        public IMongoDatabase Database => Collection.Database;
        
        public async Task CreateAsync(TModel model)
        {
            await Collection.InsertOneAsync(model);
        }

        public Task<TModel> FindOneAsync(Expression<Func<TModel, bool>> filter)
        {
            return Collection.Find(filter).FirstOrDefaultAsync();
        }

        public Task<long> CountAsync()
        {
            throw new NotImplementedException();
        }
    }
}