using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Service.DTOs;
using Service.Interfaces;
using Service.Services;
using Service.Models;

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
    public IRelayCommand GoToRecuperarPasswordCommand { get; }

    public LoginPageViewModel()
    {
        _authService = new AuthService();
        _usuarioService = new UsuarioService();
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

            // Si response es null = login exitoso en Firebase
            if (response == null)
            {
                Usuario? usuario = null;
                
                try
                {
                    // ✅ Buscar el usuario en la base de datos por email
                    usuario = await _usuarioService.GetByEmailAsync(Email);
                }
                catch (Exception ex)
                {
                    // Si el error contiene "NotFound", el usuario no existe en la BD
                    if (ex.Message.Contains("NotFound") || ex.Message.Contains("404"))
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "Usuario no registrado", 
                            $"El correo '{Email}' no está registrado en la base de datos.\n\nPor favor, contacta al administrador para completar tu registro.", 
                            "OK");
                        return;
                    }
                    
                    // Otro tipo de error
                    throw;
                }
                
                if (usuario == null)
                {
                    // Usuario no existe en la BD MySQL
                    await Application.Current.MainPage.DisplayAlert(
                        "Usuario no registrado", 
                        $"El correo '{Email}' no está registrado en la base de datos.\n\nPor favor, contacta al administrador para completar tu registro.", 
                        "OK");
                    return;
                }

                // ✅ Usuario válido - guardar datos reales
                Preferences.Set("UserLoginId", usuario.Id);
                Preferences.Set("UserEmail", usuario.Email);

                // ✅ Usar siempre el AppShell actual
                if (Application.Current?.MainPage is AppShell shell)
                {
                    shell.SetLoginState(true);
                }

                // ✅ Navegar a la pestaña inicial
                await Shell.Current.GoToAsync($"//PerfilPage");

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