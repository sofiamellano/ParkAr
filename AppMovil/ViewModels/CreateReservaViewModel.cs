using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Service.Enums;
using Service.Models;
using Service.Services;
using System.Collections.ObjectModel;

namespace AppMovil.ViewModels;

[QueryProperty(nameof(ReservaId), "reservaId")]
[QueryProperty(nameof(Modo), "modo")]
public partial class CreateReservaViewModel : BaseViewModel
{
    private readonly ReservaService _reservaService = new();
    private readonly VehiculoService _vehiculoService = new();
    private readonly LugarService _lugarService = new();
    private int _idUserLogin;

    [ObservableProperty]
    private ObservableCollection<Vehiculo> vehiculos = new();

    [ObservableProperty]
    private ObservableCollection<Lugar> lugares = new();

    [ObservableProperty]
    private ObservableCollection<Lugar> lugaresDisponibles = new();

    [ObservableProperty]
    private Vehiculo? vehiculoSeleccionado;

    [ObservableProperty]
    private Lugar? lugarSeleccionado;

    [ObservableProperty]
    private DateTime fechaInicio = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private TimeSpan horaInicio = new TimeSpan(9, 0, 0);

    [ObservableProperty]
    private TimeSpan duracion = new TimeSpan(2, 0, 0);

    [ObservableProperty]
    private bool puedeCrearReserva;

    [ObservableProperty]
    private string mensajeError = string.Empty;

    [ObservableProperty]
    private bool mostrarError;

    // ✅ NUEVAS PROPIEDADES PARA MODO EDICIÓN
    [ObservableProperty]
    private string reservaId = string.Empty;

    [ObservableProperty]
    private string modo = "crear"; // "crear" o "editar"

    [ObservableProperty]
    private bool esModoEdicion;

    [ObservableProperty]
    private string tituloBotonPrincipal = "✅ Crear Reserva";

    private Reserva? _reservaOriginal;
    private bool _reservaCargada = false;

    public CreateReservaViewModel()
    {
        _idUserLogin = Preferences.Get("UserLoginId", 0);
        _ = InitializeAsync();
    }

    // ✅ PROPIEDADES PARCIALES QUE SE EJECUTAN CUANDO CAMBIAN LOS QUERY PARAMETERS
    partial void OnReservaIdChanged(string value)
    {
        ActualizarModoYTitulos();
    }

    partial void OnModoChanged(string value)
    {
        ActualizarModoYTitulos();
    }

    private void ActualizarModoYTitulos()
    {
        EsModoEdicion = !string.IsNullOrEmpty(ReservaId) && Modo == "editar";
        
        if (EsModoEdicion)
        {
            Title = "Modificar Reserva";
            TituloBotonPrincipal = "💾 Guardar Cambios";
        }
        else
        {
            Title = "Nueva Reserva";
            TituloBotonPrincipal = "✅ Crear Reserva";
        }

        // Si estamos en modo edición y ya tenemos los datos cargados, cargar la reserva
        if (EsModoEdicion && Vehiculos.Any() && !_reservaCargada)
        {
            _ = CargarReservaParaEditar();
        }
    }

    private async Task InitializeAsync()
    {
        // Los QueryProperty se establecen después del constructor
        // El modo se actualizará automáticamente cuando cambien
        
        await LoadDatos();
        await ActualizarLugaresDisponibles();
        ValidarFormulario();
    }

    private async Task LoadDatos()
    {
        try
        {
            IsBusy = true;

            // Cargar vehículos del usuario
            var vehiculosList = await _vehiculoService.GetByUsuarioAsync(_idUserLogin);
            Vehiculos = new ObservableCollection<Vehiculo>(vehiculosList ?? new List<Vehiculo>());

            // Cargar todos los lugares
            var lugaresList = await _lugarService.GetAllAsync();
            Lugares = new ObservableCollection<Lugar>(lugaresList ?? new List<Lugar>());

            // Solo seleccionar automáticamente si estamos en modo crear y no hay vehículo seleccionado
            if (!EsModoEdicion && VehiculoSeleccionado == null && Vehiculos.Any())
            {
                VehiculoSeleccionado = Vehiculos.First();
            }

            // Si estamos en modo edición y los QueryProperty ya están establecidos, cargar reserva
            if (EsModoEdicion && !_reservaCargada)
            {
                await CargarReservaParaEditar();
            }

            ValidarFormulario();
        }
        catch (Exception ex)
        {
            MostrarError = true;
            MensajeError = $"Error al cargar datos: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CargarReservaParaEditar()
    {
        if (_reservaCargada) return; // Evitar cargar múltiples veces

        try
        {
            if (!int.TryParse(ReservaId, out int reservaIdInt))
            {
                throw new Exception("ID de reserva inválido");
            }

            IsBusy = true;
            
            // Obtener la reserva a editar
            _reservaOriginal = await _reservaService.GetByIdAsync(reservaIdInt);
            
            if (_reservaOriginal == null)
            {
                throw new Exception("Reserva no encontrada");
            }

            // Cargar los datos en los campos
            FechaInicio = _reservaOriginal.FechaInicio.Date;
            HoraInicio = _reservaOriginal.FechaInicio.TimeOfDay;
            Duracion = _reservaOriginal.FechaFin - _reservaOriginal.FechaInicio;

            // Seleccionar vehículo y lugar
            VehiculoSeleccionado = Vehiculos.FirstOrDefault(v => v.Id == _reservaOriginal.VehiculoId);
            
            // Marcar como cargada
            _reservaCargada = true;

            // Para el lugar, necesitamos cargarlo después de actualizar lugares disponibles
            // porque el lugar actual puede no estar en la lista de disponibles
            
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Reserva cargada para editar: Plaza {_reservaOriginal.Lugar?.Numero}, {_reservaOriginal.FechaInicio:dd/MM/yyyy HH:mm}");
        }
        catch (Exception ex)
        {
            MostrarError = true;
            MensajeError = $"Error al cargar la reserva: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[ERROR] Error al cargar reserva para editar: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ActualizarLugaresDisponibles()
    {
        try
        {
            var fechaHoraInicio = FechaInicio.Date + HoraInicio;
            var fechaHoraFin = fechaHoraInicio + Duracion;

            var lugaresDisponiblesList = await _lugarService.GetDisponiblesAsync(fechaHoraInicio, fechaHoraFin);
            LugaresDisponibles = new ObservableCollection<Lugar>(lugaresDisponiblesList ?? new List<Lugar>());

            // ✅ EN MODO EDICIÓN: Agregar el lugar original a la lista si no está presente
            if (EsModoEdicion && _reservaOriginal != null)
            {
                var lugarOriginal = Lugares.FirstOrDefault(l => l.Id == _reservaOriginal.LugarId);
                if (lugarOriginal != null && !LugaresDisponibles.Any(l => l.Id == lugarOriginal.Id))
                {
                    LugaresDisponibles.Add(lugarOriginal);
                }
                
                // Seleccionar el lugar original
                if (LugarSeleccionado == null)
                {
                    LugarSeleccionado = LugaresDisponibles.FirstOrDefault(l => l.Id == _reservaOriginal.LugarId);
                }
            }
            else
            {
                // Si el lugar seleccionado ya no está disponible, deseleccionarlo
                if (LugarSeleccionado != null && !LugaresDisponibles.Contains(LugarSeleccionado))
                {
                    LugarSeleccionado = null;
                }

                // Seleccionar el primer lugar disponible si no hay uno seleccionado
                if (LugarSeleccionado == null && LugaresDisponibles.Any())
                {
                    LugarSeleccionado = LugaresDisponibles.First();
                }
            }

            ValidarFormulario();
        }
        catch (Exception ex)
        {
            // Si falla obtener lugares disponibles, usar todos los lugares
            LugaresDisponibles = new ObservableCollection<Lugar>(Lugares);
        }
    }

    partial void OnVehiculoSeleccionadoChanged(Vehiculo? value) => ValidarFormulario();
    partial void OnLugarSeleccionadoChanged(Lugar? value) => ValidarFormulario();
    
    partial void OnFechaInicioChanged(DateTime value) 
    {
        _ = ActualizarLugaresDisponibles();
        ValidarFormulario();
    }
    
    partial void OnHoraInicioChanged(TimeSpan value) 
    {
        _ = ActualizarLugaresDisponibles();
        ValidarFormulario();
    }
    
    partial void OnDuracionChanged(TimeSpan value) 
    {
        _ = ActualizarLugaresDisponibles();
        ValidarFormulario();
    }

    private void ValidarFormulario()
    {
        MostrarError = false;
        MensajeError = string.Empty;

        var fechaHoraInicio = FechaInicio.Date + HoraInicio;

        // Validaciones básicas
        if (VehiculoSeleccionado == null)
        {
            MostrarError = true;
            MensajeError = "Debe seleccionar un vehículo";
            PuedeCrearReserva = false;
            return;
        }

        if (LugarSeleccionado == null)
        {
            MostrarError = true;
            MensajeError = "Debe seleccionar una plaza";
            PuedeCrearReserva = false;
            return;
        }

        // En modo edición, permitir fechas pasadas sin cambios
        if (!EsModoEdicion && fechaHoraInicio <= DateTime.Now)
        {
            MostrarError = true;
            MensajeError = "La fecha y hora deben ser futuras";
            PuedeCrearReserva = false;
            return;
        }

        if (Duracion.TotalMinutes < 30)
        {
            MostrarError = true;
            MensajeError = "La duración mínima es de 30 minutos";
            PuedeCrearReserva = false;
            return;
        }

        if (Duracion.TotalHours > 12)
        {
            MostrarError = true;
            MensajeError = "La duración máxima es de 12 horas";
            PuedeCrearReserva = false;
            return;
        }

        if (!LugaresDisponibles.Any())
        {
            MostrarError = true;
            MensajeError = "No hay plazas disponibles para la fecha y hora seleccionada";
            PuedeCrearReserva = false;
            return;
        }

        PuedeCrearReserva = true;
    }

    [RelayCommand]
    private async Task CrearReserva()
    {
        if (!PuedeCrearReserva) return;

        try
        {
            IsBusy = true;

            var fechaHoraInicio = FechaInicio.Date + HoraInicio;
            var fechaHoraFin = fechaHoraInicio + Duracion;

            if (EsModoEdicion)
            {
                // ✅ MODO EDICIÓN: Actualizar reserva existente
                await ActualizarReservaExistente(fechaHoraInicio, fechaHoraFin);
            }
            else
            {
                // ✅ MODO CREAR: Crear nueva reserva
                await CrearNuevaReserva(fechaHoraInicio, fechaHoraFin);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo {(EsModoEdicion ? "actualizar" : "crear")} la reserva: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CrearNuevaReserva(DateTime fechaHoraInicio, DateTime fechaHoraFin)
    {
        // ✅ VALIDACIÓN ADICIONAL ANTES DE CREAR LA RESERVA
        var validacionFinal = await ValidarConflictosAsync(fechaHoraInicio, fechaHoraFin);
        if (!validacionFinal.esValida)
        {
            await Shell.Current.DisplayAlert("❌ Conflicto Detectado", validacionFinal.mensaje, "OK");
            return;
        }

        var nuevaReserva = new Reserva
        {
            UsuarioId = _idUserLogin,
            VehiculoId = VehiculoSeleccionado!.Id,
            LugarId = LugarSeleccionado!.Id,
            FechaInicio = fechaHoraInicio,
            FechaFin = fechaHoraFin,
            EstadoReserva = EstadoReservaEnum.Activa
        };

        var reservaCreada = await _reservaService.AddAsync(nuevaReserva);
        
        if (reservaCreada != null)
        {
            await Shell.Current.DisplayAlert("¡Éxito!", 
                $"Reserva creada exitosamente\n\n" +
                $"📍 Plaza: {LugarSeleccionado}\n" +
                $"🚗 Vehículo: {VehiculoSeleccionado}\n" +
                $"📅 Desde: {fechaHoraInicio:dd/MM/yyyy HH:mm}\n" +
                $"⏰ Hasta: {fechaHoraFin:dd/MM/yyyy HH:mm}", "OK");

            // Navegar de vuelta a ReservasPage y refrescar
            await Shell.Current.GoToAsync("//ReservasPage");
        }
    }

    private async Task ActualizarReservaExistente(DateTime fechaHoraInicio, DateTime fechaHoraFin)
    {
        if (_reservaOriginal == null) return;

        // Validar conflictos solo si se cambió algo significativo
        var fechasOHorariosCambiaron = 
            _reservaOriginal.FechaInicio != fechaHoraInicio ||
            _reservaOriginal.FechaFin != fechaHoraFin ||
            _reservaOriginal.LugarId != LugarSeleccionado!.Id;

        if (fechasOHorariosCambiaron)
        {
            var validacionFinal = await ValidarConflictosAsync(fechaHoraInicio, fechaHoraFin, _reservaOriginal.Id);
            if (!validacionFinal.esValida)
            {
                await Shell.Current.DisplayAlert("❌ Conflicto Detectado", validacionFinal.mensaje, "OK");
                return;
            }
        }

        // Actualizar los datos de la reserva original
        _reservaOriginal.VehiculoId = VehiculoSeleccionado!.Id;
        _reservaOriginal.LugarId = LugarSeleccionado!.Id;
        _reservaOriginal.FechaInicio = fechaHoraInicio;
        _reservaOriginal.FechaFin = fechaHoraFin;

        var success = await _reservaService.UpdateAsync(_reservaOriginal);
        
        if (success)
        {
            await Shell.Current.DisplayAlert("¡Éxito!", 
                $"Reserva actualizada exitosamente\n\n" +
                $"📍 Plaza: {LugarSeleccionado}\n" +
                $"🚗 Vehículo: {VehiculoSeleccionado}\n" +
                $"📅 Desde: {fechaHoraInicio:dd/MM/yyyy HH:mm}\n" +
                $"⏰ Hasta: {fechaHoraFin:dd/MM/yyyy HH:mm}", "OK");

            // Navegar de vuelta a ReservasPage y refrescar
            await Shell.Current.GoToAsync("//ReservasPage");
        }
        else
        {
            throw new Exception("La operación de actualización falló");
        }
    }

    /// Valida que no haya conflictos con reservas existentes
    
    private async Task<(bool esValida, string mensaje)> ValidarConflictosAsync(DateTime fechaHoraInicio, DateTime fechaHoraFin, int? reservaIdExcluir = null)
    {
        try
        {
            // Obtener todas las reservas activas del usuario
            var reservasUsuario = await _reservaService.GetByUsuarioAsync(_idUserLogin);
            var reservasActivas = reservasUsuario?.Where(r => 
                r.EstadoReserva == EstadoReservaEnum.Activa && 
                !r.IsDeleted &&
                r.Id != reservaIdExcluir // Excluir la reserva actual en modo edición
            ).ToList() ?? new List<Reserva>();

            foreach (var reservaExistente in reservasActivas)
            {
                // ✅ 1. VALIDAR MISMA PLAZA Y HORARIO SOLAPADO
                if (reservaExistente.LugarId == LugarSeleccionado!.Id)
                {
                    if (HorariosSesolapan(fechaHoraInicio, fechaHoraFin, reservaExistente.FechaInicio, reservaExistente.FechaFin))
                    {
                        return (false, $"❌ Ya tienes una reserva en la Plaza {LugarSeleccionado.Numero} que se solapa con este horario.\n\n" +
                                     $"Reserva existente: {reservaExistente.FechaInicio:dd/MM/yyyy HH:mm} - {reservaExistente.FechaFin:dd/MM/yyyy HH:mm}");
                    }
                }

                // ✅ 2. VALIDAR MISMO VEHÍCULO Y HORARIO SOLAPADO
                if (reservaExistente.VehiculoId == VehiculoSeleccionado!.Id)
                {
                    if (HorariosSesolapan(fechaHoraInicio, fechaHoraFin, reservaExistente.FechaInicio, reservaExistente.FechaFin))
                    {
                        return (false, $"❌ Tu vehículo {VehiculoSeleccionado} ya tiene una reserva que se solapa con este horario.\n\n" +
                                     $"Reserva existente: {reservaExistente.FechaInicio:dd/MM/yyyy HH:mm} - {reservaExistente.FechaFin:dd/MM/yyyy HH:mm}");
                    }
                }

                // ✅ 3. VALIDAR MÚLTIPLES RESERVAS EN EL MISMO DÍA (opcional - política de negocio)
                if (reservaExistente.FechaInicio.Date == fechaHoraInicio.Date)
                {
                    var reservasDelDia = reservasActivas.Count(r => r.FechaInicio.Date == fechaHoraInicio.Date);
                    if (reservasDelDia >= 3) // Máximo 3 reservas por día
                    {
                        return (false, $"❌ Ya tienes {reservasDelDia} reservas para el día {fechaHoraInicio:dd/MM/yyyy}.\n\n" +
                                     $"Límite máximo: 3 reservas por día.");
                    }
                }
            }

            return (true, "Validación exitosa");
        }
        catch (Exception ex)
        {
            // Si falla la validación, permitir pero con advertencia
            System.Diagnostics.Debug.WriteLine($"[WARNING] Error en validación de conflictos: {ex.Message}");
            return (true, "Validación omitida por error");
        }
    }

    /// Verifica si dos rangos de tiempo se solapan
   
    private bool HorariosSesolapan(DateTime inicio1, DateTime fin1, DateTime inicio2, DateTime fin2)
    {
        // Los horarios se solapan si:
        // - El inicio de uno está entre el inicio y fin del otro
        // - O si uno contiene completamente al otro
        return (inicio1 < fin2) && (fin1 > inicio2);
    }

    [RelayCommand]
    private async Task Cancelar()
    {
        await Shell.Current.GoToAsync("//ReservasPage");
    }

    [RelayCommand]
    private void AgregarTiempo(string tipo)
    {
        switch (tipo)
        {
            case "30min":
                Duracion = Duracion.Add(TimeSpan.FromMinutes(30));
                break;
            case "1h":
                Duracion = Duracion.Add(TimeSpan.FromHours(1));
                break;
            case "-30min":
                if (Duracion.TotalMinutes > 30)
                    Duracion = Duracion.Subtract(TimeSpan.FromMinutes(30));
                break;
            case "-1h":
                if (Duracion.TotalHours > 1)
                    Duracion = Duracion.Subtract(TimeSpan.FromHours(1));
                break;
        }
    }

    [RelayCommand]
    private void EstablecerDuracion(string duracion)
    {
        switch (duracion)
        {
            case "1h":
                Duracion = TimeSpan.FromHours(1);
                break;
            case "2h":
                Duracion = TimeSpan.FromHours(2);
                break;
            case "4h":
                Duracion = TimeSpan.FromHours(4);
                break;
            case "8h":
                Duracion = TimeSpan.FromHours(8);
                break;
        }
    }
}