namespace CustomerRequestServer.Infrastructure.AI;

public interface IAirlineAIPlugin
{
    Task<string> ExplainDatabaseStructureAsync();
    Task<string> GetReservationsAsync();
    Task<string> ExecuteModificationOnMongoDbAsync(string mongoQuery);
}