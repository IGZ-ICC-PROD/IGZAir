using System.Text.Json;
using CustomerRequestServer.Domain.Models;

namespace CustomerRequestServer.Infrastructure;

public class FlightSeedDataProvider : IFlightSeedDataProvider
{
    public async Task<List<Flight>?> GetSeedDataAsync()
    {
        var fileStream = File.OpenRead("FlightSeedData.json");
        return await JsonSerializer.DeserializeAsync<List<Flight>>(fileStream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public List<Flight>? GetSeedData()
    {
        var fileStream = File.OpenRead("FlightSeedData.json");
        return JsonSerializer.Deserialize<List<Flight>>(fileStream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}