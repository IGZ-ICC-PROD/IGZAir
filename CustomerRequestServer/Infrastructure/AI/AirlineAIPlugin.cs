using System.ComponentModel;
using System.Text.Json;
using CustomerRequestServer.Domain.Models;
using CustomerRequestServer.Hubs;
using CustomerRequestServer.Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;

namespace CustomerRequestServer.Infrastructure.AI;

public class AirlineAIPlugin : IAirlineAIPlugin
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IFlightRepository _flightRepository;
    private readonly ILogger<AirlineAIPlugin> _logger;
    private readonly IHubContext<DevConsoleHub, IDevConsoleClient> _hubContext;

    public AirlineAIPlugin(IReservationRepository reservationRepository, IFlightRepository flightRepository, ILogger<AirlineAIPlugin> logger, IHubContext<DevConsoleHub, IDevConsoleClient> hubContext)
    {
        _reservationRepository = reservationRepository;
        _flightRepository = flightRepository;
        _logger = logger;
        _hubContext = hubContext;
    }

    [KernelFunction]
    [Description("Execute a modification on the MongoDB database.")]
    public async Task<string> ExecuteModificationOnMongoDbAsync(string mongoQuery)
    {
        try
        {
            _logger.Log(LogLevel.Information, "Skye called her AI Plugin, executing MongoDB query: {Query}", mongoQuery);
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

    [KernelFunction]
    [Description("Retrieve all flights from the MongoDB database. This function returns a JSON string with all the flights.")]
    public async Task<string> GetFlightsAsync()
    {
        _logger.Log(LogLevel.Information, "Skye called her AI Plugin, retrieving all flights from the MongoDB database.");
        await _hubContext.Clients.All.PushConsoleMessage("Skye called her AI Plugin, retrieving all flights from the MongoDB database.");
        List<Flight> flights = await _flightRepository.GetAsync();
        return JsonSerializer.Serialize(flights);
    }

    [KernelFunction]
    [Description("Retrieve a specific flight from the MongoDB database by its flight number. This function returns a JSON string with the flight details.")]
    public async Task<string> GetFlightByFlightNumberAsync(string flightNumber)
    {
        _logger.Log(LogLevel.Information, "Skye called her AI Plugin, retrieving flight with flight number: {FlightNumber}", flightNumber);
        await _hubContext.Clients.All.PushConsoleMessage("Skye called her AI Plugin, retrieving flight with flight number: " + flightNumber);
        Flight flight = await _flightRepository.GetByFlightNumberAsync(flightNumber);
        return JsonSerializer.Serialize(flight);
    }
}
