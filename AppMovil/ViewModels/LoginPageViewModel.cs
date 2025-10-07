using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Service.DTOs;
using Service.Interfaces;
using Service.Services;

namespace AppMovil.ViewModels;

public partial class LoginPageViewModel : BaseViewModel
{
    AuthService _authService;

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
    public IRelayCommand GoToRecuperarPasswordCommand { get; }

    public LoginPageViewModel()
    {
        _authService = new AuthService();
        Title = "Iniciar Sesión";
        LoginCommand = new AsyncRelayCommand(OnLogin, CanLogin);
        GoToRegisterCommand = new AsyncRelayCommand(GoToRegister);
        GoToRecuperarPasswordCommand = new AsyncRelayCommand(GoToRecuperarPassword);
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
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            IsLoginEnabled = false;

            var loginDto = new LoginDTO
            {
                Username = Email,
                Password = Password
            };

            var response = await _authService.Login(loginDto);

            // Si response es null = login exitoso
            if (response == null)
            {
                // ✅ LOGIN EXITOSO - Usar ID que coincida con tu BD
                // Para pruebas, usar ID 1 o 2 que existen en tu BD
                var userId = 1; // O puedes usar lógica basada en el email
                
                // Opcional: Lógica para diferentes usuarios
                if (Email.Contains("usuario2") || Email.Contains("test2"))
                {
                    userId = 2;
                }
                
                // Guardar datos del usuario en preferencias
                Preferences.Set("UserLoginId", userId);
                Preferences.Set("UserEmail", Email);

                // ✅ Usar siempre el AppShell actual
                if (Application.Current?.MainPage is AppShell shell)
                {
                    shell.SetLoginState(true);
                }

                // ✅ Navegar a la pestaña inicial
                await Shell.Current.GoToAsync($"//ReservasPage");

                // Limpiar campos después del login exitoso
                Email = string.Empty;
                Password = string.Empty;
            }
            else
            {
                // Mostrar el error específico devuelto por el servidor
                await Application.Current.MainPage.DisplayAlert("Error de Login", response, "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error de Login", $"Error al iniciar sesión: {ex.Message}", "OK");
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

    private async Task GoToRecuperarPassword()
    {
        try
        {
            await Shell.Current.GoToAsync("//RecuperarPasswordPage");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", 
                $"Error al navegar: {ex.Message}", "OK");
        }
    }
}