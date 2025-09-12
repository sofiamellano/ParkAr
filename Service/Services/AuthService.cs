using Microsoft.Extensions.Configuration;
using Service.DTOs;
using Service.Interfaces;
using Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        public AuthService(){ }
        public async Task<string?> Login(LoginDTO? login)
        {
            if (login == null)
            {
                throw new ArgumentException("El objeto login no llego.");
            }
            try
            {
                var UrlApi = Properties.Resources.UrlApi;
                var endpointAuth = ApiEndpoints.GetEndpoint("Login");
                var client = new HttpClient();
                var response = await client.PostAsJsonAsync($"{UrlApi}{endpointAuth}/login",login);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en el login " + ex.Message);
            }
        }
    }
}
