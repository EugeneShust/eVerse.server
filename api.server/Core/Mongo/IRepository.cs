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
        Task<long> CountAsync();
    }
}
