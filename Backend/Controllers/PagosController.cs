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
    public class PagosController : ControllerBase
    {
        private readonly ParkARContext _context;

        public PagosController(ParkARContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagos([FromQuery] int? usuarioId = null)
        {
            var query = _context.Pagos.AsNoTracking().Include(p => p.Usuario).Include(p => p.Reserva).Include(p => p.Suscripcion).AsQueryable();
            if (usuarioId.HasValue)
            {
                query = query.Where(p => p.UsuarioId == usuarioId.Value);
            }
            return await query.ToListAsync();
        }

        [HttpGet("deleteds")]
        public async Task<ActionResult<IEnumerable<Pago>>> GetDeletedPagos([FromQuery] int? usuarioId = null)
        {
            var query = _context.Pagos.AsNoTracking().IgnoreQueryFilters().Where(p => p.IsDeleted).Include(p => p.Usuario).Include(p => p.Reserva).Include(p => p.Suscripcion).AsQueryable();
            if (usuarioId.HasValue)
            {
                query = query.Where(p => p.UsuarioId == usuarioId.Value);
            }
            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pago>> GetPago(int id)
        {
            var pago = await _context.Pagos.AsNoTracking().Include(p => p.Usuario).Include(p => p.Reserva).Include(p => p.Suscripcion).FirstOrDefaultAsync(p => p.Id == id);
            if (pago == null)
            {
                return NotFound();
            }
            return pago;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPago(int id, Pago pago)
        {
            if (id != pago.Id)
            {
                return BadRequest();
            }
            _context.Entry(pago).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PagoExists(id))
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
        public async Task<ActionResult<Pago>> PostPago(Pago pago)
        {
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPago", new { id = pago.Id }, pago);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePago(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null)
            {
                return NotFound();
            }
            pago.IsDeleted = true;
            _context.Pagos.Update(pago);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestorePago(int id)
        {
            var pago = await _context.Pagos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);
            if (pago == null)
            {
                return NotFound();
            }
            pago.IsDeleted = false;
            _context.Pagos.Update(pago);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool PagoExists(int id)
        {
            return _context.Pagos.Any(e => e.Id == id);
        }
    }
}
