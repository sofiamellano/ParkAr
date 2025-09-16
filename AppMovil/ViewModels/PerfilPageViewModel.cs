using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppMovil.ViewModels;

public partial class PerfilPageViewModel : BaseViewModel
{
    [ObservableProperty]
    private string nombreUsuario = "Usuario Demo";

    [ObservableProperty]
    private string emailUsuario = "usuario@demo.com";

    [ObservableProperty]
    private string telefonoUsuario = "+54 9 11 1234-5678";

    [ObservableProperty]
    private bool isLoggedIn = true; // Cambiar a true ya que llegamos aquí después del login

    public PerfilPageViewModel()
    {
        Title = "Perfil";
    }

    [RelayCommand]
    private async Task GoToLogin()
    {
        // Navegar de vuelta a Login (cerrar sesión)
        var authService = new Service.Services.AuthService();
        var loginViewModel = new LoginPageViewModel();
        var loginPage = new Pages.LoginPage(loginViewModel);
        
        Application.Current.MainPage = new NavigationPage(loginPage);
    }

    [RelayCommand]
    private async Task GoToRegister()
    {
        var registerViewModel = new RegisterPageViewModel();
        var registerPage = new Pages.RegisterPage(registerViewModel);
        await Shell.Current.GoToAsync("RegisterPage");
    }

    [RelayCommand]
    private async Task GoToTicket()
    {
        if (!IsLoggedIn)
        {
            await Shell.Current.DisplayAlert("Acceso Denegado", "Debe iniciar sesión para ver sus tickets", "OK");
            return;
        }
        await Shell.Current.GoToAsync("TicketPage");
    }

    [RelayCommand]
    private async Task EditProfile()
    {
        if (!IsLoggedIn)
        {
            await Shell.Current.DisplayAlert("Acceso Denegado", "Debe iniciar sesión para editar su perfil", "OK");
            return;
        }
        await Shell.Current.DisplayAlert("Editar Perfil", "Función para editar perfil", "OK");
    }

    [RelayCommand]
    private async Task ChangePassword()
    {
        if (!IsLoggedIn)
        {
            await Shell.Current.DisplayAlert("Acceso Denegado", "Debe iniciar sesión para cambiar su contraseña", "OK");
            return;
        }
        await Shell.Current.DisplayAlert("Cambiar Contraseña", "Función para cambiar contraseña", "OK");
    }

    [RelayCommand]
    private async Task Logout()
    {
        var result = await Shell.Current.DisplayAlert("Cerrar Sesión", 
            "¿Está seguro de que desea cerrar sesión?", "Sí", "No");

        if (result)
        {
            IsLoggedIn = false;
            await Shell.Current.DisplayAlert("Sesión Cerrada", "Ha cerrado sesión correctamente", "OK");
            
            // Regresar a LoginPage
            var authService = new Service.Services.AuthService();
            var loginViewModel = new LoginPageViewModel();
            var loginPage = new Pages.LoginPage(loginViewModel);
            
            Application.Current.MainPage = new NavigationPage(loginPage);
        }
    }

    [RelayCommand]
    private async Task ShowSettings()
    {
        await Shell.Current.DisplayAlert("Configuración", "Función de configuración", "OK");
    }

    [RelayCommand]
    private async Task ShowHelp()
    {
        await Shell.Current.DisplayAlert("Ayuda", "Función de ayuda y soporte", "OK");
    }

    public override async Task OnAppearingAsync()
    {
        // El usuario ya está logueado si llegó aquí a través del AppShell
        IsLoggedIn = true;
        await base.OnAppearingAsync();
    }
}