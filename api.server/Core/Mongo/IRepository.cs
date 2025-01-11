using MongoDB.Driver;
using System.Linq.Expressions;

namespace Core.Mongo
{
    public interface IRepository
    {
        IMongoDatabase Database { get; }
    }

    public interface IRepository<TModel> where TModel : ModelBase
    {
        IMongoCollection<TModel> Collection { get; }

        Task CreateAsync(TModel model);

        Task<TModel> FindOneAsync(Expression<Func<TModel, bool>> filter);

        Task<List<TModel>> FindManyAsync(Expression<Func<TModel, bool>> filter);
        Task<List<TProjection>> FindManyAsync<TProjection>(Expression<Func<TModel, bool>> filter,
            Expression<Func<TModel, TProjection>> projection);

        Task<UpdateResult> UpdateOneAsync(Expression<Func<TModel, bool>> filter, params UpdateDefinition<TModel>[] updates);

        Task<long> CountAsync();
    }
}
