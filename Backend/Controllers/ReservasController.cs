using Backend.DataContext;
using Microsoft.AspNetCore.Mvc;
using Service.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly ParkARContext _context;
        public ReservasController(ParkARContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetReservas()
        {
            var reservas = _context.Reservas.Where(r => !r.IsDeleted).ToList();
            return Ok(reservas);
        }

        [HttpGet("{id}")]
        public IActionResult GetReserva(int id)
        {
            var reserva = _context.Reservas.FirstOrDefault(r => r.Id == id && !r.IsDeleted);
            if (reserva == null) return NotFound();
            return Ok(reserva);
        }

        [HttpPost]
        public IActionResult CreateReserva([FromBody] Reserva reserva)
        {
            if (reserva == null) return BadRequest();
            _context.Reservas.Add(reserva);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetReserva), new { id = reserva.Id }, reserva);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateReserva(int id, [FromBody] Reserva reserva)
        {
            if (reserva == null || reserva.Id != id) return BadRequest();
            var existing = _context.Reservas.Find(id);
            if (existing == null) return NotFound();

            existing.UsuarioId = reserva.UsuarioId;
            existing.VehiculoId = reserva.VehiculoId;
            existing.LugarId = reserva.LugarId;
            existing.FechaInicio = reserva.FechaInicio;
            existing.FechaFin = reserva.FechaFin;
            existing.EstadoReserva = reserva.EstadoReserva;

            _context.Reservas.Update(existing);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReserva(int id)
        {
            var reserva = _context.Reservas.Find(id);
            if (reserva == null) return NotFound();
            reserva.IsDeleted = true;
            _context.SaveChanges();
            return NoContent();
        }
    }
}
