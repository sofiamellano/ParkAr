using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Service.Models;
using Service.Services;
using System.Collections.ObjectModel;

namespace AppMovil.ViewModels;

public partial class ReservasPageViewModel : BaseViewModel
{
    ReservaService _reservaService = new ReservaService();

    [ObservableProperty]
    private string mensajeVacio = "No tienes reservas activas";

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

            var reservasList = await _reservaService.GetByUsuarioAsync(_idUserLogin);
            reservas = new ObservableCollection<Reserva>(reservasList ?? new List<Reserva>());
            
            // Actualizar mensaje si no hay reservas
            if (!reservas.Any())
            {
                MensajeVacio = "No tienes reservas activas";
            }
            
            OnPropertyChanged(nameof(Reservas));
            OnPropertyChanged(nameof(TieneReservas));
        }
        catch (ArgumentException ex)
        {
            // Error de validación (usuario no logueado)
            MensajeVacio = "Debes iniciar sesión para ver tus reservas";
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            // Otros errores
            MensajeVacio = "Error al cargar las reservas";
            await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar las reservas: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CreateReserva()
    {
        await Shell.Current.DisplayAlert("Nueva Reserva", "Función para crear nueva reserva", "OK");
    }

    [RelayCommand]
    private async Task EditReserva(Reserva reserva)
    {
        if (reserva == null) return;
        var lugarTexto = reserva.Lugar?.ToString() ?? "Plaza N/A";
        await Shell.Current.DisplayAlert("Modificar Reserva", $"Modificar reserva: {lugarTexto}", "OK");
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
            Reservas.Remove(reserva);
            OnPropertyChanged(nameof(TieneReservas));
            await Shell.Current.DisplayAlert("Reserva Cancelada", "La reserva ha sido cancelada", "OK");
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
        OnGetAll();
        await base.OnAppearingAsync();
    }
}

