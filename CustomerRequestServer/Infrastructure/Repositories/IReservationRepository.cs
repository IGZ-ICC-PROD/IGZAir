using CustomerRequestServer.Domain.Models;
using MongoDB.Bson;

namespace CustomerRequestServer.Infrastructure.Repositories;

public interface IReservationRepository
{
    Task<List<Reservation>> GetAsync();
    Task<BsonDocument> ExecuteMongoQueryAsync(string mongoQuery);

}