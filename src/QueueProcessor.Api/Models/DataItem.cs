using System.Text.Json;

namespace QueueProcessor.Api.Models;

public sealed record DataItem(
    Guid Id,
    string Source,
    JsonElement Data,
    DateTimeOffset EnqueuedAt
);
