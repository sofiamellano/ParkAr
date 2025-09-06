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
    public class ConfiguracionesController : ControllerBase
    {
        private readonly ParkARContext _context;

        public ConfiguracionesController(ParkARContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Configuracion>>> GetConfiguraciones([FromQuery] string? filtro = null)
        {
            var query = _context.Configuraciones.AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(c => c.NombreEmpresa.Contains(filtro));
            }
            return await query.ToListAsync();
        }

        [HttpGet("deleteds")]
        public async Task<ActionResult<IEnumerable<Configuracion>>> GetDeletedConfiguraciones([FromQuery] string? filtro = null)
        {
            var query = _context.Configuraciones.AsNoTracking().IgnoreQueryFilters().Where(c => c.IsDeleted).AsQueryable();
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(c => c.NombreEmpresa.Contains(filtro));
            }
            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Configuracion>> GetConfiguracion(int id)
        {
            var config = await _context.Configuraciones.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (config == null)
            {
                return NotFound();
            }
            return config;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutConfiguracion(int id, Configuracion configuracion)
        {
            if (id != configuracion.Id)
            {
                return BadRequest();
            }
            _context.Entry(configuracion).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfiguracionExists(id))
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
        public async Task<ActionResult<Configuracion>> PostConfiguracion(Configuracion configuracion)
        {
            _context.Configuraciones.Add(configuracion);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetConfiguracion", new { id = configuracion.Id }, configuracion);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfiguracion(int id)
        {
            var config = await _context.Configuraciones.FindAsync(id);
            if (config == null)
            {
                return NotFound();
            }
            config.IsDeleted = true;
            _context.Configuraciones.Update(config);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreConfiguracion(int id)
        {
            var config = await _context.Configuraciones.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == id);
            if (config == null)
            {
                return NotFound();
            }
            config.IsDeleted = false;
            _context.Configuraciones.Update(config);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ConfiguracionExists(int id)
        {
            return _context.Configuraciones.Any(e => e.Id == id);
        }
    }
}
