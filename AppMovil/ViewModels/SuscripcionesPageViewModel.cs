using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AppMovil.ViewModels;

public partial class SuscripcionesPageViewModel : BaseViewModel
{
    [ObservableProperty]
    private SuscripcionActual? suscripcionActual;

    [ObservableProperty]
    private ObservableCollection<PlanSuscripcion> planesDisponibles = new();

    [ObservableProperty]
    private bool tieneSuscripcionActiva;

    public SuscripcionesPageViewModel()
    {
        Title = "Suscripciones";
        LoadSuscripciones();
    }

    private void LoadSuscripciones()
    {
        // Simular suscripción actual
        SuscripcionActual = new SuscripcionActual
        {
            Plan = "Plan Premium",
            FechaInicio = DateTime.Now.AddMonths(-1),
            FechaVencimiento = DateTime.Now.AddMonths(2),
            Estado = "Activa",
            PrecioMensual = 1500
        };

        TieneSuscripcionActiva = SuscripcionActual?.Estado == "Activa";

        // Simular planes disponibles
        PlanesDisponibles = new ObservableCollection<PlanSuscripcion>
        {
            new PlanSuscripcion 
            { 
                Nombre = "Plan Básico", 
                PrecioMensual = 800, 
                Descripcion = "5 reservas por mes",
                Beneficios = new List<string> { "5 reservas mensuales", "Soporte básico" }
            },
            new PlanSuscripcion 
            { 
                Nombre = "Plan Premium", 
                PrecioMensual = 1500, 
                Descripcion = "Reservas ilimitadas",
                Beneficios = new List<string> { "Reservas ilimitadas", "Soporte prioritario", "Descuentos especiales" }
            },
            new PlanSuscripcion 
            { 
                Nombre = "Plan VIP", 
                PrecioMensual = 2500, 
                Descripcion = "Todo incluido + beneficios exclusivos",
                Beneficios = new List<string> { "Reservas ilimitadas", "Soporte 24/7", "Descuentos exclusivos", "Acceso anticipado" }
            }
        };
    }

    [RelayCommand]
    private async Task RenovarSuscripcion()
    {
        if (SuscripcionActual == null) return;

        await Shell.Current.GoToAsync($"PagoPage?tipo=renovacion&monto={SuscripcionActual.PrecioMensual}&descripcion=Renovación {SuscripcionActual.Plan}");
    }

    [RelayCommand]
    private async Task CambiarPlan(PlanSuscripcion plan)
    {
        if (plan == null) return;

        var result = await Shell.Current.DisplayAlert("Cambiar Plan", 
            $"¿Desea cambiar al {plan.Nombre}?", "Sí", "No");

        if (result)
        {
            await Shell.Current.GoToAsync($"PagoPage?tipo=cambio&monto={plan.PrecioMensual}&descripcion=Cambio a {plan.Nombre}");
        }
    }

    [RelayCommand]
    private async Task ActivarPlan(PlanSuscripcion plan)
    {
        if (plan == null) return;

        await Shell.Current.GoToAsync($"PagoPage?tipo=activacion&monto={plan.PrecioMensual}&descripcion=Activación {plan.Nombre}");
    }

    public override async Task OnAppearingAsync()
    {
        LoadSuscripciones();
        await base.OnAppearingAsync();
    }
}

public class SuscripcionActual
{
    public string Plan { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal PrecioMensual { get; set; }
}

public class PlanSuscripcion
{
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioMensual { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public List<string> Beneficios { get; set; } = new();
}