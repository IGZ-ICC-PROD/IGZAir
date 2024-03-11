using System.Runtime.Serialization;

namespace CustomerRequestServer.Infrastructure.AI;

public enum AgentType
{
    [AgentSystemMessageFilePath("Infrastructure/AI/AgentSystemMessages/CustomerAgent.txt")]
    [AgentGreetingFilePath("Infrastructure/AI/AgentGreetings/CustomerAgent.txt")]
    CustomerSupport,
    [AgentGreetingFilePath("Infrastructure/AI/AgentGreetings/TechnicalAgent.txt")]
    [AgentSystemMessageFilePath("Infrastructure/AI/AgentSystemMessages/TechnicalAgent.txt")]
    TechnicalSupport,
}