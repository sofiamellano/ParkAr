using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppMovil.ViewModels;

public partial class TicketPageViewModel : BaseViewModel
{
    [ObservableProperty]
    private int reservaId;

    [ObservableProperty]
    private string lugarReserva = string.Empty;

    [ObservableProperty]
    private DateTime fechaReserva;

    [ObservableProperty]
    private string horaReserva = string.Empty;

    [ObservableProperty]
    private string qrCodeData = string.Empty;

    [ObservableProperty]
    private string numeroTicket = string.Empty;

    public TicketPageViewModel()
    {
        Title = "Ticket Digital";
    }

    public override async Task OnAppearingAsync()
    {
        LoadTicketData();
        await base.OnAppearingAsync();
    }

    private void LoadTicketData()
    {
        // Simular datos del ticket
        ReservaId = 1; // Esto vendría de los parámetros de navegación
        LugarReserva = "Parking Centro - Plaza 12";
        FechaReserva = DateTime.Now.AddDays(1);
        HoraReserva = "14:00";
        NumeroTicket = $"TK-{DateTime.Now:yyyyMMdd}-{ReservaId:D4}";
        
        // Generar datos del QR (normalmente sería un código único)
        QrCodeData = $"PARKAR-{NumeroTicket}-{ReservaId}";
    }

    [RelayCommand]
    private async Task ShareTicket()
    {
        try
        {
            var ticketInfo = $"Ticket ParkAR\n" +
                           $"Número: {NumeroTicket}\n" +
                           $"Lugar: {LugarReserva}\n" +
                           $"Fecha: {FechaReserva:dd/MM/yyyy}\n" +
                           $"Hora: {HoraReserva}";

            await Share.RequestAsync(new ShareTextRequest
            {
                Text = ticketInfo,
                Title = "Compartir Ticket"
            });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo compartir el ticket: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task SaveTicket()
    {
        await Shell.Current.DisplayAlert("Guardar", "Ticket guardado en galería", "OK");
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}