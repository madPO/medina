using CloudNative.CloudEvents;
using Medina.Core.Descriptors;

namespace Medina.Core;

internal sealed class EventRouter
{
    private readonly IDescriptorStore _store;
    
    private readonly Func<Type, IEventStore> _storeFactory;

    public async Task RouteEventAsync(CloudEvent cloudEvent, CancellationToken cancellationToken = default)
    {
        var descriptors = _store.ReadAll()
            .Where(x => x.CanHandle(cloudEvent))
            .GroupBy(x => x.QueueType)
            .ToArray();

        foreach (var group in descriptors)
        {
            var queueType = group.Key;
            var items = group.Select(x => x).ToArray();

            var store = _storeFactory(queueType);
            foreach (ItemDescriptor item in items)
            {
                await store.PutAsync(cloudEvent, item, cancellationToken);
            }
        }
    }

    public Task<CloudEvent?> RequestCallback(CloudEvent cloudEvent, CancellationToken cancellationToken = default)
    {
        var storeType = _store.ReadAll()
            .Where(x => x.CanHandle(cloudEvent))
            .Select(x => x.QueueType)
            .Distinct()
            .Where(x => x.IsAssignableTo(typeof(ICallbackEventStore)))
            .FirstOrDefault();

        if (storeType is null)
        {
            // возможно нужно выбрасывать исключение
            return Task.FromResult<CloudEvent?>(null);
        }
        
        var store = (ICallbackEventStore) _storeFactory(storeType);
        var callback = store.ResolveCallback(cloudEvent, cancellationToken);

        return callback;
    }

    public EventRouter(IDescriptorStore store, Func<Type, IEventStore> storeFactory)
    {
        _store = store;
        _storeFactory = storeFactory;
    }
}