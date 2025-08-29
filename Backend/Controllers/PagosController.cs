using Backend.DataContext;
using Microsoft.AspNetCore.Mvc;
using Service.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly ParkARContext _context;
        public PagosController(ParkARContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetPagos()
        {
            var pagos = _context.Pagos.Where(p => !p.IsDeleted).ToList();
            return Ok(pagos);
        }

        [HttpGet("{id}")]
        public IActionResult GetPago(int id)
        {
            var pago = _context.Pagos.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
            if (pago == null) return NotFound();
            return Ok(pago);
        }

        [HttpPost]
        public IActionResult CreatePago([FromBody] Pago pago)
        {
            if (pago == null) return BadRequest();
            _context.Pagos.Add(pago);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetPago), new { id = pago.Id }, pago);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePago(int id, [FromBody] Pago pago)
        {
            if (pago == null || pago.Id != id) return BadRequest();
            var existing = _context.Pagos.Find(id);
            if (existing == null) return NotFound();

            existing.Monto = pago.Monto;
            existing.Metodo = pago.Metodo;
            existing.Fecha = pago.Fecha;
            existing.Concepto = pago.Concepto;
            existing.UsuarioId = pago.UsuarioId;
            existing.ReservaId = pago.ReservaId;
            existing.SuscripcionId = pago.SuscripcionId;

            _context.Pagos.Update(existing);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePago(int id)
        {
            var pago = _context.Pagos.Find(id);
            if (pago == null) return NotFound();
            pago.IsDeleted = true;
            _context.SaveChanges();
            return NoContent();
        }
    }
}
