using System.Text.Json.Serialization;
using CustomerRequestServer.Infrastructure.AI;

namespace CustomerRequestServer.Domain.Models;

public class ChatRequest
{
    public string Message { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AgentType AgentType { get; set; }
}