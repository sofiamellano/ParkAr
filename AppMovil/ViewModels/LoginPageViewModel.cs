using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Service.DTOs;
using Service.Interfaces;
using Service.Services;

namespace AppMovil.ViewModels;

public partial class LoginPageViewModel : BaseViewModel
{
    AuthService _authService;
    UsuarioService _usuarioService;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string email = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isLoginEnabled = true;

    public IRelayCommand LoginCommand { get; }
    public IRelayCommand GoToRegisterCommand { get; }

    public LoginPageViewModel()
    {
        _authService = new AuthService();
        _usuarioService = new UsuarioService();
        Title = "Iniciar Sesión";
        LoginCommand = new AsyncRelayCommand(OnLogin, CanLogin);
        GoToRegisterCommand = new AsyncRelayCommand(GoToRegister);
    }
    
    private bool CanLogin()
    {
        return !IsBusy && 
               !string.IsNullOrWhiteSpace(Email) && 
               !string.IsNullOrWhiteSpace(Password) && 
               IsLoginEnabled;
    }

    private async Task OnLogin()
    {
        try
        {
            IsBusy = true;
            IsLoginEnabled = false;

            var loginDto = new LoginDTO
            {
                Username = Email,
                Password = Password
            };

            var loginResult = await _authService.Login(loginDto);

            if (loginResult)
            {
                var usuario = await _usuarioService.GetByEmailAsync(Email);

                // ✅ Guardar el ID del usuario en las preferencias
                if (usuario != null)
                {
                    Preferences.Set("UserLoginId", usuario.Id);
                }

                // ✅ Usar siempre el AppShell actual
                if (Application.Current?.MainPage is AppShell shell)
                {
                    if (usuario != null)
                        shell.SetUserLogin(usuario);
                    else
                        shell.SetLoginState(true);
                }

                // ✅ Navegar a la pestaña inicial (por ejemplo ReservasPage)
                await Shell.Current.GoToAsync($"//ReservasPage");

                // Limpiar campos después del login exitoso
                Email = string.Empty;
                Password = string.Empty;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "Credenciales incorrectas. Verifique su email y contraseña.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error",
                $"Error al iniciar sesión: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            IsLoginEnabled = true;
        }
    }

    private async Task GoToRegister()
    {
        try
        {
            var registerViewModel = new RegisterPageViewModel();
            var registerPage = new Pages.RegisterPage(registerViewModel);
            await Application.Current.MainPage.Navigation.PushAsync(registerPage);
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", 
                $"Error al navegar: {ex.Message}", "OK");
        }
    }
}