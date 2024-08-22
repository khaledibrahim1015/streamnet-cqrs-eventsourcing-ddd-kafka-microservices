using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;
using System.Text.Json;

namespace Post.Query.Infrastructure.Consumers;

public class EventConsumer : IEventConsumer
{
    private readonly ConsumerConfig _config;
    private readonly IEventHandler _eventHandler;
    public EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler)
    {
        _config = config.Value;
        _eventHandler = eventHandler;
    }

    public void Consume(string topic)
    {
        var consumer = new ConsumerBuilder<string, string>(_config)
                         .SetKeyDeserializer(Deserializers.Utf8)
                         .SetValueDeserializer(Deserializers.Utf8)
                         .Build();

        consumer.Subscribe(topic);

        while (true)
        {
            var consumeResult = consumer.Consume();
            if (consumeResult?.Message == null) continue;

            var jsonOptions = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };

            var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, jsonOptions);

            //  using Refliction Call On Handle Method 
            var handlerMthod = _eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });
            if (handlerMthod == null)
                throw new ArgumentNullException(nameof(handlerMthod), "Could not find event handler method!");

            handlerMthod.Invoke(_eventHandler, new object[] { @event });

            consumer.Commit(consumeResult);
        }



    }
}
