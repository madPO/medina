global using MedinaBuilder = (Microsoft.Extensions.DependencyInjection.IServiceCollection collection, Medina.Core.Descriptors.IDescriptorStore store);
using Medina.Core.Descriptors;
using Medina.Core.Memory;
using Medina.Core.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Medina.Core.Extensions;

public static class IServiceCollectionExtensions
{
    public static MedinaBuilder AddMedina(this IServiceCollection collection, Action<MedinaOptions>? configure = null)
    {
        var store = new DescriptorStore();
        collection.AddSingleton<IDescriptorStore>(store);
        collection.AddSingleton<EventRouter>();
        collection.AddTransient<IDistributedContext, DistributedContext>();
        collection.AddSingleton<MemoryEventStore>();
        collection.AddHostedService<MemoryQueueWorker>();
        collection.AddSingleton<Func<Type, IEventStore>>((provider) =>
        {
            return (type) => (IEventStore) provider.GetRequiredService(type);
        });
        collection.Configure<MedinaOptions>((opt) =>
        {
            opt.Source = new Uri("app:"+Environment.MachineName.ToLower());
            configure?.Invoke(opt);
        });

        return (collection, store);
    }
}