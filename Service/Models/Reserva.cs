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

        // FK
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int VehiculoId { get; set; }
        public Vehiculo? Vehiculo { get; set; }

        public int LugarId { get; set; }
        public Lugar? Lugar { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public EstadoReserva EstadoReserva { get; set; }

        public ICollection<ReservaSuscripcion>? ReservasSuscripciones { get; set; }
        public ICollection<PagoReserva>? PagosReservas { get; set; }
    }

}
