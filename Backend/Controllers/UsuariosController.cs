using Backend.DataContext;
using Microsoft.AspNetCore.Mvc;
using Service.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ParkARContext _context;
        public UsuariosController(ParkARContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetUsuarios()
        {
            var usuarios = _context.Usuarios.Where(u => !u.IsDeleted).ToList();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public IActionResult GetUsuario(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == id && !u.IsDeleted);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [HttpPost]
        public IActionResult CreateUsuario([FromBody] Usuario usuario)
        {
            if (usuario == null)
            {
                return BadRequest();
            }
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUsuario(int id, [FromBody] Usuario usuario)
        {
            if (usuario == null || usuario.Id != id)
            {
                return BadRequest();
            }
            var existingUsuario = _context.Usuarios.Find(id);
            if (existingUsuario == null || existingUsuario.IsDeleted)
            {
                return NotFound();
            }

            existingUsuario.Nombre = usuario.Nombre;
            existingUsuario.Email = usuario.Email;
            existingUsuario.Password = usuario.Password;
            existingUsuario.TipoUsuario = usuario.TipoUsuario;

            _context.Usuarios.Update(existingUsuario);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUsuario(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null || usuario.IsDeleted)
            {
                return NotFound();
            }
            usuario.IsDeleted = true; // Soft delete
            _context.SaveChanges();
            return NoContent();
        }
    }
}
