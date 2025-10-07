using AppMovil.ViewModels;

namespace AppMovil.Pages;

public partial class CreateReservaPage : ContentPage
{
    public CreateReservaPage()
    {
        InitializeComponent();
    }

    public CreateReservaPage(CreateReservaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}