using AppMovil.ViewModels;
using Microsoft.Maui.Controls;
using Service.Models;

namespace AppMovil
{
    public partial class AppShell : Shell
    {
        public AppShellViewModel ViewModel => (AppShellViewModel)BindingContext;

        public AppShell()
        {
            InitializeComponent();
            BindingContext = new AppShellViewModel();
        }

        // Método público para cambiar el estado de login desde otras páginas
        public void SetLoginState(bool isLoggedIn)
        {
            ViewModel.SetLoginState(isLoggedIn);
        }

        public void SetUserLogin(Usuario usuario)
        {
            ViewModel.SetUserLogin(usuario);
        }
    }
}
