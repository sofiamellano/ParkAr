using Microsoft.Extensions.Caching.Memory;
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
    public class ReservaService : GenericService<Reserva>, IReservaService
    {

        public ReservaService(HttpClient? httpClient = null, IMemoryCache? memoryCache = null) : base(httpClient, memoryCache)
        {
        }
        public async Task<List<Reserva>?> GetByUsuarioAsync(int idUsuario)
        {
            // Validar que el ID de usuario sea válido
            if (idUsuario <= 0)
            {
                throw new ArgumentException("El ID de usuario debe ser mayor que 0. Verifica que el usuario esté logueado correctamente.", nameof(idUsuario));
            }

            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_endpoint}/byusuario?idusuario={idUsuario}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                // Incluir el contenido de la respuesta para mejor debugging
                throw new Exception($"Error al obtener los datos: {response.StatusCode} - {content}");
            }
            return JsonSerializer.Deserialize<List<Reserva>>(content, _options);
        }
    }
}
