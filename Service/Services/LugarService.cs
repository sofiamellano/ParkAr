using Microsoft.Extensions.Caching.Memory;
using Service.Interfaces;
using Service.Models;
using System.Text.Json;

namespace Service.Services
{
    public class LugarService : GenericService<Lugar>, ILugarService
    {
        public LugarService(HttpClient? httpClient = null, IMemoryCache? memoryCache = null) : base(httpClient, memoryCache)
        {
        }

        public async Task<List<Lugar>?> GetDisponiblesAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            SetAuthorizationHeader();
            var fechaInicioStr = fechaInicio.ToString("yyyy-MM-ddTHH:mm:ss");
            var fechaFinStr = fechaFin.ToString("yyyy-MM-ddTHH:mm:ss");
            
            var response = await _httpClient.GetAsync($"{_endpoint}/disponibles?fechaInicio={fechaInicioStr}&fechaFin={fechaFinStr}");
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener lugares disponibles: {response.StatusCode}");
            }
            
            return JsonSerializer.Deserialize<List<Lugar>>(content, _options);
        }
    }
}