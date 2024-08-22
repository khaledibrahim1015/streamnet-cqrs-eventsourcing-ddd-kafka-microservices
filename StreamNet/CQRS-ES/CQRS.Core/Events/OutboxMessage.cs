using CQRS.Core.Enums;
using CQRS.Core.Messages;

namespace CQRS.Core.Events;

public class OutboxMessage : Message
{
    public string Topic { get; set; }
    public string Payload { get; set; }
    public string PayloadType { get; set; }
    public DateTime CreatedAt { get; set; }
    public OutboxMessageStatus Status { get; set; }
}
