using Service.Enums;
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
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public TipoUsuarioEnum TipoUsuario { get; set; }
        public bool IsDeleted { get; set; }

        // Relaciones
        public ICollection<Vehiculo>? Vehiculos { get; set; }
        public ICollection<Suscripcion>? Suscripciones { get; set; }
        public ICollection<Reserva>? Reservas { get; set; }

        public override string ToString()
        {
            return $"{Nombre} ({Email})";
        }
    }

}
