using Microsoft.Extensions.Caching.Memory;
using Service.Interfaces;
using Service.Models;
using System.Text.Json;

namespace Service.Services
{
    public class SuscripcionService : GenericService<Suscripcion>, ISuscripcionService
    {
        public SuscripcionService(HttpClient? httpClient = null, IMemoryCache? memoryCache = null) : base(httpClient, memoryCache)
        {
        }

        public async Task<List<Suscripcion>?> GetByUsuarioAsync(int idUsuario)
        {
            SetAuthorizationHeader();
            
            var url = $"{_endpoint}/byusuario?idusuario={idUsuario}";
            System.Diagnostics.Debug.WriteLine($"[SERVICE] Llamando URL: {url}");
            System.Diagnostics.Debug.WriteLine($"[SERVICE] Base URL: {_httpClient.BaseAddress}");
            System.Diagnostics.Debug.WriteLine($"[SERVICE] JWT Token length: {jwtToken?.Length ?? 0}");
            
            var response = await _httpClient.GetAsync($"{_endpoint}/byusuario?idusuario={idUsuario}");
            var content = await response.Content.ReadAsStringAsync();
            
            System.Diagnostics.Debug.WriteLine($"[SERVICE] Response Status: {response.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"[SERVICE] Response Content: {content}");
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener las suscripciones: {response.StatusCode} - {content}");
            }
            return JsonSerializer.Deserialize<List<Suscripcion>>(content, _options);
        }

        public async Task<List<Suscripcion>?> GetActivasAsync()
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_endpoint}/activas");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener las suscripciones activas: {response.StatusCode}");
            }
            return JsonSerializer.Deserialize<List<Suscripcion>>(content, _options);
        }
    }
}