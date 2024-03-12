using CustomerRequestServer.Hubs;
using CustomerRequestServer.Infrastructure;
using CustomerRequestServer.Infrastructure.AI;
using CustomerRequestServer.Infrastructure.Repositories;
using Microsoft.SemanticKernel;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.Configure<AirlineDatabaseSettings>(builder.Configuration.GetSection("AirlineDatabase"));
builder.Services.AddSingleton<IReservationSeedDataProvider, ReservationSeedDataProvider>();
builder.Services.AddSingleton<IFlightSeedDataProvider, FlightSeedDataProvider>();
builder.Services.AddSingleton<IFlightRepository, FlightRepository>();
builder.Services.AddSingleton<IReservationRepository, ReservationRepository>();
builder.Services.AddSingleton<IAirlineAIPlugin, AirlineAIPlugin>();
builder.Services.AddSingleton<IChatHistoryRepository, ChatHistoryRepository>();
builder.Services.AddSingleton<IAgentProvider, AgentProvider>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IKernelBuilder>(serviceProvider =>
{
    string openAISecret = builder.Configuration["OpenAI:Secret"];
    string openAIModel = builder.Configuration["OpenAI:Model"];
    IKernelBuilder kernelBuilder = Kernel.CreateBuilder().AddOpenAIChatCompletion(openAIModel, openAISecret);
    var reservationPlugin = serviceProvider.GetRequiredService<IAirlineAIPlugin>();
    kernelBuilder.Plugins.AddFromObject(reservationPlugin);
    return kernelBuilder;
});

builder.Services.AddControllers();
builder.Services.AddSerilog((provider, configuration) =>
{
    configuration.ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

var app = builder.Build();

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