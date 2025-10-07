using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Service.Enums;
using Service.Models;
using Service.Services;
using System.Collections.ObjectModel;

namespace AppMovil.ViewModels;

public partial class ReservasPageViewModel : BaseViewModel
{
    ReservaService _reservaService = new();

    [ObservableProperty]
    private string mensajeVacio = "No tienes reservas activas";

    [ObservableProperty]
    private ObservableCollection<Reserva> reservas = new();

    public bool TieneReservas => Reservas.Count > 0;

    public IRelayCommand GetAllCommand { get; }

    private int _idUserLogin;
    private System.Timers.Timer? _updateTimer;

    public ReservasPageViewModel()
    {
        Title = "Mis Reservas";
        GetAllCommand = new RelayCommand(OnGetAll);
        _idUserLogin = Preferences.Get("UserLoginId", 0);
        
        // ✅ CONFIGURAR TIMER PARA AUTO-ACTUALIZACIÓN
        SetupAutoRefreshTimer();
        
        _ = InicializeAsync();
    }

    private void SetupAutoRefreshTimer()
    {
        // Timer que se ejecuta cada 60 segundos para verificar reservas expiradas
        _updateTimer = new System.Timers.Timer(60000); // 60 segundos
        _updateTimer.Elapsed += async (sender, e) =>
        {
            // Verificar si hay reservas que hayan expirado
            await CheckAndRemoveExpiredReservations();
        };
        _updateTimer.AutoReset = true;
        _updateTimer.Enabled = true;
    }

    private async Task CheckAndRemoveExpiredReservations()
    {
        try
        {
            var now = DateTime.Now;
            var reservasExpiradas = Reservas.Where(r => r.FechaFin <= now).ToList();
            
            if (reservasExpiradas.Any())
            {
                // Remover reservas expiradas de la UI
                foreach (var reservaExpirada in reservasExpiradas)
                {
                    Reservas.Remove(reservaExpirada);
                    System.Diagnostics.Debug.WriteLine($"[AUTO-CLEANUP] Reserva expirada removida: Plaza {reservaExpirada.Lugar?.Numero}, Fin: {reservaExpirada.FechaFin}");
                }
                
                OnPropertyChanged(nameof(TieneReservas));
                
                // Actualizar mensaje si no quedan reservas
                if (!Reservas.Any())
                {
                    MensajeVacio = "Aún no tienes reservas activas. ¡Crea tu primera reserva!";
                    OnPropertyChanged(nameof(MensajeVacio));
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERROR] Auto-cleanup failed: {ex.Message}");
        }
    }

    private async Task InicializeAsync()
    {
        OnGetAll();
    }

    private async void OnGetAll()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            
            // Verificar si el usuario está logueado
            if (_idUserLogin <= 0)
            {
                MensajeVacio = "Debes iniciar sesión para ver tus reservas";
                reservas = new ObservableCollection<Reserva>();
                OnPropertyChanged(nameof(Reservas));
                OnPropertyChanged(nameof(TieneReservas));
                return;
            }

            // DEBUG: Mostrar el ID que se está usando
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Buscando reservas para UserLoginId: {_idUserLogin}");

            var reservasList = await _reservaService.GetByUsuarioAsync(_idUserLogin);
            
            // ✅ FILTRAR SOLO RESERVAS ACTIVAS Y FUTURAS
            var reservasActivas = reservasList?.Where(r => 
                r.EstadoReserva == EstadoReservaEnum.Activa && 
                !r.IsDeleted &&
                r.FechaFin > DateTime.Now // Solo reservas que no han terminado
            ).OrderBy(r => r.FechaInicio) // Ordenar por fecha de inicio
            .ToList() ?? new List<Reserva>();
            
            // DEBUG: Mostrar cuántas reservas se encontraron después del filtro
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Total reservas obtenidas: {reservasList?.Count ?? 0}");
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Reservas activas y futuras: {reservasActivas.Count}");
            
            reservas = new ObservableCollection<Reserva>(reservasActivas);
            
            // Actualizar mensaje si no hay reservas
            if (!reservas.Any())
            {
                MensajeVacio = "Aún no tienes reservas activas. ¡Crea tu primera reserva!";
            }
            
            OnPropertyChanged(nameof(Reservas));
            OnPropertyChanged(nameof(TieneReservas));
        }
        catch (ArgumentException ex)
        {
            // Error de validación (usuario no logueado)
            System.Diagnostics.Debug.WriteLine($"[DEBUG] ArgumentException: {ex.Message}");
            MensajeVacio = "Debes iniciar sesión para ver tus reservas";
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            // DEBUG: Mostrar el error completo
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Exception: {ex.Message}");
            
            // Para usuarios de Firebase, es normal que no existan reservas inicialmente
            // No mostrar error si es porque el usuario no tiene reservas (común para usuarios nuevos)
            if (ex.Message.Contains("NotFound") || ex.Message.Contains("BadRequest"))
            {
                MensajeVacio = "Aún no tienes reservas activas. ¡Crea tu primera reserva!";
                reservas = new ObservableCollection<Reserva>();
                OnPropertyChanged(nameof(Reservas));
                OnPropertyChanged(nameof(TieneReservas));
            }
            else
            {
                // Solo mostrar errores graves (problemas de conectividad, etc.)
                MensajeVacio = "Error al cargar las reservas";
                await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar las reservas: {ex.Message}", "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CreateReserva()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] Navegando a CreateReservaPage...");
            
            // Navegar a la página de crear reserva usando navegación absoluta
            await Shell.Current.GoToAsync("//CreateReservaPage");
            
            System.Diagnostics.Debug.WriteLine("[DEBUG] Navegación exitosa");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error en navegación: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", $"Error al navegar: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task EditReserva(Reserva reserva)
    {
        if (reserva == null) return;
        
        try
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Navegando a editar reserva ID: {reserva.Id}");
            
            // Navegar a CreateReservaPage con el ID de la reserva para editar
            await Shell.Current.GoToAsync($"//CreateReservaPage?reservaId={reserva.Id}&modo=editar");
            
            System.Diagnostics.Debug.WriteLine("[DEBUG] Navegación a editar exitosa");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error al navegar a editar: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", $"Error al abrir la reserva para editar: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task CancelReserva(Reserva reserva)
    {
        if (reserva == null) return;

        var lugarTexto = reserva.Lugar?.ToString() ?? "Plaza N/A";
        var result = await Shell.Current.DisplayAlert("Cancelar Reserva", 
            $"¿Está seguro de cancelar la reserva en {lugarTexto}?", "Sí", "No");

        if (result)
        {
            try
            {
                IsBusy = true;
                
                // Cambiar estado a Cancelada
                reserva.EstadoReserva = EstadoReservaEnum.Cancelada;
                var success = await _reservaService.UpdateAsync(reserva);
                
                if (success)
                {
                    // ✅ REMOVER DE LA LISTA INMEDIATAMENTE
                    Reservas.Remove(reserva);
                    OnPropertyChanged(nameof(TieneReservas));
                    
                    await Shell.Current.DisplayAlert("✅ Reserva Cancelada", 
                        $"La reserva en {lugarTexto} ha sido cancelada exitosamente", "OK");
                    
                    // ✅ RECARGAR TODAS LAS RESERVAS PARA ASEGURAR CONSISTENCIA
                    OnGetAll();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo cancelar la reserva", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cancelar: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    [RelayCommand]
    private async Task ViewTicket(Reserva reserva)
    {
        if (reserva == null) return;
        await Shell.Current.GoToAsync($"TicketPage?reservaId={reserva.Id}");
    }

    public override async Task OnAppearingAsync()
    {
        // Refrescar el ID del usuario en caso de que haya cambiado
        _idUserLogin = Preferences.Get("UserLoginId", 0);
        System.Diagnostics.Debug.WriteLine($"[DEBUG] OnAppearingAsync - UserLoginId actualizado: {_idUserLogin}");
        
        // Siempre recargar las reservas cuando la página aparece (incluso después de crear una nueva)
        OnGetAll();
        
        await base.OnAppearingAsync();
    }

    public override async Task OnDisappearingAsync()
    {
        // Limpiar timer cuando la página desaparece
        _updateTimer?.Stop();
        await base.OnDisappearingAsync();
    }

    // Método para limpiar recursos
    protected virtual void Dispose()
    {
        _updateTimer?.Dispose();
    }
}

