using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Entities;
using Play.Common.Settings;

namespace Play.Common.Repositories
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));


            _ = services.AddSingleton(provider =>
            {
                var configuration = provider.GetService<IConfiguration>();
                ServiceSettings serviceSettings =
          configuration.GetSection(nameof(ServiceSettings))
                       .Get<ServiceSettings>();
                var mongoDbSettings =
           configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });

            return services;
        }
        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName)
        where T : IEntity
        {
            services.AddSingleton<IRepository<T>>(provider =>
                       {

                           var database = provider.GetService<IMongoDatabase>();
                           return new MongoRepository<T>(database, collectionName);
                       });
            return services;
        }
    }
}