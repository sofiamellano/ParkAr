using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Service.Models;

namespace AppMovil.ViewModels;

public partial class AppShellViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool isLoggedIn = false;

    [ObservableProperty]
    private Usuario? currentUser;

    [ObservableProperty]
    private string welcomeMessage = "Bienvenido";

    [ObservableProperty]
    private bool showUserInfo = false;

    public AppShellViewModel()
    {
        Title = "ParkAR Mobile";
    }

    public void SetLoginState(bool isLoggedIn)
    {
        IsLoggedIn = isLoggedIn;
        ShowUserInfo = isLoggedIn;
        
        if (!isLoggedIn)
        {
            CurrentUser = null;
            WelcomeMessage = "Bienvenido";
        }
    }

    public void SetUserLogin(Usuario usuario)
    {
        CurrentUser = usuario;
        IsLoggedIn = true;
        ShowUserInfo = true;
        WelcomeMessage = $"Hola, {usuario.Nombre}";
    }

    [RelayCommand]
    private async Task Logout()
    {
        var result = await Shell.Current.DisplayAlert("Cerrar Sesión", 
            "¿Está seguro de que desea cerrar sesión?", "Sí", "No");

        if (result)
        {
            SetLoginState(false);
            
            // Regresar a LoginPage
            var loginViewModel = new LoginPageViewModel();
            var loginPage = new Pages.LoginPage(loginViewModel);
            
            Application.Current.MainPage = new NavigationPage(loginPage);
        }
    }

    [RelayCommand]
    private async Task ShowProfile()
    {
        if (IsLoggedIn)
        {
            await Shell.Current.GoToAsync("//PerfilPage");
        }
    }
}