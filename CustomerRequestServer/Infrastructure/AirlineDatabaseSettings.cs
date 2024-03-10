namespace CustomerRequestServer.Infrastructure;

public class AirlineDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string ReservationCollectionName { get; set; } = null!;
}