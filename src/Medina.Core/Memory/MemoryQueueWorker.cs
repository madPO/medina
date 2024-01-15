using Medina.Core.Descriptors;
using Medina.Core.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Medina.Core.Memory;

public class MemoryQueueWorker: BackgroundService
{
    private readonly MemoryEventStore _store;

    private readonly ILogger<MemoryQueueWorker> _logger;

    private readonly SemaphoreSlim _semaphore;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _store.WaitNewEvent(stoppingToken);
            
            var (cloudEvent, descriptor, hasItem) = _store.Dequeue();

            if (!hasItem || descriptor is not OperationDescriptor operation)
            {
                continue;
            }

            try
            {
                await _semaphore.WaitAsync(stoppingToken);

                _ = Task.Run(async () =>
                {
                    await Task.Yield();
                    await operation.Handler.Invoke(cloudEvent, stoppingToken);
                }, stoppingToken)
                .ContinueWith((task) =>
                {
                    _semaphore.Release();

                    if (task.Exception is not null)
                    {
                        _logger.LogError(task.Exception, "Ошибка обработки события");            
                    }

                    return task;
                }, stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Ошибка обработки события");            
            }
        }                                                   
    }

    public MemoryQueueWorker(MemoryEventStore store, IOptions<MedinaOptions> options, ILogger<MemoryQueueWorker> logger)
    {
        _store = store;
        _logger = logger;

        var max = options.Value.ParallelWorkers;
        _semaphore = new SemaphoreSlim(max, max);
    }
}