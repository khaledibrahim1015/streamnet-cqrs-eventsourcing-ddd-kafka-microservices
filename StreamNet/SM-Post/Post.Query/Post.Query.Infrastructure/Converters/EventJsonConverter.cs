using CQRS.Core.Events;
using Post.Common.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Post.Query.Infrastructure.Converters;

public class EventJsonConverter : JsonConverter<BaseEvent>
{

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsAssignableFrom(typeof(BaseEvent));
    }
    //  in Deserialization
    //  Read Json and Convert it To concrete BaseEvent 
    //  read JSON data and figure out what type of event it represents, then create the right kind of event object
    public override BaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out var doc))
        {
            throw new JsonException($"Failed to parse {nameof(JsonDocument)}!");
        }
        //looking for a property named "Type" in the JSON root element, not "EventType"
        // considiration 
        //The EventType from your EventModel is not directly used in the JSON deserialization process. Instead, when the event is serialized to JSON (which happens when it's sent to Kafka), the actual type of the @event object is typically included in the JSON as a "Type" property.
        //This "Type" property is what the EventJsonConverter is looking for when deserializing.It's not directly related to the EventType property in your EventModel

        if (!doc.RootElement.TryGetProperty("Type", out var type))
        {
            throw new JsonException("Could not detect the Type discriminator property!");
        }



        // The result is stored in typeDiscriminator, which will contain the name of the event type (like "PostCreatedEvent", "MessageUpdatedEvent", etc.)
        var typeDiscriminator = type.GetString();

        /*
         *  var json = doc.RootElement.GetRawText();
            This line is getting the entire JSON content as a raw string. Let's break it down:
            doc is the JsonDocument that was parsed from the input.
            RootElement refers to the top-level element of the JSON document.
            GetRawText() is a method that returns the entire JSON content as a string, exactly as it was in the original input.
            This raw JSON string is stored in the json variable.
         * 
         */
        var json = doc.RootElement.GetRawText();

        return typeDiscriminator switch
        {
            nameof(PostCreatedEvent) => JsonSerializer.Deserialize<PostCreatedEvent>(json, options),
            nameof(MessageUpdatedEvent) => JsonSerializer.Deserialize<MessageUpdatedEvent>(json, options),
            nameof(PostLikeEvent) => JsonSerializer.Deserialize<PostLikeEvent>(json, options),
            nameof(CommentAddedEvent) => JsonSerializer.Deserialize<CommentAddedEvent>(json, options),
            nameof(CommentUpdatedEvent) => JsonSerializer.Deserialize<CommentUpdatedEvent>(json, options),
            nameof(CommentRemovedEvent) => JsonSerializer.Deserialize<CommentRemovedEvent>(json, options),
            nameof(PostRemovedEvent) => JsonSerializer.Deserialize<PostRemovedEvent>(json, options),
            _ => throw new JsonException($"{typeDiscriminator} is not supported yet!")
        };

    }


    // in Serialization
    public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
