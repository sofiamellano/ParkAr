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
    public class PlanesController : ControllerBase
    {
        private readonly ParkARContext _context;

        public PlanesController(ParkARContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plan>>> GetPlanes([FromQuery] string? filtro = null)
        {
            var query = _context.Planes.AsNoTracking().Include(p => p.Suscripciones).AsQueryable();
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(p => p.Nombre.Contains(filtro));
            }
            return await query.ToListAsync();
        }

        [HttpGet("deleteds")]
        public async Task<ActionResult<IEnumerable<Plan>>> GetDeletedPlanes([FromQuery] string? filtro = null)
        {
            var query = _context.Planes.AsNoTracking().IgnoreQueryFilters().Where(p => p.IsDeleted).Include(p => p.Suscripciones).AsQueryable();
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(p => p.Nombre.Contains(filtro));
            }
            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Plan>> GetPlan(int id)
        {
            var plan = await _context.Planes.AsNoTracking().Include(p => p.Suscripciones).FirstOrDefaultAsync(p => p.Id == id);
            if (plan == null)
            {
                return NotFound();
            }
            return plan;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlan(int id, Plan plan)
        {
            if (id != plan.Id)
            {
                return BadRequest();
            }
            _context.Entry(plan).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanExists(id))
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
        public async Task<ActionResult<Plan>> PostPlan(Plan plan)
        {
            _context.Planes.Add(plan);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPlan", new { id = plan.Id }, plan);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            var plan = await _context.Planes.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }
            plan.IsDeleted = true;
            _context.Planes.Update(plan);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestorePlan(int id)
        {
            var plan = await _context.Planes.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);
            if (plan == null)
            {
                return NotFound();
            }
            plan.IsDeleted = false;
            _context.Planes.Update(plan);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool PlanExists(int id)
        {
            return _context.Planes.Any(e => e.Id == id);
        }
    }
}
