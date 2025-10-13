using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Service.Enums;
using Service.Models;
using Service.Services;
using System.Collections.ObjectModel;

namespace AppMovil.ViewModels;

public partial class HistorialPageViewModel : BaseViewModel
{
    private readonly ReservaService _reservaService = new();
    private int _idUserLogin;

    [ObservableProperty]
    private string mensajeVacio = "No tienes reservas en el historial";

    [ObservableProperty]
    private ObservableCollection<Reserva> historialReservas = new();

    public bool TieneHistorial => HistorialReservas?.Count > 0;

    public HistorialPageViewModel()
    {
        Title = "Historial";
        _idUserLogin = Preferences.Get("UserLoginId", 0);
        _ = InicializeAsync();
    }

    private async Task InicializeAsync()
    {
        await OnGetAll();
    }

    private async Task OnGetAll()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            // Verificar si el usuario está logueado
            if (_idUserLogin <= 0)
            {
                MensajeVacio = "Debes iniciar sesión para ver tu historial";
                HistorialReservas = new ObservableCollection<Reserva>();
                OnPropertyChanged(nameof(TieneHistorial));
                return;
            }

            System.Diagnostics.Debug.WriteLine($"[DEBUG] Cargando historial para UserLoginId: {_idUserLogin}");

            // Obtener todas las reservas del usuario
            var todasLasReservas = await _reservaService.GetByUsuarioAsync(_idUserLogin);

            // Filtrar solo reservas no activas (canceladas, finalizadas, o activas que ya terminaron)
            var reservasHistorial = todasLasReservas?.Where(r => 
                !r.IsDeleted && 
                (r.EstadoReserva == EstadoReservaEnum.Cancelada || 
                 r.EstadoReserva == EstadoReservaEnum.Finalizada ||
                 (r.EstadoReserva == EstadoReservaEnum.Activa && r.FechaFin <= DateTime.Now))
            ).OrderByDescending(r => r.FechaInicio) // Ordenar por fecha más reciente primero
            .ToList() ?? new List<Reserva>();

            System.Diagnostics.Debug.WriteLine($"[DEBUG] Reservas en historial encontradas: {reservasHistorial.Count}");

            HistorialReservas = new ObservableCollection<Reserva>(reservasHistorial);
            OnPropertyChanged(nameof(TieneHistorial));

            // Actualizar mensaje si no hay historial
            if (!TieneHistorial)
            {
                MensajeVacio = "No tienes reservas finalizadas o canceladas";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error al cargar historial: {ex.Message}");
            MensajeVacio = "Error al cargar el historial";
            HistorialReservas = new ObservableCollection<Reserva>();
            OnPropertyChanged(nameof(TieneHistorial));
            
            // Solo mostrar alert para errores graves, no para casos normales
            if (!ex.Message.Contains("NotFound") && !ex.Message.Contains("BadRequest"))
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo cargar el historial: {ex.Message}", "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ViewDetails(Reserva reserva)
    {
        if (reserva == null) return;

        try
        {
            // Calcular duración
            var duracion = reserva.FechaFin - reserva.FechaInicio;
            var duracionTexto = $"{duracion.Hours}h {duracion.Minutes}m";

            // Obtener información de manera segura
            var lugarTexto = reserva.Lugar?.ToString() ?? $"Plaza {reserva.LugarId}";
            var vehiculoTexto = reserva.Vehiculo?.ToString() ?? $"Vehículo ID {reserva.VehiculoId}";

            // Determinar estado y color para mostrar
            string estadoTexto, estadoIcono, estadoColor;
            
            if (reserva.EstadoReserva == EstadoReservaEnum.Cancelada)
            {
                estadoTexto = "CANCELADA";
                estadoIcono = "❌";
                estadoColor = "#E74C3C"; // Rojo
            }
            else if (reserva.EstadoReserva == EstadoReservaEnum.Finalizada)
            {
                estadoTexto = "FINALIZADA";
                estadoIcono = "✅";
                estadoColor = "#27AE60"; // Verde
            }
            else if (reserva.EstadoReserva == EstadoReservaEnum.Activa && reserva.FechaFin <= DateTime.Now)
            {
                estadoTexto = "COMPLETADA";
                estadoIcono = "✅";
                estadoColor = "#27AE60"; // Verde
            }
            else
            {
                estadoTexto = "FINALIZADA";
                estadoIcono = "✅";
                estadoColor = "#27AE60"; // Verde
            }

            // Crear mensaje con formato mejorado
            var mensaje = $"🎫 DETALLES DE LA RESERVA\n\n" +
                         $"🅿️ LUGAR\n" +
                         $"   {lugarTexto}\n\n" +
                         $"🚗 VEHÍCULO\n" +
                         $"   {vehiculoTexto}\n\n" +
                         $"📅 FECHA Y HORARIO\n" +
                         $"   🕐 Inicio: {reserva.FechaInicio:dd/MM/yyyy HH:mm}\n" +
                         $"   🕔 Fin: {reserva.FechaFin:dd/MM/yyyy HH:mm}\n" +
                         $"   ⏱️ Duración: {duracionTexto}\n\n" +
                         $"📊 ESTADO\n" +
                         $"   {estadoIcono} {estadoTexto}\n\n" +
                         $"📋 INFORMACIÓN ADICIONAL\n" +
                         $"   🆔 ID Reserva: {reserva.Id}\n" +
                         $"   📝 Creada: {reserva.FechaInicio:dd/MM/yyyy}";

            // Mostrar modal con botones personalizados según el estado
            if (reserva.EstadoReserva == EstadoReservaEnum.Cancelada)
            {
                await Shell.Current.DisplayAlert("📋 Reserva Cancelada", mensaje, "Entendido");
            }
            else
            {
                var result = await Shell.Current.DisplayAlert("📋 Detalles de Reserva", mensaje, "Ver Ticket", "Cerrar");
                
                if (result)
                {
                    // Si el usuario quiere ver el ticket, navegar al TicketPage
                    await Shell.Current.GoToAsync($"///TicketPage?reservaId={reserva.Id}");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error al mostrar detalles: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "No se pudieron cargar los detalles de la reserva", "OK");
        }
    }

    [RelayCommand]
    private async Task RefreshHistorial()
    {
        await OnGetAll();
    }

    public override async Task OnAppearingAsync()
    {
        // Recargar historial cuando la página aparece
        await OnGetAll();
        await base.OnAppearingAsync();
    }
}