using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Service.Models;
using Service.Services;
using Service.Enums;

namespace AppMovil.ViewModels;

[QueryProperty(nameof(ReservaIdParam), "reservaId")]
public partial class TicketPageViewModel : BaseViewModel
{
    private readonly ReservaService _reservaService = new();

    [ObservableProperty]
    private string reservaIdParam = string.Empty;

    [ObservableProperty]
    private int reservaId;

    [ObservableProperty]
    private Reserva? reserva;

    [ObservableProperty]
    private string lugarReserva = string.Empty;

    [ObservableProperty]
    private DateTime fechaReserva = DateTime.Now;

    [ObservableProperty]
    private string horaInicio = string.Empty;

    [ObservableProperty]
    private string horaFin = string.Empty;

    [ObservableProperty]
    private string vehiculoInfo = string.Empty;

    [ObservableProperty]
    private string estadoReserva = string.Empty;

    [ObservableProperty]
    private string numeroTicket = string.Empty;

    [ObservableProperty]
    private bool isTicketValid = true;

    public TicketPageViewModel()
    {
        Title = "Ticket Digital";
    }

    public override async Task OnAppearingAsync()
    {
        await LoadTicketData();
        await base.OnAppearingAsync();
    }

    partial void OnReservaIdParamChanged(string value)
    {
        if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var id))
        {
            ReservaId = id;
            _ = LoadTicketData();
        }
    }

    private async Task LoadTicketData()
    {
        if (ReservaId <= 0) return;

        try
        {
            IsBusy = true;

            System.Diagnostics.Debug.WriteLine($"[DEBUG] Cargando ticket para reserva ID: {ReservaId}");

            // Cargar datos reales de la reserva
            Reserva = await _reservaService.GetByIdAsync(ReservaId);

            if (Reserva == null)
            {
                await Shell.Current.DisplayAlert("Error", "No se encontró la reserva", "OK");
                await Shell.Current.GoToAsync("//ReservasPage");
                return;
            }

            // Asignar datos a las propiedades de forma segura
            LugarReserva = Reserva.Lugar != null ? $"Plaza {Reserva.Lugar.Numero}" : "Plaza N/A";
            FechaReserva = Reserva.FechaInicio;
            HoraInicio = Reserva.FechaInicio.ToString("HH:mm");
            HoraFin = Reserva.FechaFin.ToString("HH:mm");
            
            // Manejar información del vehículo de forma segura
            if (Reserva.Vehiculo != null)
            {
                VehiculoInfo = $"{Reserva.Vehiculo.Patente} - {Reserva.Vehiculo.TipoVehiculo}";
            }
            else
            {
                VehiculoInfo = "Vehículo N/A";
            }
            
            EstadoReserva = Reserva.EstadoReserva.ToString();
            
            // Generar número de ticket
            NumeroTicket = $"TK-{Reserva.FechaInicio:yyyyMMdd}-{Reserva.Id:D4}";

            // Determinar si el ticket es válido
            IsTicketValid = Reserva.EstadoReserva == EstadoReservaEnum.Activa && 
                           Reserva.FechaFin > DateTime.Now;

            System.Diagnostics.Debug.WriteLine($"[DEBUG] Ticket cargado: {NumeroTicket}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error al cargar ticket: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", $"No se pudo cargar el ticket: {ex.Message}", "OK");
            
            // En caso de error, volver a reservas
            await Shell.Current.GoToAsync("//ReservasPage");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshTicket()
    {
        await LoadTicketData();
    }

    [RelayCommand]
    private async Task GoBack()
    {
        // ✅ NAVEGAR DE VUELTA A LA PÁGINA DE RESERVAS
        await Shell.Current.GoToAsync("//ReservasPage");
    }
}