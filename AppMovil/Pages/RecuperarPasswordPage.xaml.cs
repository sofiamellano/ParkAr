using AppMovil.ViewModels;

namespace AppMovil.Pages;

public partial class RecuperarPasswordPage : ContentPage
{
    public RecuperarPasswordPage(RecuperarPasswordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    public RecuperarPasswordPage()
    {
        InitializeComponent();
        BindingContext = new RecuperarPasswordViewModel();
    }
}