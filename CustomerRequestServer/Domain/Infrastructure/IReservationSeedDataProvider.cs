using CustomerRequestServer.Domain.Models;

namespace CustomerRequestServer.Domain.Infrastructure;

public interface IReservationSeedDataProvider
{
    Task<List<Reservation>?> GetSeedDataAsync();
    List<Reservation>? GetSeedData();
}