using Microsoft.Extensions.Caching.Memory;
using Service.DTOs;
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
        public UsuarioService(HttpClient? httpClient = null, IMemoryCache? memoryCache = null) : base(httpClient, memoryCache)
        { }
        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"{_endpoint}/byemail?email={email}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al obtener los datos: {response.StatusCode}");
            }
            return JsonSerializer.Deserialize<Usuario>(content, _options);
        }

        //public async Task<bool> LoginInSystem(string email, string password)
        //{
        //    var loginDTO = new LoginDTO
        //    {
        //        Username = email,
        //        Password = password
        //    };
        //    var response = await _httpClient.PostAsJsonAsync($"{_endpoint}/login", loginDTO);
        //    var content = await response.Content.ReadAsStringAsync();
        //    if (!response.IsSuccessStatusCode)
        //    {
        //        throw new Exception($"Error al iniciar sesión: {response.StatusCode}");
        //    }
        //    var result = JsonSerializer.Deserialize<bool>(content, _options);
        //    return result;
        //}
    }
}
