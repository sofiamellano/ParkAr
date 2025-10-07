using Backend.DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Enums;
using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LugaresController : ControllerBase
    {
        private readonly ParkARContext _context;

        public LugaresController(ParkARContext context)
        {
            _context = context;
        }

        // GET: api/Lugares
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lugar>>> GetLugares([FromQuery] int? filtro = null)
        {
            var query = _context.Lugares.AsNoTracking().Include(l => l.Reservas).AsQueryable();
            if (filtro.HasValue)
            {
                query = query.Where(l => l.Numero == filtro.Value);
            }
            return await query.ToListAsync();
        }
        [HttpGet("disponibles")]
        public async Task<ActionResult<List<Lugar>>> GetDisponibles(
    [FromQuery] DateTime fechaInicio,
    [FromQuery] DateTime fechaFin)
        {
            try
            {
                // Validar fechas
                if (fechaInicio >= fechaFin)
                {
                    return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin");
                }

                if (fechaInicio < DateTime.Now)
                {
                    return BadRequest("La fecha de inicio debe ser futura");
                }

                // Obtener lugares que NO tienen reservas activas en el rango de fechas
                var lugaresOcupados = await _context.Reservas
                    .Where(r => !r.IsDeleted &&
                               r.EstadoReserva == EstadoReservaEnum.Activa &&
                               ((r.FechaInicio <= fechaInicio && r.FechaFin > fechaInicio) ||
                                (r.FechaInicio < fechaFin && r.FechaFin >= fechaFin) ||
                                (r.FechaInicio >= fechaInicio && r.FechaFin <= fechaFin)))
                    .Select(r => r.LugarId)
                    .Distinct()
                    .ToListAsync();

                var lugaresDisponibles = await _context.Lugares
                    .Where(l => !l.IsDeleted && !lugaresOcupados.Contains(l.Id))
                    .ToListAsync();

                return Ok(lugaresDisponibles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("deleteds")]
        public async Task<ActionResult<IEnumerable<Lugar>>> GetDeletedLugares([FromQuery] int? filtro = null)
        {
            var query = _context.Lugares.AsNoTracking().IgnoreQueryFilters().Where(l => l.IsDeleted).Include(l => l.Reservas).AsQueryable();
            if (filtro.HasValue)
            {
                query = query.Where(l => l.Numero == filtro.Value);
            }
            return await query.ToListAsync();
        }

        // GET: api/Lugares/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lugar>> GetLugar(int id)
        {
            var lugar = await _context.Lugares.AsNoTracking().Include(l => l.Reservas).FirstOrDefaultAsync(a => a.Id.Equals(id));

            if (lugar == null)
            {
                return NotFound();
            }

            return lugar;
        }

        // PUT: api/Lugares/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLugar(int id, Lugar lugar)
        {
            if (id != lugar.Id)
            {
                return BadRequest();
            }

            _context.Entry(lugar).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LugarExists(id))
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

        // POST: api/Lugares
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Lugar>> PostLugar(Lugar lugar)
        {
            _context.Lugares.Add(lugar);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLugar", new { id = lugar.Id }, lugar);
        }

        // DELETE: api/Lugares/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLugar(int id)
        {
            var lugar = await _context.Lugares.FindAsync(id);
            if (lugar == null)
            {
                return NotFound();
            }

            lugar.IsDeleted = true;
            _context.Lugares.Update(lugar);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreLugar(int id)
        {
            var lugar = await _context.Lugares.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Id.Equals(id));
            if (lugar == null)
            {
                return NotFound();
            }
            lugar.IsDeleted = false;
            _context.Lugares.Update(lugar);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        private bool LugarExists(int id)
        {
            return _context.Lugares.Any(e => e.Id == id);
        }
    }
}
