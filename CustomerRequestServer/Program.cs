using CustomerRequestServer.Domain.Infrastructure;
using CustomerRequestServer.Domain.Infrastructure.AI;
using CustomerRequestServer.Domain.Infrastructure.Repositories;
using CustomerRequestServer.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;
using Serilog;
using Serilog.Sinks.SignalR.Core.Extensions;
using Serilog.Sinks.SignalR.Core.Interfaces;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.Configure<AirlineDatabaseSettings>(builder.Configuration.GetSection("AirlineDatabase"));
builder.Services.AddSingleton<IReservationSeedDataProvider, ReservationSeedDataProvider>();
builder.Services.AddSingleton<IReservationRepository, ReservationRepository>();
builder.Services.AddSingleton<IAIReservationPlugin, AIReservationPlugin>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IKernelBuilder>(serviceProvider =>
{
    string openAISecret = builder.Configuration["OpenAI:Secret"];
    string openAIModel = builder.Configuration["OpenAI:Model"];
    IKernelBuilder kernelBuilder = Kernel.CreateBuilder().AddOpenAIChatCompletion(openAIModel, openAISecret);
    var reservationPlugin = serviceProvider.GetRequiredService<IAIReservationPlugin>();
    kernelBuilder.Plugins.AddFromObject(reservationPlugin);
    return kernelBuilder;
});

builder.Services.AddControllers();

var app = builder.Build();

IHubContext<DevConsoleHub, ISerilogHub> hubContext = app.Services.GetRequiredService<IHubContext<DevConsoleHub, ISerilogHub>>();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.SignalR(hubContext)
    .CreateLogger();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.MapHub<DevConsoleHub>("/devConsoleHub");
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();

app.Run();