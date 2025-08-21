using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public class PagoSuscripcion
    {
        public int PagoId { get; set; }
        public Pago? Pago { get; set; }

        public int SuscripcionClienteId { get; set; }
        public SuscripcionCliente? SuscripcionCliente { get; set; }
    }

}
