using Service.Interfaces;
using Service.Models;
using System.Text.Json;

namespace Service.Services
{
    public class VehiculoService : GenericService<Vehiculo>, IVehiculoService
    {
        public VehiculoService(HttpClient? httpClient = null) : base(httpClient)
        {
        }

        public async Task<List<Vehiculo>?> GetByUsuarioAsync(int idUsuario)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_endpoint}/byusuario?idusuario={idUsuario}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener los vehículos: {response.StatusCode}");
            }
            return JsonSerializer.Deserialize<List<Vehiculo>>(content, _options);
        }
    }
}