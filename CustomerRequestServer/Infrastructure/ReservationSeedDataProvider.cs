using System.Text.Json;
using CustomerRequestServer.Domain.Models;

namespace CustomerRequestServer.Infrastructure;

public class ReservationSeedDataProvider : IReservationSeedDataProvider
{
    public async Task<List<Reservation>?> GetSeedDataAsync()
    {
        var fileStream = File.OpenRead("ReservationSeedData.json");
        return await JsonSerializer.DeserializeAsync<List<Reservation>>(fileStream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public List<Reservation>? GetSeedData()
    {
        var fileStream = File.OpenRead("ReservationSeedData.json");
        return JsonSerializer.Deserialize<List<Reservation>>(fileStream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}