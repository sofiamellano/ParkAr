using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Models;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        FirebaseAuthClient firebaseAuthClient;
        IConfiguration _configuration;
        FirebaseAuthConfig _config;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            SettingFirebase();
        }
        private void SettingFirebase()
        {
            // Configuración de Firebase con proveedor de autenticación por correo electrónico y anónimo
            _config = new FirebaseAuthConfig
            {
                ApiKey = _configuration["ApiKeyFirebase"],
                AuthDomain = _configuration["AuthDomainFirebase"],
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()

                },
            };
                
            firebaseAuthClient = new FirebaseAuthClient(_config);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            try
            {
                var credential = await firebaseAuthClient.SignInWithEmailAndPasswordAsync(login.Username, login.Password);
                return Ok(credential.User.GetIdTokenAsync().Result);
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(new { message = ex.Reason.ToString() });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            try
            {
                var user = await firebaseAuthClient.CreateUserWithEmailAndPasswordAsync(register.Email, register.Password, register.Nombre);
                await SendVerificationEmailAsync(user.User.GetIdTokenAsync().Result);
                return Ok(user.User.GetIdTokenAsync().Result);
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(new { message = ex.Reason.ToString() });
            }
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] LoginDTO login)
        {
            try
            {
                await firebaseAuthClient.ResetEmailPasswordAsync(login.Username);
                return Ok();
            }
            catch (FirebaseAuthException ex)
            {
                return BadRequest(new { message = ex.Reason.ToString() });
            };
        }


        private async Task SendVerificationEmailAsync(string idToken)
        {
            using (var client = new HttpClient())
            {
                var RequestUri = "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=" + _config.ApiKey;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent("{\"requestType\":\"VERIFY_EMAIL\",\"idToken\":\"" + idToken + "\"}");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(RequestUri, content);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
