using CustomerRequestServer.Domain.Models;

namespace CustomerRequestServer.Domain.Infrastructure.Repositories;

public interface IReservationRepository
{
    Task<List<Reservation>> GetAsync();
    Task ExecuteMongoQueryAsync(string mongoQuery);

}