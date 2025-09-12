using AppMovil.Pages;
using AppMovil.ViewModels;
using Service.Services;

namespace AppMovil;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Iniciar con la página de Login
        var authService = new AuthService();
        var loginViewModel = new LoginPageViewModel(authService);
        var loginPage = new LoginPage(loginViewModel);
        
        MainPage = new NavigationPage(loginPage);
    }
}
