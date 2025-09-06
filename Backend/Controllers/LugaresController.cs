using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
