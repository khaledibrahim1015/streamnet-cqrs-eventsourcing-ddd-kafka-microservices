using Post.Cmd.Infrastructure.Configuraion;

namespace Post.Cmd.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRequiredServices(this IServiceCollection services, IConfiguration configuration)
        {
            //  Add Services to The Conatainer 
            services.Configure<MongoDbConfiguration>(configuration.GetSection(nameof(MongoDbConfiguration)));

            return services;
        }

    }
}
