using Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int VehiculoId { get; set; }
        public int LugarId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public EstadoReservaEnum EstadoReserva { get; set; }
        public bool IsDeleted { get; set; }

        // Relaciones
        public Usuario? Usuario { get; set; }
        public Vehiculo? Vehiculo { get; set; }
        public Lugar? Lugar { get; set; }
        public ICollection<Pago>? Pagos { get; set; }
    }

}
