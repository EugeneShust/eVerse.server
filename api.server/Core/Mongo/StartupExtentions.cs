using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Core.Mongo
{
    public static class StartupExtentions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbOptions>(configuration.GetSection("MongoDB"));

            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            services.AddScoped(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(options.DatabaseName);
            });

            return services;
        }

        public static void AddRepository<TRepository>(this IServiceCollection services)
        where TRepository : class, IRepository
        {
            services.AddScoped<TRepository>();

            var interfaces = typeof(TRepository).GetInterfaces();
            foreach (var @interface in interfaces)
            {
                services.AddScoped(@interface, typeof(TRepository));
            }
        }
    }
}
