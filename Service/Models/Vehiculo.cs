using Service.Enums;
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
        public string Patente { get; set; } = null!;
        public TipoVehiculoEnum TipoVehiculo { get; set; }
        public int UsuarioId { get; set; }
        public bool IsDeleted { get; set; }

        // Relaciones
        public Usuario? Usuario { get; set; }
        public ICollection<Reserva>? Reservas { get; set; }

        public override string ToString()
        {
            return $"{Patente} ({TipoVehiculo})";
        }
    }
}
