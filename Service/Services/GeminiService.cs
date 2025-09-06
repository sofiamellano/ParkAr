using Microsoft.Extensions.Configuration;
using Service.Interfaces;
using Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class GeminiService : IGenimiService
    {
        private readonly IConfiguration _configuration;
        public GeminiService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string?> GetPrompt(string textPrompt)
        {
            if (string.IsNullOrEmpty(textPrompt))
            {
                throw new ArgumentException("El texto del prompt no puede ser nulo o vacío.", nameof(textPrompt));
            }
            try 
            {
                var UrlApi = _configuration["UrlApi"];
                var endpointGemini = ApiEndpoints.GetEndpoint("Gemini");
                var client = new HttpClient();
                var response = await client.GetAsync($"{UrlApi}{endpointGemini}/prompt/{textPrompt}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    throw new Exception($"Error en la respuesta de la API:{response.StatusCode} -{response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el prompt de Gemini." + ex.Message);
            }
        }
    }
}
