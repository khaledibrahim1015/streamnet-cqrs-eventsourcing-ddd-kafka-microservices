using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;

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

            //  Create Database From Code 
            var dbContext = services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
            dbContext.Database.EnsureCreated();


            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommenRepository, CommentRepository>();
            services.AddScoped<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();

            services.Configure<ConsumerConfig>(configuration.GetSection(nameof(ConsumerConfig)));

            services.AddHostedService<ConsumerHostedService>();
            return services;
        }
    }
}
