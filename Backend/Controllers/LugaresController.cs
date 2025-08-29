using Backend.DataContext;
using Microsoft.AspNetCore.Mvc;
using Service.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LugaresController : ControllerBase
    {
        private readonly ParkARContext _context;
        public LugaresController(ParkARContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetLugares()
        {
            var lugares = _context.Lugares.Where(l => !l.IsDeleted).ToList();
            return Ok(lugares);
        }

        [HttpGet("{id}")]
        public IActionResult GetLugar(int id)
        {
            var lugar = _context.Lugares.FirstOrDefault(l => l.Id == id && !l.IsDeleted);
            if (lugar == null) return NotFound();
            return Ok(lugar);
        }

        [HttpPost]
        public IActionResult CreateLugar([FromBody] Lugar lugar)
        {
            if (lugar == null) return BadRequest();
            _context.Lugares.Add(lugar);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetLugar), new { id = lugar.Id }, lugar);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateLugar(int id, [FromBody] Lugar lugar)
        {
            if (lugar == null || lugar.Id != id) return BadRequest();
            var existing = _context.Lugares.Find(id);
            if (existing == null) return NotFound();

            existing.Numero = lugar.Numero;

            _context.Lugares.Update(existing);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteLugar(int id)
        {
            var lugar = _context.Lugares.Find(id);
            if (lugar == null) return NotFound();
            lugar.IsDeleted = true;
            _context.SaveChanges();
            return NoContent();
        }
    }
}
