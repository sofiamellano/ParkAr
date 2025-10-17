using Microsoft.Extensions.Caching.Memory;
using Service.Interfaces;
using Service.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.Services
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _endpoint;
        protected readonly JsonSerializerOptions _options;
        public static string? jwtToken = string.Empty;
        private readonly IMemoryCache? _memoryCache;

        public GenericService(HttpClient? httpClient = null, IMemoryCache? memoryCache = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            _memoryCache = memoryCache;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (_httpClient.BaseAddress is null)
            {
                _httpClient.BaseAddress = new Uri(Properties.Resources.UrlApi);
            }

            _endpoint = ApiEndpoints.GetEndpoint(typeof(T).Name);
        }

        protected void SetAuthorizationHeader()
        {
            // Si ya está configurado (por un DelegatingHandler), no hacer nada
            if (_httpClient.DefaultRequestHeaders.Authorization is not null)
                return;

            // 1) Intentar leer desde IMemoryCache (configurado por FirebaseAuthService)
            if (_memoryCache is not null && _memoryCache.TryGetValue("jwt", out string? cachedToken) && !string.IsNullOrWhiteSpace(cachedToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cachedToken);
                return;
            }

            // 2) Respaldo: variable estática (evitar uso si no es necesario)
            if (!string.IsNullOrWhiteSpace(GenericService<object>.jwtToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GenericService<object>.jwtToken);
                return;
            }
            // Si no se definió el token, se lanza una excepción

            throw new InvalidOperationException("El token JWT no está disponible para la autorización.");
        }
        public async Task<T?> AddAsync(T? entity)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync(_endpoint, entity);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al agregar el dato: {response.StatusCode} - {content}");
            }
            return JsonSerializer.Deserialize<T>(content, _options);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"{_endpoint}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al eliminar el dato: {response.StatusCode}");
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<List<T>?> GetAllAsync(string? filtro = "")
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_endpoint}?filtro={filtro}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<T>>(content, _options);
            }
            else
            {
                throw new Exception("Error al obtener los datos");
            }
        }

        public async Task<List<T>?> GetAllDeletedsAsync()
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_endpoint}/deleteds");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener los datos: {response.StatusCode}");
            }
            return JsonSerializer.Deserialize<List<T>>(content, _options);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_endpoint}/{id}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener los datos: {response.StatusCode}");
            }
            return JsonSerializer.Deserialize<T>(content, _options);
        }

        public async Task<bool> RestoreAsync(int id)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PutAsync($"{_endpoint}/restore/{id}", null);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al restaurar el dato: {response.StatusCode}");
            }
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(T? entity)
        {
            SetAuthorizationHeader();
            var idValue = entity!.GetType().GetProperty("Id")!.GetValue(entity);
            var response = await _httpClient.PutAsJsonAsync($"{_endpoint}/{idValue}", entity);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Hubo un problema al actualizar");
            }
            else
            {
                return response.IsSuccessStatusCode;
            }
        }
    }
}
