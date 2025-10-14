using QueueProcessor.Api.Models;

namespace QueueProcessor.Api.Queue;

public interface IDataQueue
{
    ValueTask EnqueueAsync(DataItem item, CancellationToken cancellationToken);
    ValueTask<DataItem> DequeueAsync(CancellationToken cancellationToken);
}
