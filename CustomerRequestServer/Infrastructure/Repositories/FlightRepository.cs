using CustomerRequestServer.Domain.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CustomerRequestServer.Infrastructure.Repositories;

public class FlightRepository : IFlightRepository
{
        private readonly IMongoCollection<Flight> _flightCollection;
        private readonly IFlightSeedDataProvider _seedDataProvider;
        private readonly ILogger<FlightRepository> _logger;
    
        public FlightRepository(IOptions<AirlineDatabaseSettings> airlineDatabaseSettings, IFlightSeedDataProvider seedDataProvider, ILogger<FlightRepository> logger)
        {
            _seedDataProvider = seedDataProvider;
            var mongoClient = new MongoClient(airlineDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(airlineDatabaseSettings.Value.DatabaseName);
            _flightCollection = mongoDatabase.GetCollection<Flight>(airlineDatabaseSettings.Value.FlightCollectionName);
            _logger = logger;
    
            SeedData();
        }
    
        private void SeedData()
        {
            if (_flightCollection.EstimatedDocumentCount() == 0)
            {
                var testFlights = _seedDataProvider.GetSeedData();
                _flightCollection.InsertMany(testFlights);
            }
        }
    
        public async Task<List<Flight>> GetAsync()
        {
            var result = await _flightCollection.FindAsync(_ => true);
            return await result.ToListAsync();
        }
    
        public async Task<Flight> GetByFlightNumberAsync(string flightNumber)
        {
            var filter = Builders<Flight>.Filter.Eq(f => f.FlightNumber, flightNumber);
            return await _flightCollection.Find(filter).FirstOrDefaultAsync();
        }
    
        public async Task<BsonDocument> ExecuteMongoQueryAsync(string mongoQuery)
        {
            try
            {
                var updateCommand = BsonDocument.Parse(mongoQuery);
    
                BsonDocument? result = await _flightCollection.Database.RunCommandAsync<BsonDocument>(updateCommand);
                _logger.LogInformation("The result of the MongoDB query is: " + result);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred while executing the MongoDB query: " + e.Message);
                throw;
            }
        }
}