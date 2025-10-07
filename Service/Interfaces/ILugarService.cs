using Service.Models;

namespace Service.Interfaces
{
    public interface ILugarService : IGenericService<Lugar>
    {
        Task<List<Lugar>?> GetDisponiblesAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}