using CustomerRequestServer.Domain.Models;
using MongoDB.Bson;

namespace CustomerRequestServer.Infrastructure.Repositories;

public interface IFlightRepository
{
    Task<List<Flight>> GetAsync();
    Task<Flight> GetByFlightNumberAsync(string flightNumber);
   public Task<BsonDocument> ExecuteMongoQueryAsync(string mongoQuery);
}