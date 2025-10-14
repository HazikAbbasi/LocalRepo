using System.Text.Json;

namespace QueueProcessor.Api.Models;

public sealed record EnqueueRequest(
    string Source,
    JsonElement Data
);
