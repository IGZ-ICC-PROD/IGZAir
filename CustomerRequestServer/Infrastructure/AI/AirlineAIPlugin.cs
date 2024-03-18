using System.ComponentModel;
using System.Text.Json;
using CustomerRequestServer.Domain.Models;
using CustomerRequestServer.Hubs;
using CustomerRequestServer.Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;
using MongoDB.Bson;

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
    [Description(
        "Explain the structure of the MongoDB database, including the collections and their fields, along with sample JSON documents.")]
    public async Task<string> ExplainDatabaseStructureAsync()
    {
        _logger.Log(LogLevel.Information,
            "Skye called her AI Plugin, requesting an explanation of the database structure.");
        await _hubContext.Clients.All.PushConsoleMessage(
            "Skye called her AI Plugin, requesting an explanation of the database structure.");

        string explanation = @"
The MongoDB database consists of two collections: 'Flights' and 'Reservations'.

The 'Flights' collection contains documents with the following fields:
- _id (string): The unique identifier of the flight.
- DepartureAirport (object): Contains information about the departure airport, including the airport code, name, and city.
- ArrivalAirport (object): Contains information about the arrival airport, including the airport code, name, and city.
- DepartureTime (date): The scheduled departure date and time of the flight.
- ArrivalTime (date): The scheduled arrival date and time of the flight.
- Aircraft (object): Contains information about the aircraft, including the model and seating capacity for each class.
- SeatsAvailable (object): Contains the number of available seats for each class on the flight.

Sample JSON document for the 'Flights' collection:
{
    '_id': 'FL123',
    'DepartureAirport': {
        'Code': 'JFK',
        'Name': 'John F. Kennedy International Airport',
        'City': 'New York'
    },
    'ArrivalAirport': {
        'Code': 'LHR',
        'Name': 'London Heathrow Airport',
        'City': 'London'
    },
    'DepartureTime': '2024-03-20T09:00:00Z',
    'ArrivalTime': '2024-03-20T12:30:00Z',
    'Aircraft': {
        'Model': 'Boeing 747',
        'Capacity': {
            'Economy': 100,
            'Business': 20,
            'First': 10
        }
    },
    'SeatsAvailable': {
        'Economy': 80,
        'Business': 15,
        'First': 8
    }
}

The 'Reservations' collection contains documents with the following fields:
- _id (string): The unique identifier of the reservation.
- Customer (object): Contains information about the customer, including their name, contact details, passport number, and frequent flyer number.
- FlightNumber (string): The flight number associated with the reservation.
- DepartureDate (date): The departure date of the flight for this reservation.
- SeatClass (string): The class of the seat reserved (e.g., Economy, Business, First).
- SeatNumber (string): The assigned seat number for the reservation.
- Status (string): The current status of the reservation (e.g., Confirmed, Pending).
- CreatedAt (date): The date and time when the reservation was created.

Sample JSON document for the 'Reservations' collection:
{
    '_id': 'R001',
    'Customer': {
        'FirstName': 'John',
        'LastName': 'Doe',
        'Email': 'john.doe@example.com',
        'Phone': '+1-123-456-7890',
        'PassportNumber': 'ABC123456',
        'FrequentFlyerNumber': 'FF1234'
    },
    'FlightNumber': 'FL123',
    'DepartureDate': '2024-03-20',
    'SeatClass': 'Economy',
    'SeatNumber': '23A',
    'Status': 'Confirmed',
    'CreatedAt': '2024-02-15T10:30:00Z'
}

These collections are related through the FlightNumber field in the Reservations collection, which corresponds to the _id field in the Flights collection.
";

        return explanation;
    }



    [KernelFunction]
    [Description("Execute a modification on the MongoDB database.")]
    public async Task<string> ExecuteModificationOnMongoDbAsync(string mongoQuery)
    {
        try
        {
            _logger.Log(LogLevel.Information, "Skye called her AI Plugin, executing MongoDB query: {Query}", mongoQuery);
            await _hubContext.Clients.All.PushConsoleMessage("Skye called her AI Plugin, executing MongoDB query: " + mongoQuery);
           BsonDocument result = await _reservationRepository.ExecuteMongoQueryAsync(mongoQuery);
           return result.ToJson();
        }
        catch (Exception e)
        {
            return e.Message;
        }
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
