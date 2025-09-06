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
    public class SuscripcionesController : ControllerBase
    {
        private readonly ParkARContext _context;

        public SuscripcionesController(ParkARContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Suscripcion>>> GetSuscripciones([FromQuery] int? usuarioId = null)
        {
            var query = _context.Suscripciones.AsNoTracking().Include(s => s.Usuario).Include(s => s.Plan).Include(s => s.Pagos).AsQueryable();
            if (usuarioId.HasValue)
            {
                query = query.Where(s => s.UsuarioId == usuarioId.Value);
            }
            return await query.ToListAsync();
        }

        [HttpGet("deleteds")]
        public async Task<ActionResult<IEnumerable<Suscripcion>>> GetDeletedSuscripciones([FromQuery] int? usuarioId = null)
        {
            var query = _context.Suscripciones.AsNoTracking().IgnoreQueryFilters().Where(s => s.IsDeleted).Include(s => s.Usuario).Include(s => s.Plan).Include(s => s.Pagos).AsQueryable();
            if (usuarioId.HasValue)
            {
                query = query.Where(s => s.UsuarioId == usuarioId.Value);
            }
            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Suscripcion>> GetSuscripcion(int id)
        {
            var suscripcion = await _context.Suscripciones.AsNoTracking().Include(s => s.Usuario).Include(s => s.Plan).Include(s => s.Pagos).FirstOrDefaultAsync(s => s.Id == id);
            if (suscripcion == null)
            {
                return NotFound();
            }
            return suscripcion;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSuscripcion(int id, Suscripcion suscripcion)
        {
            if (id != suscripcion.Id)
            {
                return BadRequest();
            }
            _context.Entry(suscripcion).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SuscripcionExists(id))
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
        public async Task<ActionResult<Suscripcion>> PostSuscripcion(Suscripcion suscripcion)
        {
            _context.Suscripciones.Add(suscripcion);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetSuscripcion", new { id = suscripcion.Id }, suscripcion);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuscripcion(int id)
        {
            var suscripcion = await _context.Suscripciones.FindAsync(id);
            if (suscripcion == null)
            {
                return NotFound();
            }
            suscripcion.IsDeleted = true;
            _context.Suscripciones.Update(suscripcion);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreSuscripcion(int id)
        {
            var suscripcion = await _context.Suscripciones.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == id);
            if (suscripcion == null)
            {
                return NotFound();
            }
            suscripcion.IsDeleted = false;
            _context.Suscripciones.Update(suscripcion);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool SuscripcionExists(int id)
        {
            return _context.Suscripciones.Any(e => e.Id == id);
        }
    }
}
