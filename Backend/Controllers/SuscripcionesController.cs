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
            var query = _context.Suscripciones.AsNoTracking().Include(s => s.Usuario).Include(s => s.Plan).AsQueryable();
            if (usuarioId.HasValue)
            {
                query = query.Where(s => s.UsuarioId == usuarioId.Value);
            }
            return await query.ToListAsync();
        }

        [HttpGet("byusuario")]
        public async Task<ActionResult<List<Suscripcion>>> GetByUsuario([FromQuery] int idusuario)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[API] Buscando suscripciones para usuario: {idusuario}");

                if (idusuario <= 0)
                {
                    return BadRequest("El ID de usuario debe ser mayor que 0");
                }

                // Incluir Plan en la consulta
                var suscripciones = await _context.Suscripciones
                    .Where(s => s.UsuarioId == idusuario && !s.IsDeleted)
                    .Include(s => s.Plan) // ✅ IMPORTANTE: Incluir Plan
                    .Include(s => s.Usuario) // Opcional
                    .OrderByDescending(s => s.FechaInicio)
                    .ToListAsync();

                System.Diagnostics.Debug.WriteLine($"[API] Suscripciones encontradas: {suscripciones.Count}");

                return Ok(suscripciones);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[API] Error: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("deleteds")]
        public async Task<ActionResult<IEnumerable<Suscripcion>>> GetDeletedSuscripciones([FromQuery] int? usuarioId = null)
        {
            var query = _context.Suscripciones.AsNoTracking().IgnoreQueryFilters().Where(s => s.IsDeleted).Include(s => s.Usuario).Include(s => s.Plan).AsQueryable();
            if (usuarioId.HasValue)
            {
                query = query.Where(s => s.UsuarioId == usuarioId.Value);
            }
            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Suscripcion>> GetSuscripcion(int id)
        {
            var suscripcion = await _context.Suscripciones.AsNoTracking().Include(s => s.Usuario).Include(s => s.Plan).FirstOrDefaultAsync(s => s.Id == id);
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
