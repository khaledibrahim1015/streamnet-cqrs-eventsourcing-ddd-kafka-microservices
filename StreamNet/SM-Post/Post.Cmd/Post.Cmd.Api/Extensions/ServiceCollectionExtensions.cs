using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.Commands.interfaces;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Configuraion;
using Post.Cmd.Infrastructure.Dispatchers;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Producers;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;

namespace Post.Cmd.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRequiredServices(this IServiceCollection services, IConfiguration configuration)
        {
            //  Add Services to The Conatainer 
            services.Configure<MongoDbConfiguration>(configuration.GetSection(nameof(MongoDbConfiguration)));
            services.Configure<ProducerConfig>(configuration.GetSection(nameof(ProducerConfig)));

            services.AddScoped<IEventStoreRepository, EventStoreRepository>();
            services.AddScoped<IEventProducer, EventProducer>();

            services.AddScoped<IEventStore, EventStore>();
            services.AddScoped<IEventSourceHandler<PostAggregate>, EventSourceHandler>();
            services.AddScoped<ICommandHandler, CommandHandler>();

            //  Register Commands Handler Method 
            var commandHandler = services.BuildServiceProvider().GetRequiredService<ICommandHandler>();
            var dispatcher = new CommandDispatcher();
            dispatcher.RegisterHandler<NewPostCommand>(commandHandler.HandleAsync);
            dispatcher.RegisterHandler<EditMessageCommand>(commandHandler.HandleAsync);
            dispatcher.RegisterHandler<LikePostCommand>(commandHandler.HandleAsync);
            dispatcher.RegisterHandler<AddCommentCommand>(commandHandler.HandleAsync);
            dispatcher.RegisterHandler<EditCommentCommand>(commandHandler.HandleAsync);
            dispatcher.RegisterHandler<RemoveCommentCommand>(commandHandler.HandleAsync);
            dispatcher.RegisterHandler<DeletePostCommand>(commandHandler.HandleAsync);
            services.AddSingleton<ICommandDispatcher>(_ => dispatcher);



            return services;
        }

    }
}
