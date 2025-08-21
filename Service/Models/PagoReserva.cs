using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public class PagoReserva
    {
        public int PagoId { get; set; }
        public Pago? Pago { get; set; }

        public int ReservaId { get; set; }
        public Reserva? Reserva { get; set; }
    }

}
