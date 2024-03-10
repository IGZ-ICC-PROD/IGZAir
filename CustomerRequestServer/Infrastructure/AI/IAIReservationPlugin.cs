namespace CustomerRequestServer.Infrastructure.AI;

public interface IAIReservationPlugin
{
    Task<string> GetReservationsAsync();
    Task<string> ExecuteModificationOnMongoDbAsync(string mongoQuery);
}