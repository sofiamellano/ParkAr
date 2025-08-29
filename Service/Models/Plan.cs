using Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int? Duracion { get; set; } // días de duración, NULL si es por hora
        public bool IsDeleted { get; set; }

        // Relaciones
        public ICollection<Suscripcion>? Suscripciones { get; set; }
    }
}
