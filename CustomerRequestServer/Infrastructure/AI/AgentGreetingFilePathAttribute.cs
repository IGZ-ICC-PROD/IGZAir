namespace CustomerRequestServer.Infrastructure.AI;

public class AgentGreetingFilePathAttribute : Attribute
{
    public AgentGreetingFilePathAttribute(string path)
    {
        Path = path;
    }

    public string Path { get;  }
}