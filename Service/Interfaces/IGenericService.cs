using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IGenericService<T> where T : class
    {
        Task<List<T>?> GetAllAsync(string? filtro = "");
        Task<List<T>?> GetAllDeletedsAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T?> AddAsync(T? entity);
        Task<bool> UpdateAsync(T? entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
    }
}