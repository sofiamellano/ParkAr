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
    private string mensajeVacio = "No tienes reservas";

    [ObservableProperty]
    private ObservableCollection<Reserva> reservas = new();

    public bool TieneReservas => Reservas.Count > 0;

    public IRelayCommand GetAllCommand { get; }

    private int _idUserLogin;

    public ReservasPageViewModel()
    {
        Title = "Mis Reservas";
        GetAllCommand = new RelayCommand(OnGetAll);
        _idUserLogin = Preferences.Get("UserLoginId", 0);
        _ = InicializeAsync();
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
            
            // ✅ MOSTRAR TODAS LAS RESERVAS (activas, canceladas, finalizadas)
            // Solo filtrar las que no están eliminadas (IsDeleted = false)
            var todasLasReservas = reservasList?.Where(r => !r.IsDeleted)
                .OrderByDescending(r => r.FechaInicio) // Ordenar por fecha de inicio (más recientes primero)
                .ToList() ?? new List<Reserva>();
            
            // DEBUG: Mostrar cuántas reservas se encontraron
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Total reservas encontradas: {todasLasReservas.Count}");
            
            reservas = new ObservableCollection<Reserva>(todasLasReservas);
            
            // Actualizar mensaje si no hay reservas
            if (!reservas.Any())
            {
                MensajeVacio = "Aún no tienes reservas. ¡Crea tu primera reserva!";
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
            
            // Para usuarios nuevos, es normal que no existan reservas inicialmente
            if (ex.Message.Contains("NotFound") || ex.Message.Contains("BadRequest"))
            {
                MensajeVacio = "Aún no tienes reservas. ¡Crea tu primera reserva!";
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

        // ✅ VALIDAR QUE SOLO SE PUEDAN EDITAR RESERVAS ACTIVAS Y FUTURAS
        if (reserva.EstadoReserva != EstadoReservaEnum.Activa)
        {
            await Shell.Current.DisplayAlert("No disponible", 
                "Solo se pueden modificar reservas activas.", "OK");
            return;
        }

        if (reserva.FechaInicio <= DateTime.Now)
        {
            await Shell.Current.DisplayAlert("No disponible", 
                "No se pueden modificar reservas que ya han comenzado.", "OK");
            return;
        }
        
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

        // ✅ VALIDAR QUE SOLO SE PUEDAN CANCELAR RESERVAS ACTIVAS
        if (reserva.EstadoReserva != EstadoReservaEnum.Activa)
        {
            await Shell.Current.DisplayAlert("No disponible", 
                "Solo se pueden cancelar reservas activas.", "OK");
            return;
        }

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
                    await Shell.Current.DisplayAlert("✅ Reserva Cancelada", 
                        $"La reserva en {lugarTexto} ha sido cancelada exitosamente", "OK");
                    
                    // ✅ RECARGAR TODAS LAS RESERVAS PARA MOSTRAR EL CAMBIO DE ESTADO
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
        if (reserva == null) 
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo obtener la información de la reserva", "OK");
            return;
        }

        try
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Navegando a TicketPage con reservaId: {reserva.Id}");
            
            // ✅ CORRECCIÓN: Agregar la ruta y el parámetro
            await Shell.Current.GoToAsync($"///TicketPage?reservaId={reserva.Id}");
            
            System.Diagnostics.Debug.WriteLine("[DEBUG] Navegación exitosa a TicketPage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error al navegar a TicketPage: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", $"No se pudo abrir el ticket: {ex.Message}", "OK");
        }
    }

    public override async Task OnAppearingAsync()
    {
        // Refrescar el ID del usuario en caso de que haya cambiado
        _idUserLogin = Preferences.Get("UserLoginId", 0);
        System.Diagnostics.Debug.WriteLine($"[DEBUG] OnAppearingAsync - UserLoginId actualizado: {_idUserLogin}");
        
        // Siempre recargar las reservas cuando la página aparece
        OnGetAll();
        
        await base.OnAppearingAsync();
    }
}

