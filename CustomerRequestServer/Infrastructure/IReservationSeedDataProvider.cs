using CustomerRequestServer.Domain.Models;

namespace CustomerRequestServer.Infrastructure;

public interface IReservationSeedDataProvider
{
    Task<List<Reservation>?> GetSeedDataAsync();
    List<Reservation>? GetSeedData();
}