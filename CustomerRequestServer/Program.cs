using CustomerRequestServer.Domain.Infrastructure;
using CustomerRequestServer.Domain.Infrastructure.AI;
using CustomerRequestServer.Domain.Infrastructure.Repositories;
using Microsoft.SemanticKernel;
using Serilog;
using Serilog.Core;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

Logger logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Services.Configure<AirlineDatabaseSettings>(builder.Configuration.GetSection("AirlineDatabase"));
builder.Services.AddSingleton<IReservationSeedDataProvider, ReservationSeedDataProvider>();
builder.Services.AddSingleton<IReservationRepository, ReservationRepository>();
builder.Services.AddSingleton<IAIReservationPlugin, AIReservationPlugin>();
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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();

app.Run();