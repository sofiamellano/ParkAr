using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppMovil.ViewModels;

public partial class RegisterPageViewModel : BaseViewModel
{
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

            // Simular registro
            await Task.Delay(1000);

            await Application.Current.MainPage.DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
            await Application.Current.MainPage.Navigation.PopAsync();
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