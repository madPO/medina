namespace Medina.Core.Transport;

using System.Threading;
using System.Threading.Tasks;

using Wrapper;

/// <summary>
/// Транспорт распределенных события
/// </summary>
public interface IDistributedEventTransport
{
    /// <summary>
    /// Опубликовать 
    /// </summary>
    /// <param name="wrapper">Обертка события</param>
    /// <param name="cancellationToken">Токен для отмены</param>
    Task DeliverEventAsync(EventWrapper wrapper, CancellationToken cancellationToken = default);
}