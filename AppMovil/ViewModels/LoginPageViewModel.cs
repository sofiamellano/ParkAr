using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Service.DTOs;
using Service.Interfaces;

namespace AppMovil.ViewModels;

public partial class LoginPageViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isLoginEnabled = true;

    public LoginPageViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Iniciar Sesión";
    }

    [RelayCommand]
    private async Task Login()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Por favor ingrese email y contraseña", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            IsLoginEnabled = false;

            // Por ahora permite ingresar con cualquier cosa (sin validación real)
            var loginDto = new LoginDTO
            {
                Username = Email,
                Password = Password
            };

            // Simular login exitoso
            await Task.Delay(1000);

            // Cambiar a AppShell después del login exitoso
            Application.Current.MainPage = new AppShell();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Error al iniciar sesión: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            IsLoginEnabled = true;
        }
    }

    [RelayCommand]
    private async Task GoToRegister()
    {
        var registerViewModel = new RegisterPageViewModel();
        var registerPage = new Pages.RegisterPage(registerViewModel);
        await Application.Current.MainPage.Navigation.PushAsync(registerPage);
    }
}