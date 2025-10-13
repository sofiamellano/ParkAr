using Service.Models;

namespace Service.Interfaces
{
    public interface ISuscripcionService : IGenericService<Suscripcion>
    {
        Task<List<Suscripcion>?> GetByUsuarioAsync(int idUsuario);
        Task<List<Suscripcion>?> GetActivasAsync();
    }
}