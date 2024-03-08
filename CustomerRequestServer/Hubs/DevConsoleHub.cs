using Microsoft.AspNetCore.SignalR;
using Serilog.Sinks.SignalR.Core.Hubs;
using Serilog.Sinks.SignalR.Core.Interfaces;

namespace CustomerRequestServer.Hubs;

public class DevConsoleHub : BaseSerilogHub
{
    public Task PushEventLog(string message)
    {
        return Clients.All.PushEventLog(message);
    }
}