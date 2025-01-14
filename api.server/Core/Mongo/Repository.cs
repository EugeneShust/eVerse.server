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
        public UpdateDefinitionBuilder<TModel> Update => Builders<TModel>.Update;

        public async Task CreateAsync(TModel model)
        {
            await Collection.InsertOneAsync(model);
        }

        public Task<TModel> FindOneAsync(Expression<Func<TModel, bool>> filter)
        {
            return Collection.Find(filter).FirstOrDefaultAsync();
        }

        public Task<List<TModel>> FindManyAsync(Expression<Func<TModel, bool>> filter)
        {
            return Collection.Find(filter).ToListAsync();
        }

        public Task<List<TProjection>> FindManyAsync<TProjection>(Expression<Func<TModel, bool>> filter, Expression<Func<TModel, TProjection>> projection)
        {
            return Collection.Find(filter).Project(projection).ToListAsync();
        }

        public Task<UpdateResult> UpdateOneAsync(Expression<Func<TModel, bool>> filter, params UpdateDefinition<TModel>[] updates)
        {
            var update = Update.Combine(updates);

            return Collection.UpdateOneAsync(filter, update);
        }

        public Task<ReplaceOneResult> ReplaceOneAsync(Expression<Func<TModel, bool>> filter, TModel replacement)
        {
            return Collection.ReplaceOneAsync(filter, replacement);
        }

        public Task<long> CountAsync()
        {
            throw new NotImplementedException();
        }
    }
}