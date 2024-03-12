using CustomerRequestServer.Domain.Models;

namespace CustomerRequestServer.Infrastructure;

public interface IFlightSeedDataProvider
{
    Task<List<Flight>?> GetSeedDataAsync();
    List<Flight>? GetSeedData();
    
}