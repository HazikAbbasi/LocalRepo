using System.Text.Json;
using Microsoft.Extensions.Logging;
using QueueProcessor.Api.Models;

namespace QueueProcessor.Api.Services;

public sealed class DataProcessor : IDataProcessor
{
    private readonly ILogger<DataProcessor> _logger;

    public DataProcessor(ILogger<DataProcessor> logger)
    {
        _logger = logger;
    }

    public Task ProcessAsync(DataItem item, CancellationToken cancellationToken)
    {
        var payload = item.Data.ValueKind == JsonValueKind.Undefined ? "<undefined>" : item.Data.ToString();
        _logger.LogInformation("Processing item {Id} from {Source}. Payload: {Payload}", item.Id, item.Source, payload);
        // Replace this stub with real processing logic
        return Task.CompletedTask;
    }
}
