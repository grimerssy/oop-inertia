using ConsoleUI;
using Inertia.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateDefaultBuilder();

hostBuilder.ConfigureServices(services =>
{
    services.AddSingleton<App>();

    services.AddTransient<BestScoresStorage>();
});

var host = hostBuilder.Build();

var app = host.Services.GetService<App>() ?? throw new NullReferenceException();

app.Start();