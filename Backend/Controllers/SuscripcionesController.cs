using Backend.DataContext;
using Microsoft.AspNetCore.Mvc;
using Service.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuscripcionesController : ControllerBase
    {
        private readonly ParkARContext _context;
        public SuscripcionesController(ParkARContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetSuscripciones()
        {
            var suscripciones = _context.Suscripciones.Where(s => !s.IsDeleted).ToList();
            return Ok(suscripciones);
        }

        [HttpGet("{id}")]
        public IActionResult GetSuscripcion(int id)
        {
            var suscripcion = _context.Suscripciones.FirstOrDefault(s => s.Id == id && !s.IsDeleted);
            if (suscripcion == null) return NotFound();
            return Ok(suscripcion);
        }

        [HttpPost]
        public IActionResult CreateSuscripcion([FromBody] Suscripcion suscripcion)
        {
            if (suscripcion == null) return BadRequest();
            _context.Suscripciones.Add(suscripcion);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetSuscripcion), new { id = suscripcion.Id }, suscripcion);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSuscripcion(int id, [FromBody] Suscripcion suscripcion)
        {
            if (suscripcion == null || suscripcion.Id != id) return BadRequest();
            var existing = _context.Suscripciones.Find(id);
            if (existing == null) return NotFound();

            existing.PlanId = suscripcion.PlanId;
            existing.UsuarioId = suscripcion.UsuarioId;
            existing.FechaInicio = suscripcion.FechaInicio;
            existing.FechaFin = suscripcion.FechaFin;
            existing.Estado = suscripcion.Estado;

            _context.Suscripciones.Update(existing);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSuscripcion(int id)
        {
            var suscripcion = _context.Suscripciones.Find(id);
            if (suscripcion == null) return NotFound();
            suscripcion.IsDeleted = true;
            _context.SaveChanges();
            return NoContent();
        }
    }
}
