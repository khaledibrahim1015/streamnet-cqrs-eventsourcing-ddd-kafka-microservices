using Microsoft.EntityFrameworkCore;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRequiredService(this IServiceCollection services, IConfiguration configuration)
        {
            Action<DbContextOptionsBuilder> configureDbContext = o => o.UseLazyLoadingProxies().UseSqlServer(configuration.GetConnectionString("SqlServer"));
            services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDbContext));

            //services.AddDbContext<DatabaseContext>(configureDbContext);
            services.AddScoped(serviceProvider =>
            {
                var databaseCtxFactory = serviceProvider.GetRequiredService<DatabaseContextFactory>();
                return databaseCtxFactory.CreateDbContext();
            });



            return services;
        }
    }
}
