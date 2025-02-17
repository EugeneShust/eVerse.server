using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

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

        public static IServiceCollection AddGridFS(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IGridFSBucket>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;

                var mongoClient = sp.GetRequiredService<IMongoClient>();
                var database = mongoClient.GetDatabase(options.DatabaseName);

                return new GridFSBucket(database, new GridFSBucketOptions
                {
                    BucketName = "fs",
                    ChunkSizeBytes = 255 * 1024, // 255 KB
                    WriteConcern = WriteConcern.WMajority,
                    ReadPreference = ReadPreference.Primary
                });
            });

            return services;
        }
    }
}
