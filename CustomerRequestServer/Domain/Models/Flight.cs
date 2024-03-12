using MongoDB.Bson.Serialization.Attributes;

namespace CustomerRequestServer.Domain.Models;

public class Flight
{
    [BsonId]
    public string FlightNumber { get; set; }
    public Airport DepartureAirport { get; set; }
    public Airport ArrivalAirport { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public Aircraft Aircraft { get; set; }
    public SeatInfo SeatsAvailable { get; set; }
}