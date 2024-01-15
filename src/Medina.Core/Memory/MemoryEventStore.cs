using System.Collections.Concurrent;
using CloudNative.CloudEvents;
using Medina.Core.Descriptors;

namespace Medina.Core.Memory;

public sealed class MemoryEventStore : IEventStore, ICallbackEventStore
{
    private readonly ConcurrentQueue<(CloudEvent cloudEvent, ItemDescriptor descriptor)> _queue;

    // todo: clear it by timeout
    private readonly ConcurrentDictionary<string, TaskCompletionSource<CloudEvent?>> _completionSources;

    private readonly SemaphoreSlim _semaphore;

    public Task PutAsync(CloudEvent cloudEvent, ItemDescriptor descriptor,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _queue.Enqueue((cloudEvent, descriptor));
        _semaphore.Release(1);

        return Task.CompletedTask;
    }

    public Task<CloudEvent?> ResolveCallback(CloudEvent cloudEvent, CancellationToken cancellationToken = default)
    {
        var source = new TaskCompletionSource<CloudEvent?>();
        _completionSources.TryAdd(cloudEvent.Id, source);

        return source.Task;
    }
    
    public Task ReleaseCallbackAsync(CloudEvent cloudEvent, CancellationToken cancellationToken = default)
    {
        var callBackId = cloudEvent["CallbackId"]?.ToString();

        if (callBackId is null)
        {
            // todo: нужно логировать такие случаи
            return Task.CompletedTask;
        }
        
        if (_completionSources.TryRemove(callBackId, out var source))
        {
            source.TrySetResult(cloudEvent);
        }
        // todo: else тут тоже нужно логировать
        
        return Task.CompletedTask;
    }

    public Task AckAsync(CloudEvent cloudEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    
    public (CloudEvent cloudEvent, ItemDescriptor descriptor, bool hasItem) Dequeue()
    {
        var hasItem = _queue.TryDequeue(out var item);

        return (item.cloudEvent, item.descriptor, hasItem);
    }

    public Task WaitNewEvent(CancellationToken cancellationToken)
    {
        return _semaphore.WaitAsync(cancellationToken);
    }

    public MemoryEventStore()
    {
        _queue = new ConcurrentQueue<(CloudEvent cloudEvent, ItemDescriptor descriptor)>();
        _completionSources = new ConcurrentDictionary<string, TaskCompletionSource<CloudEvent?>>();
        _semaphore = new SemaphoreSlim(0);
    }
}