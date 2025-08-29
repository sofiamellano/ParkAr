using Service.Enums;
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
        public int UsuarioId { get; set; }
        public int? ReservaId { get; set; }
        public int? SuscripcionId { get; set; }
        public decimal Monto { get; set; }
        public MetodoPagoEnum Metodo { get; set; }
        public DateTime Fecha { get; set; }
        public ConceptoPagoEnum Concepto { get; set; }
        public bool IsDeleted { get; set; }

        // Relaciones
        public Usuario? Usuario { get; set; }
        public Reserva? Reserva { get; set; }
        public Suscripcion? Suscripcion { get; set; }
    }
}
