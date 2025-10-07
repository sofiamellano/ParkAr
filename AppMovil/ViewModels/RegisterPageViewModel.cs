using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Service.Services;

namespace AppMovil.ViewModels;

public partial class RegisterPageViewModel : BaseViewModel
{
    private readonly AuthService _authService;

    [ObservableProperty]
    private string fullName = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string phoneNumber = string.Empty;

    public RegisterPageViewModel()
    {
        Title = "Registrarse";
        _authService = new AuthService();
    }

    [RelayCommand]
    private async Task Register()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || 
            string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Por favor complete todos los campos obligatorios", "OK");
            return;
        }

        if (Password != ConfirmPassword)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            // Usar el método del AuthService para crear el usuario
            bool registrationResult = await _authService.CreateUserWithEmailAndPasswordAsync(Email, Password, FullName);

            if (registrationResult)
            {
                await Application.Current.MainPage.DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
                
                // Limpiar los campos
                FullName = string.Empty;
                Email = string.Empty;
                Password = string.Empty;
                ConfirmPassword = string.Empty;
                PhoneNumber = string.Empty;
                
                // Volver a la página de login
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar el usuario. Verifique los datos e intente nuevamente.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Error al registrar usuario: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToLogin()
    {
        await Application.Current.MainPage.Navigation.PopAsync();
    }
}