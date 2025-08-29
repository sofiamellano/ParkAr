using Backend.DataContext;
using Microsoft.AspNetCore.Mvc;
using Service.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanesController : ControllerBase
    {
        private readonly ParkARContext _context;
        public PlanesController(ParkARContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetPlanes()
        {
            var planes = _context.Planes.Where(p => !p.IsDeleted).ToList();
            return Ok(planes);
        }

        [HttpGet("{id}")]
        public IActionResult GetPlan(int id)
        {
            var plan = _context.Planes.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpPost]
        public IActionResult CreatePlan([FromBody] Plan plan)
        {
            if (plan == null) return BadRequest();
            _context.Planes.Add(plan);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetPlan), new { id = plan.Id }, plan);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePlan(int id, [FromBody] Plan plan)
        {
            if (plan == null || plan.Id != id) return BadRequest();
            var existing = _context.Planes.Find(id);
            if (existing == null) return NotFound();

            existing.Nombre = plan.Nombre;
            existing.Descripcion = plan.Descripcion;
            existing.Precio = plan.Precio;
            existing.Duracion = plan.Duracion;

            _context.Planes.Update(existing);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePlan(int id)
        {
            var plan = _context.Planes.Find(id);
            if (plan == null) return NotFound();
            plan.IsDeleted = true;
            _context.SaveChanges();
            return NoContent();
        }
    }
}
