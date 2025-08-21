using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public class SuscripcionCliente
    {
        public int Id { get; set; }

        // FK
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int PlanSuscripcionId { get; set; }
        public PlanSuscripcion? PlanSuscripcion { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public EstadoSuscripcion EstadoSuscripcion { get; set; }

        public ICollection<ReservaSuscripcion>? ReservasSuscripciones { get; set; }
        public ICollection<PagoSuscripcion>? PagosSuscripciones { get; set; }
    }

}
