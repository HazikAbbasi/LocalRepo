using QueueProcessor.Api.Models;

namespace QueueProcessor.Api.Services;

public interface IDataProcessor
{
    Task ProcessAsync(DataItem item, CancellationToken cancellationToken);
}
