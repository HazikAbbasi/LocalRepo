using Microsoft.Extensions.Options;
using QueueProcessor.Api.Models;
using QueueProcessor.Api.Options;
using QueueProcessor.Api.Queue;
using QueueProcessor.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure queue options from configuration
builder.Services.Configure<QueueOptions>(builder.Configuration.GetSection("Queue"));

// Register queue and worker services
builder.Services.AddSingleton<IDataQueue, ChannelDataQueue>();
builder.Services.AddSingleton<IDataProcessor, DataProcessor>();
builder.Services.AddHostedService<QueueWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/enqueue", async (EnqueueRequest request, IDataQueue queue, CancellationToken ct) =>
{
    var item = new DataItem(
        Id: Guid.NewGuid(),
        Source: request.Source,
        Data: request.Data,
        EnqueuedAt: DateTimeOffset.UtcNow
    );
    await queue.EnqueueAsync(item, ct);
    return Results.Accepted($"/queue/{item.Id}");
})
.WithName("EnqueueData")
.WithOpenApi();

app.Run();
