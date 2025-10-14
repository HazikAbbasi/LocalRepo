using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QueueProcessor.Api.Models;
using QueueProcessor.Api.Queue;

namespace QueueProcessor.Api.Services;

public sealed class QueueWorker : BackgroundService
{
    private readonly ILogger<QueueWorker> _logger;
    private readonly IDataQueue _queue;
    private readonly IDataProcessor _processor;

    public QueueWorker(ILogger<QueueWorker> logger, IDataQueue queue, IDataProcessor processor)
    {
        _logger = logger;
        _queue = queue;
        _processor = processor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queue worker started");
        while (!stoppingToken.IsCancellationRequested)
        {
            DataItem item;
            try
            {
                item = await _queue.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            try
            {
                await ProcessWithRetryAsync(item, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process item {Id} from {Source}", item.Id, item.Source);
            }
        }
        _logger.LogInformation("Queue worker stopping");
    }

    private async Task ProcessWithRetryAsync(DataItem item, CancellationToken ct)
    {
        const int maxAttempts = 3;
        var delayMs = 250;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await _processor.ProcessAsync(item, ct);
                _logger.LogInformation("Processed item {Id} (attempt {Attempt})", item.Id, attempt);
                return;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                _logger.LogWarning(ex, "Error processing item {Id}. Retrying (attempt {Attempt}/{Max})", item.Id, attempt, maxAttempts);
                await Task.Delay(delayMs, ct);
                delayMs = Math.Min(delayMs * 2, 4000);
            }
        }

        await _processor.ProcessAsync(item, ct);
        _logger.LogInformation("Processed item {Id} on final attempt", item.Id);
    }
}
