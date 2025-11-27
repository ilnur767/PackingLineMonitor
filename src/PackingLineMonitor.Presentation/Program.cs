using FluentValidation;
using PackingLineMonitor.Application.Abstractions;
using PackingLineMonitor.Application.Messaging;
using PackingLineMonitor.Application.Queries;
using PackingLineMonitor.DataEmulator;
using PackingLineMonitor.Infrastructure.Database;
using PackingLineMonitor.Infrastructure.Events;
using PackingLineMonitor.Infrastructure.MessageQueues;
using PackingLineMonitor.Infrastructure.Repositories;
using PackingLineMonitor.Infrastructure.Services;
using PackingLineMonitor.Presentation.BackgroundServices;
using PackingLineMonitor.Presentation.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DataEmulationService>();
builder.Services.AddSingleton<LineEventAggregationService>();
builder.Services.AddSingleton<INormalizationService, NormalizationService>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHostedService<DataEmulationBackgroundService>();
builder.Services.AddHostedService<NormalizationBackgroundService>();
builder.Services.AddHostedService<LineEventBackgroundService>();
builder.Services.AddSingleton<IMessageQueue<LineCounterMeasurement>, InMemoryMessageQueue<LineCounterMeasurement>>();
builder.Services.AddSingleton<IMessageQueue<LineStatusChangedEvent>, InMemoryMessageQueue<LineStatusChangedEvent>>();
builder.Services.AddDbContext<PackingLineMonitorDbContext>();
builder.Services.AddScoped<ILineEventRepository, LineEventRepository>();
builder.Services.AddScoped<IGetLineStatsHandler, GetLineStatsHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<GetLineStatsQueryValidator>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.ApplyMigration();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();