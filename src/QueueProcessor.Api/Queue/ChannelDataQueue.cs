using System.Threading.Channels;
using Microsoft.Extensions.Options;
using QueueProcessor.Api.Models;
using QueueProcessor.Api.Options;

namespace QueueProcessor.Api.Queue;

public sealed class ChannelDataQueue : IDataQueue
{
    private readonly Channel<DataItem> _channel;

    public ChannelDataQueue(IOptions<QueueOptions> options)
    {
        var opts = options.Value ?? new QueueOptions();
        var boundedOptions = new BoundedChannelOptions(opts.Capacity)
        {
            FullMode = opts.FullMode,
            SingleReader = opts.SingleReader,
            SingleWriter = opts.SingleWriter
        };
        _channel = Channel.CreateBounded<DataItem>(boundedOptions);
    }

    public ValueTask EnqueueAsync(DataItem item, CancellationToken cancellationToken)
    {
        return _channel.Writer.WriteAsync(item, cancellationToken);
    }

    public ValueTask<DataItem> DequeueAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAsync(cancellationToken);
    }
}
