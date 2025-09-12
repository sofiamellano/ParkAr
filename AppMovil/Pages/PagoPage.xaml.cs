using AppMovil.ViewModels;

namespace AppMovil.Pages;

public partial class PagoPage : ContentPage
{
    public PagoPage(PagoPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is BaseViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is BaseViewModel vm)
        {
            await vm.OnDisappearingAsync();
        }
    }
}