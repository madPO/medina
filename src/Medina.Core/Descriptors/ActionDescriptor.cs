namespace Medina.Core.Descriptors;

/// <summary>
/// Описатель обратного вызова
/// </summary>
public sealed class CallbackDescriptor: OperationDescriptor
{
    public CallbackDescriptor(string id, string alternative, string eventType, EventDelegate handler) : base(id, alternative, eventType, handler)
    {
    }
}