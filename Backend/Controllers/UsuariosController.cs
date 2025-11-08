using Backend.DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.DTOs;
using Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly ParkARContext _context;

        public UsuariosController(ParkARContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios([FromQuery] string? filtro = null)
        {
            var query = _context.Usuarios.AsNoTracking().Include(u => u.Vehiculos).Include(u => u.Suscripciones).Include(u => u.Reservas).AsQueryable();
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(u => u.Nombre.Contains(filtro) || u.Email.Contains(filtro));
            }
            return await query.ToListAsync();
        }

        [HttpGet("deleteds")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetDeletedUsuarios([FromQuery] string? filtro = null)
        {
            var query = _context.Usuarios.AsNoTracking().IgnoreQueryFilters().Where(u => u.IsDeleted).Include(u => u.Vehiculos).Include(u => u.Suscripciones).Include(u => u.Reservas).AsQueryable();
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(u => u.Nombre.Contains(filtro) || u.Email.Contains(filtro));
            }
            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.AsNoTracking().Include(u => u.Vehiculos).Include(u => u.Suscripciones).Include(u => u.Reservas).FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }
            return usuario;
        }

        [HttpGet("byemail")]
        public async Task<ActionResult<Usuario>> GetByEmailUsuario([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("El parametro email es obligatorio.");
            }
            var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (usuario == null)
            {
                return NotFound();
            }
            return usuario;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }
            _context.Entry(usuario).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            // Validar que el email no esté vacío
            if (string.IsNullOrWhiteSpace(usuario.Email))
            {
                return BadRequest(new { message = "El email es obligatorio." });
            }

            // Verificar si ya existe un usuario con ese email
            var usuarioExistente = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == usuario.Email.ToLower());

            if (usuarioExistente != null)
            {
                return Conflict(new { message = $"Ya existe un usuario registrado con el email '{usuario.Email}'." });
            }

            _context.Usuarios.Add(usuario);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UsuarioExists(usuario.Id))
                {
                    return Conflict("Ya existe un usuario con el mismo ID");
                }
                else
                {
                    throw new Exception("Ocurrió un error al crear el usuario");
                }
            }

            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        //[HttpPost("login")]
        //public async Task<ActionResult<bool>> LoginInSystem([FromBody] LoginDTO loginDTO)
        //{
        //    var usuario = await _context.Usuarios
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(u => u.Email.Equals(loginDTO.Username) &&
        //                                  u.Password.Equals(loginDTO.Password.GetHashSha256()));
        //    if (usuario == null)
        //        return Unauthorized("Credenciales inválidas");
        //    return true;
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            usuario.IsDeleted = true;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("restore/{id}")]
        public async Task<IActionResult> RestoreUsuario(int id)
        {
            var usuario = await _context.Usuarios.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }
            usuario.IsDeleted = false;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
