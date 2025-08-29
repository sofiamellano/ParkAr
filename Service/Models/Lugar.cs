using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models
{
    public class Lugar
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public bool IsDeleted { get; set; }

        // Relaciones
        public ICollection<Reserva>? Reservas { get; set; }
    }
}
