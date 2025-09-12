using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppMovil.ViewModels;

public partial class HistorialPageViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<HistorialItem> historialReservas = new();

    [ObservableProperty]
    private ObservableCollection<HistorialItem> historialPagos = new();

    [ObservableProperty]
    private string selectedTab = "Reservas";

    [ObservableProperty]
    private bool isReservasTabSelected = true;

    [ObservableProperty]
    private bool isPagosTabSelected = false;

    public HistorialPageViewModel()
    {
        Title = "Historial";
        LoadHistorial();
    }

    private void LoadHistorial()
    {
        // Simular historial de reservas
        HistorialReservas = new ObservableCollection<HistorialItem>
        {
            new HistorialItem 
            { 
                Id = 1,
                Titulo = "Parking Centro - Plaza 12",
                Fecha = DateTime.Now.AddDays(-2),
                Descripcion = "Reserva completada",
                Estado = "Finalizada",
                Monto = "$500"
            },
            new HistorialItem 
            { 
                Id = 2,
                Titulo = "Parking Norte - Plaza 8",
                Fecha = DateTime.Now.AddDays(-7),
                Descripcion = "Reserva cancelada por el usuario",
                Estado = "Cancelada",
                Monto = "$300"
            },
            new HistorialItem 
            { 
                Id = 3,
                Titulo = "Parking Sur - Plaza 15",
                Fecha = DateTime.Now.AddDays(-15),
                Descripcion = "Reserva completada",
                Estado = "Finalizada",
                Monto = "$450"
            }
        };

        // Simular historial de pagos
        HistorialPagos = new ObservableCollection<HistorialItem>
        {
            new HistorialItem 
            { 
                Id = 4,
                Titulo = "Pago Plan Premium",
                Fecha = DateTime.Now.AddDays(-1),
                Descripcion = "Renovación mensual",
                Estado = "Completado",
                Monto = "$1,500"
            },
            new HistorialItem 
            { 
                Id = 5,
                Titulo = "Pago Reserva",
                Fecha = DateTime.Now.AddDays(-2),
                Descripcion = "Parking Centro - Plaza 12",
                Estado = "Completado",
                Monto = "$500"
            },
            new HistorialItem 
            { 
                Id = 6,
                Titulo = "Reembolso",
                Fecha = DateTime.Now.AddDays(-7),
                Descripcion = "Cancelación reserva",
                Estado = "Procesado",
                Monto = "-$300"
            }
        };
    }

    [RelayCommand]
    private void SelectReservasTab()
    {
        SelectedTab = "Reservas";
        IsReservasTabSelected = true;
        IsPagosTabSelected = false;
    }

    [RelayCommand]
    private void SelectPagosTab()
    {
        SelectedTab = "Pagos";
        IsReservasTabSelected = false;
        IsPagosTabSelected = true;
    }

    [RelayCommand]
    private async Task ViewDetails(HistorialItem item)
    {
        if (item == null) return;

        await Shell.Current.DisplayAlert("Detalles", 
            $"Título: {item.Titulo}\nFecha: {item.Fecha:dd/MM/yyyy}\nDescripción: {item.Descripcion}\nEstado: {item.Estado}\nMonto: {item.Monto}", 
            "OK");
    }

    public override async Task OnAppearingAsync()
    {
        LoadHistorial();
        await base.OnAppearingAsync();
    }
}

public class HistorialItem
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Monto { get; set; } = string.Empty;
}