using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public class Vehiculo
    {
        public int Id { get; set; }
        public string Patente { get; set; } = string.Empty;
        public string TipoVehiculo { get; set; } = string.Empty; // podría ser enum también

        // FK
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public ICollection<Reserva>? Reservas { get; set; }
    }

}
