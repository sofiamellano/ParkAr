using AppMovil.Pages;
using AppMovil.ViewModels;
using Service.Services;

namespace AppMovil;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}
