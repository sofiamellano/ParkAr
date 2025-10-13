using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Service.Models;
using Service.Services;
using Service.Enums;
using System.Collections.ObjectModel;

namespace AppMovil.ViewModels;

public partial class SuscripcionesPageViewModel : BaseViewModel
{
    private readonly SuscripcionService _suscripcionService = new();
    private readonly PlanService _planService = new();
    private int _idUserLogin;

    [ObservableProperty]
    private SuscripcionActual? suscripcionActual;

    [ObservableProperty]
    private ObservableCollection<SuscripcionDisponible> suscripcionesDisponibles = new();

    [ObservableProperty]
    private bool tieneSuscripcionActiva;

    public SuscripcionesPageViewModel()
    {
        Title = "Suscripciones";
        _idUserLogin = Preferences.Get("UserLoginId", 0);
    }

    public override async Task OnAppearingAsync()
    {
        await LoadSuscripciones();
        await base.OnAppearingAsync();
    }

    private async Task LoadSuscripciones()
    {
        try
        {
            IsBusy = true;
            
            await CargarSuscripcionActual();
            await CargarSuscripcionesDisponibles();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error al cargar suscripciones: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "No se pudieron cargar las suscripciones", "OK");
            
            // Inicializar colecciones vacías en caso de error
            SuscripcionActual = null;
            TieneSuscripcionActiva = false;
            SuscripcionesDisponibles = new ObservableCollection<SuscripcionDisponible>();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CargarSuscripcionActual()
    {
        if (_idUserLogin <= 0) return;

        try
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Cargando suscripción para usuario: {_idUserLogin}");
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Fecha/Hora actual: {DateTime.Now}");

            var suscripciones = await _suscripcionService.GetByUsuarioAsync(_idUserLogin);
            
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Suscripciones encontradas: {suscripciones?.Count ?? 0}");
            
            if (suscripciones != null)
            {
                foreach (var s in suscripciones)
                {
                    var esFuturo = s.FechaFin.Date >= DateTime.Now.Date; // ✅ COMPARAR SOLO FECHAS
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Suscripción ID: {s.Id}, Estado: {s.Estado} ({(int)s.Estado}), FechaFin: {s.FechaFin}, Válida: {esFuturo}, Plan: {s.Plan?.Nombre ?? "NULL"}");
                }
            }

            // ✅ CORREGIR LA COMPARACIÓN DE FECHAS
            var suscripcionActiva = suscripciones?.FirstOrDefault(s => 
                s.Estado == EstadoSuscripcionEnum.Activo && 
                !s.IsDeleted &&
                s.FechaFin.Date >= DateTime.Now.Date); // ✅ COMPARAR SOLO FECHAS SIN HORA

            System.Diagnostics.Debug.WriteLine($"[DEBUG] Suscripción activa encontrada: {suscripcionActiva != null}");

            if (suscripcionActiva?.Plan != null)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Plan encontrado directamente: {suscripcionActiva.Plan.Nombre}");
                
                SuscripcionActual = new SuscripcionActual
                {
                    Nombre = suscripcionActiva.Plan.Nombre,
                    FechaInicio = suscripcionActiva.FechaInicio,
                    FechaVencimiento = suscripcionActiva.FechaFin,
                    Estado = "Activa",
                    PrecioMensual = suscripcionActiva.Plan.Precio
                };
                
                TieneSuscripcionActiva = true;
                System.Diagnostics.Debug.WriteLine("[DEBUG] ✅ Suscripción activa configurada correctamente");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] No se encontró suscripción activa válida");
                SuscripcionActual = null;
                TieneSuscripcionActiva = false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error al cargar suscripción actual: {ex.Message}");
            SuscripcionActual = null;
            TieneSuscripcionActiva = false;
        }
    }

    private async Task CargarSuscripcionesDisponibles()
    {
        try
        {
            // Cargar planes disponibles desde la API y convertirlos a suscripciones disponibles
            var planes = await _planService.GetAllAsync();
            var suscripcionesDisponiblesList = new List<SuscripcionDisponible>();

            foreach (var plan in planes?.Where(p => !p.IsDeleted) ?? new List<Plan>())
            {
                var suscripcionDisponible = new SuscripcionDisponible
                {
                    Id = plan.Id,
                    Nombre = plan.Nombre,
                    PrecioMensual = plan.Precio,
                    Descripcion = plan.Descripcion ?? "Suscripción disponible",
                    DuracionDias = plan.Duracion ?? 30, // Por defecto 30 días si no está especificado
                    Beneficios = GenerarBeneficios(plan)
                };
                
                suscripcionesDisponiblesList.Add(suscripcionDisponible);
            }

            SuscripcionesDisponibles = new ObservableCollection<SuscripcionDisponible>(suscripcionesDisponiblesList);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error al cargar suscripciones disponibles: {ex.Message}");
            SuscripcionesDisponibles = new ObservableCollection<SuscripcionDisponible>();
        }
    }

    private List<string> GenerarBeneficios(Plan plan)
    {
        var beneficios = new List<string>();
        
        // Generar beneficios basados en el precio y duración del plan
        if (plan.Precio <= 1000)
        {
            beneficios.AddRange(new[] { "5 reservas mensuales", "Soporte básico" });
        }
        else if (plan.Precio <= 2000)
        {
            beneficios.AddRange(new[] { "Reservas ilimitadas", "Soporte prioritario", "Descuentos especiales" });
        }
        else
        {
            beneficios.AddRange(new[] { "Reservas ilimitadas", "Soporte 24/7", "Descuentos exclusivos", "Acceso anticipado" });
        }

        // Agregar duración si está disponible
        if (plan.Duracion.HasValue)
        {
            var duracionTexto = plan.Duracion.Value >= 365 ? "Anual" : 
                               plan.Duracion.Value >= 30 ? "Mensual" : 
                               $"{plan.Duracion.Value} días";
            beneficios.Add($"Duración: {duracionTexto}");
        }

        return beneficios;
    }

    [RelayCommand]
    private async Task RenovarSuscripcion()
    {
        if (SuscripcionActual == null) return;

        try
        {
            var confirmar = await Shell.Current.DisplayAlert("Renovar Suscripción", 
                $"¿Desea renovar su {SuscripcionActual.Nombre}?", 
                "Sí", "No");

            if (confirmar)
            {
                IsBusy = true;
                
                // TODO: Lógica para renovar suscripción existente
                // Por ahora solo mostrar mensaje de éxito
                await Shell.Current.DisplayAlert("✅ Suscripción Renovada", 
                    $"Su suscripción {SuscripcionActual.Nombre} ha sido renovada exitosamente.", "OK");
                
                // Recargar datos
                await LoadSuscripciones();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al renovar: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CambiarSuscripcion(SuscripcionDisponible suscripcion)
    {
        if (suscripcion == null) return;

        try
        {
            var accion = TieneSuscripcionActiva ? "cambiar a" : "activar";
            var result = await Shell.Current.DisplayAlert($"{(TieneSuscripcionActiva ? "Cambiar" : "Activar")} Suscripción", 
                $"¿Desea {accion} {suscripcion.Nombre}?", "Sí", "No");

            if (result)
            {
                await ActivarSuscripcionDirectamente(suscripcion);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al procesar: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task ActivarSuscripcion(SuscripcionDisponible suscripcion)
    {
        if (suscripcion == null) return;

        try
        {
            var result = await Shell.Current.DisplayAlert("Activar Suscripción", 
                $"¿Desea activar {suscripcion.Nombre}?", "Activar", "Cancelar");

            if (result)
            {
                await ActivarSuscripcionDirectamente(suscripcion);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al activar suscripción: {ex.Message}", "OK");
        }
    }

    // ✅ NUEVO MÉTODO PARA ACTIVAR SUSCRIPCIÓN DIRECTAMENTE
    private async Task ActivarSuscripcionDirectamente(SuscripcionDisponible suscripcionDisponible)
    {
        try
        {
            IsBusy = true;
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Activando suscripción: {suscripcionDisponible.Nombre} para usuario: {_idUserLogin}");

            // 1. Desactivar suscripción actual si existe
            if (TieneSuscripcionActiva)
            {
                var suscripcionesActuales = await _suscripcionService.GetByUsuarioAsync(_idUserLogin);
                var suscripcionActual = suscripcionesActuales?.FirstOrDefault(s => 
                    s.Estado == EstadoSuscripcionEnum.Activo && !s.IsDeleted);

                if (suscripcionActual != null)
                {
                    suscripcionActual.Estado = EstadoSuscripcionEnum.Cancelado;
                    await _suscripcionService.UpdateAsync(suscripcionActual);
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Suscripción anterior cancelada: {suscripcionActual.Id}");
                }
            }

            // 2. Crear nueva suscripción
            var nuevaSuscripcion = new Suscripcion
            {
                UsuarioId = _idUserLogin,
                PlanId = suscripcionDisponible.Id,
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(suscripcionDisponible.DuracionDias),
                Estado = EstadoSuscripcionEnum.Activo,
                IsDeleted = false
            };

            var suscripcionCreada = await _suscripcionService.AddAsync(nuevaSuscripcion);

            if (suscripcionCreada != null)
            {
                await Shell.Current.DisplayAlert("✅ Suscripción Activada", 
                    $"¡{suscripcionDisponible.Nombre} activada exitosamente!\n\n" +
                    $"Válida hasta: {nuevaSuscripcion.FechaFin:dd/MM/yyyy}", "OK");

                System.Diagnostics.Debug.WriteLine($"[DEBUG] Nueva suscripción creada: {suscripcionCreada.Id}");
                
                // Recargar datos para mostrar la nueva suscripción
                await LoadSuscripciones();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo activar la suscripción", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error al activar suscripción: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", $"Error al activar suscripción: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshSuscripciones()
    {
        await LoadSuscripciones();
    }
}

// Mantener las clases de presentación para el UI
public class SuscripcionActual
{
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal PrecioMensual { get; set; }
}

public class SuscripcionDisponible
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioMensual { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int DuracionDias { get; set; }
    public List<string> Beneficios { get; set; } = new();
}