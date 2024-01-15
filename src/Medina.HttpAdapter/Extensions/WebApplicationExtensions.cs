using CloudNative.CloudEvents;
using Medina.Core;
using Medina.Core.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UUIDNext;

namespace Medina.HttpAdapter.Extensions;

public static class WebApplicationExtensions
{
    public static RouteHandlerBuilder UseHttpEntrypoint<TModel>(this WebApplication host, string pattern, string[] methods, Action<CloudEvent>? configure = null)
    {
        async Task<IResult> Handler([AsParameters] TModel model, [FromServices] IDistributedContext context, IOptions<MedinaOptions> options, CancellationToken cancellationToken = default)
        {
            var @event = new CloudEvent
            {
                Id = Uuid.NewSequential().ToString(),
                Data = model,
                Source = options.Value?.Source,
                Time = DateTimeOffset.UtcNow,
                DataContentType = "application/" + typeof(TModel).FullName?.ToLower(),
            };
            
            configure?.Invoke(@event);

            var response = await Guard.Operation(async () =>
            {
                var output = await context.ExecuteAsync(@event, cancellationToken);

                return output?.Data;
            });

            return response.ToActionResult();
        }
        
        return host.MapMethods(pattern, methods, Handler);
    }

    public static RouteHandlerBuilder UsePostEntrypoint<TModel>(this WebApplication host, string pattern, Action<CloudEvent>? configure = null)
    {
        return UseHttpEntrypoint<TModel>(host, pattern, new[] {HttpMethods.Post}, configure);
    }

    public static RouteHandlerBuilder UseGetEntrypoint<TModel>(this WebApplication host, string pattern, Action<CloudEvent>? configure = null)
    {
        return UseHttpEntrypoint<TModel>(host, pattern, new[] {HttpMethods.Get}, configure);
    }

    public static RouteHandlerBuilder UsePutEntrypoint<TModel>(this WebApplication host, string pattern, Action<CloudEvent>? configure = null)
    {
        return UseHttpEntrypoint<TModel>(host, pattern, new[] {HttpMethods.Put}, configure);
    }

    public static RouteHandlerBuilder UseDeleteEntrypoint<TModel>(this WebApplication host, string pattern, Action<CloudEvent>? configure = null)
    {
        return UseHttpEntrypoint<TModel>(host, pattern, new[] {HttpMethods.Delete}, configure);
    }
}