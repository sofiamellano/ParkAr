using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.DataContext;
using Service.Models;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservasController : ControllerBase
    {
        private readonly ParkARContext _context;

        public ReservasController(ParkARContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas([FromQuery] int? usuarioId = null)
        {
            var query = _context.Reservas.AsNoTracking().Include(r => r.Usuario).Include(r => r.Vehiculo).Include(r => r.Lugar).AsQueryable();
            if (usuarioId.HasValue)
            {
                query = query.Where(r => r.UsuarioId == usuarioId.Value);
            }
            return await query.ToListAsync();
        }

        [HttpGet("deleteds")]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetDeletedReservas([FromQuery] int? usuarioId = null)
        {
            var query = _context.Reservas.AsNoTracking().IgnoreQueryFilters().Where(r => r.IsDeleted).Include(r => r.Usuario).Include(r => r.Vehiculo).Include(r => r.Lugar).AsQueryable();
            if (usuarioId.HasValue)
            {
                query = query.Where(r => r.UsuarioId == usuarioId.Value);
            }
            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reserva>> GetReserva(int id)
        {
            var reserva = await _context.Reservas.AsNoTracking().Include(r => r.Usuario).Include(r => r.Vehiculo).Include(r => r.Lugar).FirstOrDefaultAsync(r => r.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }
            return reserva;
        }

        [HttpGet("byusuario")]
        public async Task<ActionResult<List<Reserva>?>> GetByUsuario([FromQuery] int idusuario = 0)
        {
            if (idusuario == 0)
            {
                return BadRequest("El parametro idusuario es obligatorio.");
            }

            var reservas = await _context.Reservas
                .Include(p => p.Lugar)
                .Include(p => p.Vehiculo)
                .AsNoTracking()
                .Where(p => p.UsuarioId.Equals(idusuario))
                .ToListAsync();

            return reservas;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReserva(int id, Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return BadRequest();
            }
            _context.Entry(reserva).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Reserva>> PostReserva(Reserva reserva)
        {
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetReserva", new { id = reserva.Id }, reserva);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            reserva.IsDeleted = true;
            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreReserva(int id)
        {
            var reserva = await _context.Reservas.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }
            reserva.IsDeleted = false;
            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }
    }
}
