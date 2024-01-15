using System.Reflection;
using CloudNative.CloudEvents;
using Medina.Core.Attributes;
using Medina.Core.Descriptors;
using Medina.Core.Memory;
using Medina.Core.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Medina.Core.Extensions;

public static class IHostExtensions
{
    public static IHost UseOperation(this IHost host, EventDelegate handler, Action<OperationDescriptorOption> configure)
    {
        var store = host.Services.GetRequiredService<IDescriptorStore>();
        
        var did = UUIDNext.Uuid.NewRandom().ToString();
        var option = new OperationDescriptorOption
        {
            Did = did,
            Alternative = did,
            EventType = did,
        };

        configure(option);
        
        var descriptor = new OperationDescriptor(option.Did, option.Alternative, option.EventType, handler);
        store.Add(descriptor);

        return host;
    }
    
    private record ArgumentFactory(int position, Func<CloudEvent, CancellationToken, object?> factory);  
        
    public static IHost UseOperation(this IHost host, Delegate handler, Action<OperationDescriptorOption> configure)
    {
        var container = host.Services;
        var store = container.GetRequiredService<IDescriptorStore>();
        
        var did = UUIDNext.Uuid.NewRandom().ToString();
        var option = new OperationDescriptorOption
        {
            Did = did,
            Alternative = did,
            EventType = did,
        };

        configure(option);              
        
        // todo: тут нужен функционал по аналогии с IModelBinder
        // todo: желательно уйти от рефлексии
        var methodInfo = handler.Method;
        var arguments = methodInfo.GetParameters();
        var factoryList = new List<ArgumentFactory>();

        foreach (var argument in arguments)
        {
            var attributes = argument.GetCustomAttributes();

            if (attributes.Any(x => x.GetType() == typeof(FromDataAttribute)))
            {
                factoryList.Add(new (argument.Position, (e, _) => e.Data));
                continue;
            }

            if (attributes.Any(x => x.GetType() == typeof(FromServicesAttribute)))
            {
                factoryList.Add(new (argument.Position, (_, _) => container.GetRequiredService(argument.ParameterType)));
                continue;
            }

            if (argument.ParameterType.IsAssignableTo(typeof(CloudEvent)))
            {
                factoryList.Add(new (argument.Position, (e, _) => e));
                continue;
            } 
            
            if (argument.ParameterType.IsAssignableTo(typeof(CancellationToken)))
            {
                factoryList.Add(new (argument.Position, (_, token) => token));
                continue;
            }  
            
            factoryList.Add(new (argument.Position, (e, _) => default));
        }
        
        async Task<CloudEvent?> EventHandler(CloudEvent @event, CancellationToken cancellationToken = default)
        {
            var result = handler
                .DynamicInvoke(factoryList
                    .OrderBy(x => x.position)
                    .Select(x => x.factory(@event, cancellationToken))
                    .ToArray());

            if (result is null)
            {
                return null;
            }      
            
            if (!handler.Method.ReturnType.IsAssignableTo(typeof(Task)))
            {
                return (CloudEvent?) result;
            }

            if (handler.Method.ReturnType.IsConstructedGenericType)
            {
                return await (Task<CloudEvent?>) result;
            }

            await (Task) result;  
            return null;
        };
        
        var descriptor = new OperationDescriptor(option.Did, option.Alternative, option.EventType, EventHandler);
        store.Add(descriptor);

        return host;
    }

    public static IHost UseCallback(this IHost host, Action<OperationDescriptorOption> configure)
    {
        var container = host.Services;
        var store = container.GetRequiredService<IDescriptorStore>();
        
        var did = UUIDNext.Uuid.NewRandom().ToString();
        var option = new OperationDescriptorOption
        {
            Did = did,
            Alternative = did,
            EventType = did,
        };

        configure(option);

        async Task<CloudEvent?> CallbackHandler(CloudEvent @event, CancellationToken cancellationToken = default)
        {
            var eventStore = container.GetRequiredService<MemoryEventStore>();

            await eventStore.ReleaseCallbackAsync(@event, cancellationToken);

            return null;
        }

        var descriptor = new OperationDescriptor(option.Did, option.Alternative, option.EventType, CallbackHandler);
        store.Add(descriptor);

        return host;
    }
}