using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public RolUsuario TipoUsuario { get; set; } // enum

        // Relaciones
        public ICollection<Vehiculo>? Vehiculos { get; set; }
        public ICollection<SuscripcionCliente>? Suscripciones { get; set; }
        public ICollection<Reserva>? Reservas { get; set; }
        public ICollection<Pago>? Pagos { get; set; }
    }

}
