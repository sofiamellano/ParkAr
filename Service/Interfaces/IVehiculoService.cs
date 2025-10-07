using Service.Models;

namespace Service.Interfaces
{
    public interface IVehiculoService : IGenericService<Vehiculo>
    {
        Task<List<Vehiculo>?> GetByUsuarioAsync(int idUsuario);
    }
}