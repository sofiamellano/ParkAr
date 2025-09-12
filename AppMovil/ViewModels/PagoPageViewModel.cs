using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppMovil.ViewModels;

public partial class PagoPageViewModel : BaseViewModel
{
    [ObservableProperty]
    private string numeroTarjeta = string.Empty;

    [ObservableProperty]
    private string nombreTitular = string.Empty;

    [ObservableProperty]
    private string fechaVencimiento = string.Empty;

    [ObservableProperty]
    private string cvv = string.Empty;

    [ObservableProperty]
    private decimal montoAPagar;

    [ObservableProperty]
    private string descripcionPago = string.Empty;

    public PagoPageViewModel()
    {
        Title = "Realizar Pago";
    }

    [RelayCommand]
    private async Task ProcessPayment()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(NumeroTarjeta) || string.IsNullOrWhiteSpace(NombreTitular) ||
            string.IsNullOrWhiteSpace(FechaVencimiento) || string.IsNullOrWhiteSpace(Cvv))
        {
            await Shell.Current.DisplayAlert("Error", "Por favor complete todos los campos", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            // Simular procesamiento de pago
            await Task.Delay(2000);

            await Shell.Current.DisplayAlert("Pago Exitoso", 
                $"El pago de ${MontoAPagar:N2} ha sido procesado correctamente", "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al procesar el pago: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}