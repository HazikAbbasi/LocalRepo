using System.Threading.Channels;

namespace QueueProcessor.Api.Options;

public sealed class QueueOptions
{
    public int Capacity { get; set; } = 1000;
    public bool SingleReader { get; set; } = true;
    public bool SingleWriter { get; set; } = false;
    public BoundedChannelFullMode FullMode { get; set; } = BoundedChannelFullMode.Wait;
}
