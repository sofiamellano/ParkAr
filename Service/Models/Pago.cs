using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public class Pago
    {
        public int Id { get; set; }

        // FK
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public decimal Monto { get; set; }
        public MetodoPago Metodo { get; set; } // enum
        public DateTime Fecha { get; set; }
        public ConceptoPago Concepto { get; set; } // enum

        public ICollection<PagoReserva>? PagosReservas { get; set; }
        public ICollection<PagoSuscripcion>? PagosSuscripciones { get; set; }
    }

}
