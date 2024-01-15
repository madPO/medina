using CloudNative.CloudEvents;
using Medina.Core.Descriptors;

namespace Medina.Core;

/// <summary>
/// Стор событий
/// </summary>
public interface IEventStore
{
    /// <summary>
    /// Отправить событие в стор
    /// </summary>
    public Task PutAsync(CloudEvent cloudEvent, ItemDescriptor descriptor, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отметить событие как принятое
    /// </summary>
    public Task AckAsync(CloudEvent cloudEvent, CancellationToken cancellationToken = default);
}

public interface ICallbackEventStore
{                                                                                                                  
    /// <summary>
    /// Запросить из стора обратный вызов для события
    /// </summary>
    public Task<CloudEvent?> ResolveCallback(CloudEvent cloudEvent, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Освободить обратный вызов
    /// </summary>
    public Task ReleaseCallbackAsync(CloudEvent cloudEvent, CancellationToken cancellationToken = default);
    
}