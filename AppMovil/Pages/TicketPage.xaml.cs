using AppMovil.ViewModels;

namespace AppMovil.Pages;

public partial class TicketPage : ContentPage
{
    public TicketPage(TicketPageViewModel viewModel)
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