using System.Collections.Concurrent;

namespace CustomerRequestServer.Infrastructure.AI;

public class AgentProvider : IAgentProvider
{
    private readonly ConcurrentDictionary<AgentType,string> _systemMessageDictionary = new();
    private readonly ConcurrentDictionary<AgentType,string> _greetingMessageDictionary = new();
    
    public async Task<string> GetSystemMessageAsync(AgentType agentType)
    {
        if (_systemMessageDictionary.TryGetValue(agentType, out var systemMessage))
        {
            return systemMessage;
        }

        var name = Enum.GetName(typeof(AgentType), agentType);
        var agentFilePath = typeof(AgentType).GetField(name).GetCustomAttributes(false).OfType<AgentSystemMessageFilePathAttribute>().SingleOrDefault().Path;

        var systeMessageFromFile = await File.ReadAllTextAsync(agentFilePath);
        _systemMessageDictionary.TryAdd(agentType, systeMessageFromFile);
        return systeMessageFromFile;
    }

    public Task<string> GetGreetingMessageAsync(AgentType agentType)
    {
        if (_greetingMessageDictionary.TryGetValue(agentType, out var greetingMessage))
        {
            return Task.FromResult(greetingMessage);
        }

        var name = Enum.GetName(typeof(AgentType), agentType);
        var agentFilePath = typeof(AgentType).GetField(name).GetCustomAttributes(false).OfType<AgentGreetingFilePathAttribute>().SingleOrDefault().Path;

        var greetingMessageFromFile = File.ReadAllText(agentFilePath);
        _greetingMessageDictionary.TryAdd(agentType, greetingMessageFromFile);
        return Task.FromResult(greetingMessageFromFile);
    }
}