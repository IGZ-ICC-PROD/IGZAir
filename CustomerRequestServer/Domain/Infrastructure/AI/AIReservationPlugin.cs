using System.ComponentModel;
using System.Text.Json;
using CustomerRequestServer.Domain.Infrastructure.Repositories;
using CustomerRequestServer.Domain.Models;
using Microsoft.SemanticKernel;

namespace CustomerRequestServer.Domain.Infrastructure.AI;

public class AIReservationPlugin : IAIReservationPlugin
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ILogger<AIReservationPlugin> _logger;
    
    public AIReservationPlugin(IReservationRepository reservationRepository, ILogger<AIReservationPlugin> logger)
    {
        _reservationRepository = reservationRepository;
        _logger = logger;
    }
    
    [KernelFunction]
    [Description("Execute a modification on the MongoDB database.")]
    public async Task<string> ExecuteModificationOnMongoDbAsync(string mongoQuery)
    {
        try
        {
            _logger.Log(LogLevel.Information, "Called by AI, executing MongoDB query: " + mongoQuery);
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
        List<Reservation> reservations = await _reservationRepository.GetAsync();
        return JsonSerializer.Serialize(reservations);
    }
    
}