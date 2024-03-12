using MongoDB.Bson.Serialization.Attributes;

namespace CustomerRequestServer.Domain.Models;

public class Reservation
{
    [BsonId]
    public string ReservationId { get; set; }
    public Customer Customer { get; set; }
    public string FlightNumber { get; set; }
    public DateTime DepartureDate { get; set; }
    public string SeatClass { get; set; }
    public string SeatNumber { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
