namespace CustomerRequestServer.Hubs;

public interface IDevConsoleClient
{
    public Task PushConsoleMessage(string message);
}