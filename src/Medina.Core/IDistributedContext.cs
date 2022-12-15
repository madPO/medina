namespace Medina.Core;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Распределенный контекст
/// </summary>
public interface IDistributedContext
{
    /// <summary>
    /// Опубликовать событие
    /// </summary>
    /// <param name="model">Модель события</param>
    /// <param name="cancellationToken">Токен для отмены публикации</param>
    Task PublishEventAsync(object model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Выполнить команду
    /// </summary>
    /// <param name="arg">Аргументы команды</param>
    /// <param name="cancellationToken">Токен для отмены выполнения</param>
    /// <typeparam name="TResult">Модель результата</typeparam>
    /// <returns>Результат выполнения</returns>
    Task<TResult> ExecuteAsync<TResult>(object arg, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сформировать запрос
    /// </summary>
    /// <returns>Запрос данных</returns>
    IQueryable<TModel> Query<TModel>();
}