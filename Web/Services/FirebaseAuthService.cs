using Firebase.Auth;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.JSInterop;
using Service.Models.Login;
using Service.Services;


namespace Web.Services
{
    public class FirebaseAuthService
    {
        private readonly IJSRuntime _jsRuntime;
        public event Action OnChangeLogin;
        public FirebaseUser CurrentUser { get; set; }
        private IMemoryCache _memoryCache;

        public FirebaseAuthService(IJSRuntime jsRuntime, IMemoryCache memoryCache)
        {
            _jsRuntime = jsRuntime;
            _memoryCache = memoryCache;
        }

        public async Task<FirebaseUser?> SignInWithEmailPassword(string email, string password, bool rememberPassword)
        {
            var user = await _jsRuntime.InvokeAsync<FirebaseUser?>("firebaseAuth.signInWithEmailPassword", email, password, rememberPassword);
            if (user != null)
            {
                CurrentUser = user;
                await SetUserToken();
                OnChangeLogin?.Invoke();
            }
            return user;
        }

        public async Task<string> createUserWithEmailAndPassword(string email, string password, string displayName)
        {
            var userId = await _jsRuntime.InvokeAsync<string>("firebaseAuth.createUserWithEmailAndPassword", email, password, displayName);
            if (userId != null)
            {
                OnChangeLogin?.Invoke();
            }
            return userId;
        }

        public async Task SignOut()
        {
            await _jsRuntime.InvokeVoidAsync("firebaseAuth.signOut");
            CurrentUser = null;
            _memoryCache.Remove("jwt");
            OnChangeLogin?.Invoke();
        }

        public async Task<FirebaseUser?> GetUserFirebase()
        {
            var userFirebase = await _jsRuntime.InvokeAsync<FirebaseUser>("firebaseAuth.getUserFirebase");
            if (userFirebase != null && userFirebase.EmailVerified)
            {
                CurrentUser = userFirebase;
                return userFirebase;
            }
            else return null;
        }

        public async Task SetUserToken()
        {
            var jwtToken = await _jsRuntime.InvokeAsync<string?>("firebaseAuth.getUserToken");
            _memoryCache.Set("jwt", jwtToken);
        }

        public async Task<String?> GetUserToken()
        {
            // Usa el provider para resolver y cachear el token
            return await _memoryCache.GetOrCreateAsync("jwt", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(55);
                var token = await _jsRuntime.InvokeAsync<string?>("firebaseAuth.getUserToken");
                return token;
            });
        }

        public async Task<bool> IsUserAuthenticated()
        {
            var user = await GetUserFirebase();
            if (user != null)
            {
                await SetUserToken();
                OnChangeLogin?.Invoke();
            }
            return user != null;
        }

        public async Task<FirebaseUser?> LoginWithGoogle()
        {
            var userFirebase = await _jsRuntime.InvokeAsync<FirebaseUser>("firebaseAuth.loginWithGoogle");
            CurrentUser = userFirebase;
            OnChangeLogin?.Invoke();
            return userFirebase;
        }

        public async Task<bool> RecoveryPassword (string email)
        {
            return await _jsRuntime.InvokeAsync<bool>("firebaseAuth.recoveryPassword", email);

        }
    }
}