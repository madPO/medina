using CloudNative.CloudEvents;
using Medina.Core.Options;
using Microsoft.Extensions.Options;
using UUIDNext;

namespace Medina.Core;

/// <summary>
/// Распределенный контекст
/// </summary>
public interface IDistributedContext
{
    /// <summary>
    /// Опубликовать событие
    /// </summary>
    Task PublishAsync(CloudEvent @event, CancellationToken cancellationToken = default);

    /// <summary>
    /// Выполнить операцию
    /// </summary>
    Task<CloudEvent?> ExecuteAsync(CloudEvent @event, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать событие
    /// </summary>
    CloudEvent CreateEvent(object? data, string type);
}

internal sealed class DistributedContext : IDistributedContext
{
    private readonly EventRouter _router;
    
    private readonly IOptions<MedinaOptions> _options;

    public async Task PublishAsync(CloudEvent @event, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
            
        await _router.RouteEventAsync(@event, cancellationToken);
    }

    public async Task<CloudEvent?> ExecuteAsync(CloudEvent @event, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        await _router.RouteEventAsync(@event, cancellationToken);
        var result = await _router.RequestCallback(@event, cancellationToken);

        return result;
    }

    public CloudEvent CreateEvent(object? data, string type)
    {
        return new CloudEvent
        {
            Id = Uuid.NewSequential().ToString(),
            Data = data,
            Time = DateTimeOffset.UtcNow,
            DataContentType = "application/" + data?.GetType().FullName?.ToLower(),
            Type = type,
            Source = _options.Value.Source,
        };
    }

    public DistributedContext(EventRouter router, IOptions<MedinaOptions> options)
    {
        _router = router;
        _options = options;
    }
}