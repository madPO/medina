namespace Medina.Core;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal sealed class DistributedContext : IDistributedContext
{
    public Task PublishEventAsync(object model, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<TResult> ExecuteAsync<TResult>(object arg, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public IQueryable<TModel> Query<TModel>()
    {
        throw new System.NotImplementedException();
    }
}