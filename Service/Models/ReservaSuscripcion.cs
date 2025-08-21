using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public class ReservaSuscripcion
    {
        public int ReservaId { get; set; }
        public Reserva? Reserva { get; set; }

        public int SuscripcionClienteId { get; set; }
        public SuscripcionCliente? SuscripcionCliente { get; set; }
    }

}
