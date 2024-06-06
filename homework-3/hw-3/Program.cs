using hw_3;
using hw_3.Config;
using hw_3.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

builder.Services.Configure<ThreadsConfig>(builder.Configuration.GetSection(nameof(ThreadsConfig)));
builder.Services.Configure<FilesConfig>(builder.Configuration.GetSection(nameof(FilesConfig)));

builder.Services.AddScoped<IDemandService, DemandService>();
builder.Services.AddScoped<FileProcessor>();

var provider = builder.Services.BuildServiceProvider();
await using var scope = provider.CreateAsyncScope();

var processor = scope.ServiceProvider.GetRequiredService<FileProcessor>();

var tokenSource = new CancellationTokenSource();
var token = tokenSource.Token;

Console.CancelKeyPress += (sender, e) =>
{
    Console.WriteLine("Cancelled");
    tokenSource.Cancel();
    e.Cancel = true;
};

await processor.Run(token);