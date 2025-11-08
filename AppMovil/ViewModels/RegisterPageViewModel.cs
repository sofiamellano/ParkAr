using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Service.Enums;
using Service.ExtentionMethods;
using Service.Models;
using Service.Services;

namespace AppMovil.ViewModels;

public partial class RegisterPageViewModel : ObservableObject
{
    AuthService _authService = new();
    UsuarioService _usuarioService = new();

    public IRelayCommand RegisterCommand { get; }
    public IRelayCommand VolverCommand { get; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string fullName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string email = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private TipoUsuarioEnum tipoUsuario = TipoUsuarioEnum.Cliente;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string password = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public RegisterPageViewModel()
    {
        RegisterCommand = new RelayCommand(Register, CanRegister);
        VolverCommand = new AsyncRelayCommand(OnVolver);
    }

    private bool CanRegister()
    {
        if (!IsBusy)
        {
            return !string.IsNullOrWhiteSpace(FullName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                   Password.Length >= 6;
        }
        return false;
    }

    private async void Register()
    {
        if (IsBusy) return;
        IsBusy = true;

        if (Password != ConfirmPassword)
        {
            await Application.Current.MainPage.DisplayAlert("Registrarse", "Las contraseñas ingresadas no coinciden", "Ok");
            IsBusy = false;
            return;
        }

        try
        {
            // Crear usuario en Firebase
            var user = await _authService.CreateUserWithEmailAndPasswordAsync(Email, Password, FullName);

            if (user == false)
            {
                await Application.Current.MainPage.DisplayAlert("Registrarse", "No se pudo crear el usuario en Firebase", "Ok");
                return;
            }
            else
            {
                // Crear usuario en la base de datos MySQL
                var newUser = new Usuario
                {
                    Nombre = FullName,
                    Email = Email,
                    Password = Password.GetHashSha256(),
                    TipoUsuario = TipoUsuario
                };

                var resultado = await _usuarioService.AddAsync(newUser);

                if (resultado != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Registrarse", "Cuenta creada exitosamente!", "Ok");

                    // Limpiar los campos
                    FullName = string.Empty;
                    Email = string.Empty;
                    Password = string.Empty;
                    ConfirmPassword = string.Empty;

                    // Navegar al login
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "El usuario se creó en Firebase pero no se pudo registrar en la base de datos. Contacte al administrador.", "Ok");
                }
            }
        }
        catch (FirebaseAuthException error)
        {
            await Application.Current.MainPage.DisplayAlert("Registrarse", "Ocurrió un problema: " + error.Reason, "Ok");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Error al registrar usuario: {ex.Message}", "Ok");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task OnVolver()
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}