using System.ComponentModel;
using System.Text.Json;
using CustomerRequestServer.Domain.Models;
using CustomerRequestServer.Hubs;
using CustomerRequestServer.Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;

namespace CustomerRequestServer.Infrastructure.AI;

public class AIReservationPlugin : IAIReservationPlugin
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ILogger<AIReservationPlugin> _logger;
    private readonly IHubContext<DevConsoleHub,IDevConsoleClient> _hubContext;
    
    public AIReservationPlugin(IReservationRepository reservationRepository, ILogger<AIReservationPlugin> logger, IHubContext<DevConsoleHub, IDevConsoleClient> hubContext)
    {
        _reservationRepository = reservationRepository;
        _logger = logger;
        _hubContext = hubContext;
    }
    
    [KernelFunction]
    [Description("Execute a modification on the MongoDB database.")]
    public async Task<string> ExecuteModificationOnMongoDbAsync(string mongoQuery)
    {
        try
        {
            _logger.Log(LogLevel.Information, "Skye called her AI Plugin, executing MongoDB query: {Query}",mongoQuery);
            await _hubContext.Clients.All.PushConsoleMessage("Skye called her AI Plugin, executing MongoDB query: " + mongoQuery);
            await _reservationRepository.ExecuteMongoQueryAsync(mongoQuery);
        }
        catch (Exception e)
        {
            return e.Message;
        }

        return "Operation executed successfully.";
    }
    
    [KernelFunction]
    [Description("Retrieve all reservations from the MongoDB database. This function returns a JSON string with all the reservations.")]
    public async Task<string> GetReservationsAsync()
    {
        _logger.Log(LogLevel.Information, "Skye called her AI Plugin, retrieving all reservations from the MongoDB database.");
        await _hubContext.Clients.All.PushConsoleMessage("Skye called her AI Plugin, retrieving all reservations from the MongoDB database.");
        List<Reservation> reservations = await _reservationRepository.GetAsync();
        return JsonSerializer.Serialize(reservations);
    }
    
}