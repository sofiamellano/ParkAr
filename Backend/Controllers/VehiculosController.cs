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
    public class VehiculosController : ControllerBase
    {
        private readonly ParkARContext _context;

        public VehiculosController(ParkARContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehiculo>>> GetVehiculos([FromQuery] string? filtro = null)
        {
            var query = _context.Vehiculos.AsNoTracking().Include(v => v.Usuario).Include(v => v.Reservas).AsQueryable();
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(v => v.Patente.Contains(filtro));
            }
            return await query.ToListAsync();
        }

        [HttpGet("deleteds")]
        public async Task<ActionResult<IEnumerable<Vehiculo>>> GetDeletedVehiculos([FromQuery] string? filtro = null)
        {
            var query = _context.Vehiculos.AsNoTracking().IgnoreQueryFilters().Where(v => v.IsDeleted).Include(v => v.Usuario).Include(v => v.Reservas).AsQueryable();
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(v => v.Patente.Contains(filtro));
            }
            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vehiculo>> GetVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos.AsNoTracking().Include(v => v.Usuario).Include(v => v.Reservas).FirstOrDefaultAsync(v => v.Id == id);
            if (vehiculo == null)
            {
                return NotFound();
            }
            return vehiculo;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVehiculo(int id, Vehiculo vehiculo)
        {
            if (id != vehiculo.Id)
            {
                return BadRequest();
            }
            _context.Entry(vehiculo).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculoExists(id))
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
        public async Task<ActionResult<Vehiculo>> PostVehiculo(Vehiculo vehiculo)
        {
            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetVehiculo", new { id = vehiculo.Id }, vehiculo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
            {
                return NotFound();
            }
            vehiculo.IsDeleted = true;
            _context.Vehiculos.Update(vehiculo);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos.IgnoreQueryFilters().FirstOrDefaultAsync(v => v.Id == id);
            if (vehiculo == null)
            {
                return NotFound();
            }
            vehiculo.IsDeleted = false;
            _context.Vehiculos.Update(vehiculo);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool VehiculoExists(int id)
        {
            return _context.Vehiculos.Any(e => e.Id == id);
        }
    }
}
