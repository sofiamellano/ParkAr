using Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public class Suscripcion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int PlanId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public EstadoSuscripcionEnum Estado { get; set; }
        public bool IsDeleted { get; set; }

        // Relaciones
        public Usuario? Usuario { get; set; }
        public Plan? Plan { get; set; }
        public ICollection<Pago>? Pagos { get; set; }
    }
}
