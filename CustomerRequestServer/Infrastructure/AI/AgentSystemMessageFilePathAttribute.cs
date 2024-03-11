namespace CustomerRequestServer.Infrastructure.AI;

public class AgentSystemMessageFilePathAttribute : Attribute
{
    public AgentSystemMessageFilePathAttribute(string path)
    {
        Path = path;
    }

    public string Path { get;  }
}