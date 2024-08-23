using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Dispatchers;
using Post.Query.Api.Queries;
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
            services.AddScoped<IQueryHandler, QueryHandler>();
            services.Configure<ConsumerConfig>(configuration.GetSection(nameof(ConsumerConfig)));

            services.AddHostedService<ConsumerHostedService>();

            // Register QueryHandlers Method 
            var queryHandler = services.BuildServiceProvider().GetRequiredService<IQueryHandler>();
            var queryDispatcher = new QueryDispatcher();
            queryDispatcher.RegisterHandler<FindAllPostsQuery>(queryHandler.HandleAsync);
            queryDispatcher.RegisterHandler<FindPostByIdQuery>(queryHandler.HandleAsync);
            queryDispatcher.RegisterHandler<FindPostsWithAuthorQuery>(queryHandler.HandleAsync);
            queryDispatcher.RegisterHandler<FindPostsWithCommentsQuery>(queryHandler.HandleAsync);
            queryDispatcher.RegisterHandler<FindPostsWithLikesQuery>(queryHandler.HandleAsync);





            return services;
        }
    }
}
