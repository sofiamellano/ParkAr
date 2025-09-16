using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IReservaService : IGenericService<Reserva> 
    {
        public Task<List<Reserva>?> GetByUsuarioAsync(int idUsuario);
    }
}