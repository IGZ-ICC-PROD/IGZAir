using System.ComponentModel;

namespace CustomerRequestServer.Infrastructure.AI;

public interface IAgentProvider
{
    Task<string> GetSystemMessageAsync(AgentType agentType);

    Task<string> GetGreetingMessageAsync(AgentType agentType);
}

