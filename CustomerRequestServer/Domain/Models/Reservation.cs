using MongoDB.Bson.Serialization.Attributes;

namespace CustomerRequestServer.Domain.Models;

public class Reservation
{
    [BsonId]
    public string ReservationId { get; set; }
    public string CustomerName { get; set; }
    public string FlightNumber { get; set; }
    public DateTime DepartureDate { get; set; }
    public string From { get; set; }
    public string To { get; set; }
}