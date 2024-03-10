using System.Text.Json;
using CustomerRequestServer.Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;

namespace CustomerRequestServer.Hubs;

public class DevConsoleHub : Hub<IDevConsoleClient>
{
    private readonly IReservationRepository _reservationRepository;
    
    public DevConsoleHub(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }
    
    public async Task PushConsoleMessage(string message)
    {
        await Clients.All.PushConsoleMessage(message);
    }

    public async Task ExecuteConsoleCommand(string command)
    {
        try
        {
           BsonDocument result = await _reservationRepository.ExecuteMongoQueryAsync(command);
           
           var dotNetObject = BsonTypeMapper.MapToDotNetValue(result);

           var options = new JsonSerializerOptions { WriteIndented = true };
           
           string json = JsonSerializer.Serialize(dotNetObject, options);
           string indentedJson = json.Replace("\n", "\n    ");
           
           string markdown = $"```\n    {indentedJson}\n```";
           await PushConsoleMessage($"Command executed successfully. Result: {markdown}");
        }
        catch (Exception e)
        {
            await PushConsoleMessage("An error occurred while executing the command: " + e.Message);
        }
    }

    
}