//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;

//var builder = FunctionsApplication.CreateBuilder(args);

//builder.ConfigureFunctionsWebApplication();

//builder.Services
//    .AddApplicationInsightsTelemetryWorkerService()
//    .ConfigureFunctionsApplicationInsights();
using Microsoft.Extensions.Hosting;

//builder.Build().Run();


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => { })
    .Build();

host.Run();
