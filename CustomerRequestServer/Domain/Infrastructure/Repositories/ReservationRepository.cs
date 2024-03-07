using CustomerRequestServer.Domain.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CustomerRequestServer.Domain.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly IMongoCollection<Reservation> _reservationCollection;
    private readonly IReservationSeedDataProvider _seedDataProvider;
    private readonly ILogger<ReservationRepository> _logger;

    public ReservationRepository(IOptions<AirlineDatabaseSettings> airlineDatabaseSettings,IReservationSeedDataProvider seedDataProvider, ILogger<ReservationRepository> logger)
    {
        _seedDataProvider = seedDataProvider;
        var mongoClient = new MongoClient(airlineDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(airlineDatabaseSettings.Value.DatabaseName);
        _reservationCollection = mongoDatabase.GetCollection<Reservation>(airlineDatabaseSettings.Value.ReservationCollectionName);
        _logger = logger;
        
        SeedData();
    }
    
    private void SeedData()
    {
        if (_reservationCollection.EstimatedDocumentCount() == 0)
        {
            var testReservations = _seedDataProvider.GetSeedData();
            _reservationCollection.InsertMany(testReservations);
        }
    }
    
    public async Task<List<Reservation>> GetAsync()
    {
       var result =  await _reservationCollection.FindAsync(_ => true);
       return await result.ToListAsync();
    }
    
    public async Task ExecuteMongoQueryAsync(string mongoQuery)
    {
        try
        {
            var updateCommand= BsonDocument.Parse(mongoQuery);
            
            _logger.LogInformation("executing update command after merge: " + updateCommand);
            var result = await _reservationCollection.Database.RunCommandAsync<BsonDocument>(updateCommand);
            
            _logger.LogInformation("The result of the MongoDB query is: " + result);
        }
        catch (Exception e)
        {
            _logger.LogError("An error occurred while executing the MongoDB query: " + e.Message);
            throw;
        }

    }
}