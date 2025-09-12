using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppMovil.ViewModels;

public partial class ReservasPageViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<ReservaItem> reservas = new();

    [ObservableProperty]
    private bool hasReservas;

    public ReservasPageViewModel()
    {
        Title = "Mis Reservas";
        LoadReservas();
    }

    private void LoadReservas()
    {
        // Simular datos de reservas
        Reservas = new ObservableCollection<ReservaItem>
        {
            new ReservaItem 
            { 
                Id = 1, 
                Lugar = "Parking Centro - Plaza 12",
                Fecha = DateTime.Now.AddDays(1),
                Hora = "14:00",
                Estado = "Confirmada",
                Precio = "$500"
            },
            new ReservaItem 
            { 
                Id = 2, 
                Lugar = "Parking Norte - Plaza 5",
                Fecha = DateTime.Now.AddDays(2),
                Hora = "10:30",
                Estado = "Pendiente",
                Precio = "$300"
            }
        };
        HasReservas = Reservas.Count > 0;
    }

    [RelayCommand]
    private async Task CreateReserva()
    {
        await Shell.Current.DisplayAlert("Nueva Reserva", "Función para crear nueva reserva", "OK");
    }

    [RelayCommand]
    private async Task EditReserva(ReservaItem reserva)
    {
        if (reserva == null) return;
        await Shell.Current.DisplayAlert("Modificar Reserva", $"Modificar reserva: {reserva.Lugar}", "OK");
    }

    [RelayCommand]
    private async Task CancelReserva(ReservaItem reserva)
    {
        if (reserva == null) return;

        var result = await Shell.Current.DisplayAlert("Cancelar Reserva", 
            $"¿Está seguro de cancelar la reserva en {reserva.Lugar}?", "Sí", "No");

        if (result)
        {
            Reservas.Remove(reserva);
            HasReservas = Reservas.Count > 0;
            await Shell.Current.DisplayAlert("Reserva Cancelada", "La reserva ha sido cancelada", "OK");
        }
    }

    [RelayCommand]
    private async Task ViewTicket(ReservaItem reserva)
    {
        if (reserva == null) return;
        await Shell.Current.GoToAsync($"TicketPage?reservaId={reserva.Id}");
    }

    public override async Task OnAppearingAsync()
    {
        LoadReservas();
        await base.OnAppearingAsync();
    }
}

public class ReservaItem
{
    public int Id { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Hora { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Precio { get; set; } = string.Empty;
}