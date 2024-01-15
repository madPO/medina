using CloudNative.CloudEvents;
using Medina.Core.Memory;

namespace Medina.Core.Descriptors;

/// <summary>
/// Описатель операции
/// </summary>
public class OperationDescriptor: ItemDescriptor<MemoryEventStore>
{       
    /// <summary>
    /// Тип события
    /// </summary>
    public string EventType { get; }
    
    /// <summary>
    /// Обработчик
    /// </summary>
    public EventDelegate Handler { get; }

    public override bool CanHandle(CloudEvent @event)
    {
        return @event.Type == EventType;
    }

    /*public override Task<CloudEvent?> HandleAsync(CloudEvent @event, CancellationToken cancellationToken = default)
    {
        return Handler.Invoke(@event, cancellationToken);
    }*/

    public OperationDescriptor(string id, string alternative, string eventType, EventDelegate handler) : base(id, alternative)
    {
        Handler = handler;
        EventType = eventType;
    }
}