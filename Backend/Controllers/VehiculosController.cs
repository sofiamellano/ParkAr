using Backend.DataContext;
using Microsoft.AspNetCore.Mvc;
using Service.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiculosController : ControllerBase
    {
        private readonly ParkARContext _context;
        public VehiculosController(ParkARContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetVehiculos()
        {
            var vehiculos = _context.Vehiculos.Where(v => !v.IsDeleted).ToList();
            return Ok(vehiculos);
        }

        [HttpGet("{id}")]
        public IActionResult GetVehiculo(int id)
        {
            var vehiculo = _context.Vehiculos.FirstOrDefault(v => v.Id == id && !v.IsDeleted);
            if (vehiculo == null) return NotFound();
            return Ok(vehiculo);
        }

        [HttpPost]
        public IActionResult CreateVehiculo([FromBody] Vehiculo vehiculo)
        {
            if (vehiculo == null) return BadRequest();
            _context.Vehiculos.Add(vehiculo);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetVehiculo), new { id = vehiculo.Id }, vehiculo);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateVehiculo(int id, [FromBody] Vehiculo vehiculo)
        {
            if (vehiculo == null || vehiculo.Id != id) return BadRequest();
            var existing = _context.Vehiculos.Find(id);
            if (existing == null) return NotFound();

            existing.Patente = vehiculo.Patente;
            existing.TipoVehiculo = vehiculo.TipoVehiculo;
            existing.UsuarioId = vehiculo.UsuarioId;

            _context.Vehiculos.Update(existing);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteVehiculo(int id)
        {
            var vehiculo = _context.Vehiculos.Find(id);
            if (vehiculo == null) return NotFound();
            vehiculo.IsDeleted = true; // Soft delete
            _context.SaveChanges();
            return NoContent();
        }
    }
}
