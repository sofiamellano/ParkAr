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
                var response = await client.PostAsJsonAsync($"{UrlApi}{endpointAuth}/login/", login);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    GenericService<object>.jwtToken = result;
                    return null;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return errorContent;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en el login " + ex.Message);
            }
        }

        public async Task<bool> ResetPassword(LoginDTO? loginReset)
        {
            if (loginReset == null)
            {
                throw new ArgumentException("El objeto loginReset no llegó.");
            }
            try
            {
                var UrlApi = Properties.Resources.UrlApi;
                var endpointAuth = ApiEndpoints.GetEndpoint("Login");
                var client = new HttpClient();
                var response = await client.PostAsJsonAsync($"{UrlApi}{endpointAuth}/resetpassword/", loginReset);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al recuperar contraseña: " + ex.Message);
            }
        }

        public async Task<bool> CreateUserWithEmailAndPasswordAsync(string email, string password, string nombre)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nombre))
            {
                throw new ArgumentException("Email, password o nombre no pueden ser nulos o vacíos.");
            }
            try
            {
                var UrlApi = Properties.Resources.UrlApi;
                var endpointAuth = ApiEndpoints.GetEndpoint("Login");
                var client = new HttpClient();
                var newUser = new RegisterDTO { Email = email, Password = password, Nombre = nombre };
                var response = await client.PostAsJsonAsync($"{UrlApi}{endpointAuth}/register/", newUser);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    GenericService<object>.jwtToken = result;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear usuario: " + ex.Message);
            }
        }
    }
}
