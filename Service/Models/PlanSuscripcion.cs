using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public class PlanSuscripcion
    {
        public int Id { get; set; }
        public TipoSuscripcion Nombre { get; set; } // enum
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int? Duracion { get; set; } // null si es por hora

        public ICollection<SuscripcionCliente>? SuscripcionesClientes { get; set; }
    }

}
