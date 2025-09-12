using CommunityToolkit.Mvvm.ComponentModel;

namespace AppMovil.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    public virtual async Task OnAppearingAsync()
    {
        await Task.CompletedTask;
    }

    public virtual async Task OnDisappearingAsync()
    {
        await Task.CompletedTask;
    }
}