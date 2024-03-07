using CustomerRequestServer.Domain.Infrastructure.Repositories;
using CustomerRequestServer.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerRequestServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ILogger<ReservationRepository> _logger;

    public ReservationController(IReservationRepository reservationRepository, ILogger<ReservationRepository> logger)
    {
        _reservationRepository = reservationRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Reservation>>> GetReservations()
    {
        var reservations = await _reservationRepository.GetAsync();
        return Ok(reservations);
    }
}