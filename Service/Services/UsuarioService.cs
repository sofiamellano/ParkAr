using Service.Interfaces;
using Service.Models;
using Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.Services
{
    public class UsuarioService : GenericService<Usuario>, IUsuarioService
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _endpoint;
        protected readonly JsonSerializerOptions _options;
        public static string? jwtToken = string.Empty;

        public UsuarioService(HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _endpoint = Properties.Resources.UrlApi + ApiEndpoints.GetEndpoint(typeof(Usuario).Name);

            if (!string.IsNullOrEmpty(GenericService<object>.jwtToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GenericService<object>.jwtToken);
            else
                throw new ArgumentException("Token no definido.", nameof(GenericService<object>.jwtToken));
        }
        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{_endpoint}/byemail?email={email}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener los datos: {response.StatusCode}");
            }
            return JsonSerializer.Deserialize<Usuario>(content, _options);
        }
    }
}
