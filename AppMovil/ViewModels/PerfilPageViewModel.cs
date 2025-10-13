using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Service.Models;
using Service.Services;
using Service.Enums;
using System.Collections.ObjectModel;

namespace AppMovil.ViewModels;

public partial class PerfilPageViewModel : BaseViewModel
{
    private readonly UsuarioService _usuarioService = new();
    private readonly VehiculoService _vehiculoService = new();
    private int _idUserLogin;

    [ObservableProperty]
    private Usuario? usuario;

    [ObservableProperty]
    private string nombreUsuario = "Cargando...";

    [ObservableProperty]
    private string emailUsuario = "Cargando...";

    [ObservableProperty]
    private ObservableCollection<Vehiculo> vehiculosUsuario = new();

    [ObservableProperty]
    private bool isLoggedIn = true;

    [ObservableProperty]
    private bool tieneVehiculos = false;

    public PerfilPageViewModel()
    {
        Title = "Perfil";
        _idUserLogin = Preferences.Get("UserLoginId", 0);
    }

    public override async Task OnAppearingAsync()
    {
        await CargarDatosUsuario();
        await CargarVehiculosUsuario();
        await base.OnAppearingAsync();
    }

    private async Task CargarDatosUsuario()
    {
        try
        {
            if (_idUserLogin <= 0) return;

            IsBusy = true;
            Usuario = await _usuarioService.GetByIdAsync(_idUserLogin);

            if (Usuario != null)
            {
                NombreUsuario = Usuario.Nombre ?? "Usuario";
                EmailUsuario = Usuario.Email ?? "Sin email";
                IsLoggedIn = true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error al cargar datos del usuario: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "No se pudieron cargar los datos del perfil", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CargarVehiculosUsuario()
    {
        try
        {
            if (_idUserLogin <= 0) return;

            var vehiculos = await _vehiculoService.GetByUsuarioAsync(_idUserLogin);
            VehiculosUsuario = new ObservableCollection<Vehiculo>(vehiculos ?? new List<Vehiculo>());
            TieneVehiculos = VehiculosUsuario.Count > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error al cargar vehículos: {ex.Message}");
            VehiculosUsuario = new ObservableCollection<Vehiculo>();
            TieneVehiculos = false;
        }
    }

    [RelayCommand]
    private async Task AgregarVehiculo()
    {
        try
        {
            // Solicitar patente
            var patente = await Shell.Current.DisplayPromptAsync(
                "Nuevo Vehículo", 
                "Ingrese la patente:", 
                "Continuar", "Cancelar", 
                placeholder: "ABC123");

            if (string.IsNullOrWhiteSpace(patente)) return;

            // Seleccionar tipo de vehículo usando los valores del enum
            var tipoSeleccionado = await Shell.Current.DisplayActionSheet(
                "Tipo de Vehículo", 
                "Cancelar", 
                null, 
                "Auto", "Moto", "Camioneta", "Bicicleta");

            if (tipoSeleccionado == "Cancelar" || string.IsNullOrEmpty(tipoSeleccionado)) return;

            // Convertir string a enum
            TipoVehiculoEnum tipoEnum;
            switch (tipoSeleccionado)
            {
                case "Auto":
                    tipoEnum = TipoVehiculoEnum.Auto;
                    break;
                case "Moto":
                    tipoEnum = TipoVehiculoEnum.Moto;
                    break;
                case "Camioneta":
                    tipoEnum = TipoVehiculoEnum.Camioneta;
                    break;
                case "Bicicleta":
                    tipoEnum = TipoVehiculoEnum.Bicicleta;
                    break;
                default:
                    tipoEnum = TipoVehiculoEnum.Auto;
                    break;
            }

            // Crear nuevo vehículo (solo con los campos que existen)
            var nuevoVehiculo = new Vehiculo
            {
                UsuarioId = _idUserLogin,
                Patente = patente.ToUpper(),
                TipoVehiculo = tipoEnum
            };

            IsBusy = true;
            var vehiculoCreado = await _vehiculoService.AddAsync(nuevoVehiculo);

            if (vehiculoCreado != null)
            {
                await Shell.Current.DisplayAlert("✅ Éxito", 
                    $"Vehículo {patente.ToUpper()} agregado correctamente", "OK");
                
                // Recargar vehículos
                await CargarVehiculosUsuario();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] Error al agregar vehículo: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", $"No se pudo agregar el vehículo: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EditarVehiculo(Vehiculo vehiculo)
    {
        if (vehiculo == null) return;

        var action = await Shell.Current.DisplayActionSheet(
            $"Vehículo: {vehiculo.Patente}",
            "Cancelar",
            "Eliminar",
            "Editar Patente",
            "Cambiar Tipo");

        try
        {
            IsBusy = true;

            switch (action)
            {
                case "Editar Patente":
                    var nuevaPatente = await Shell.Current.DisplayPromptAsync(
                        "Editar Patente", 
                        "Nueva patente:", 
                        "Guardar", "Cancelar", 
                        vehiculo.Patente);
                    
                    if (!string.IsNullOrWhiteSpace(nuevaPatente) && nuevaPatente != vehiculo.Patente)
                    {
                        vehiculo.Patente = nuevaPatente.ToUpper();
                        await _vehiculoService.UpdateAsync(vehiculo);
                        await CargarVehiculosUsuario();
                        await Shell.Current.DisplayAlert("✅ Actualizado", "Patente actualizada", "OK");
                    }
                    break;

                case "Cambiar Tipo":
                    var nuevoTipo = await Shell.Current.DisplayActionSheet(
                        "Nuevo Tipo de Vehículo", 
                        "Cancelar", 
                        null, 
                        "Auto", "Moto", "Camioneta", "Bicicleta");
                    
                    if (nuevoTipo != "Cancelar" && !string.IsNullOrEmpty(nuevoTipo))
                    {
                        TipoVehiculoEnum tipoEnum;
                        switch (nuevoTipo)
                        {
                            case "Auto":
                                tipoEnum = TipoVehiculoEnum.Auto;
                                break;
                            case "Moto":
                                tipoEnum = TipoVehiculoEnum.Moto;
                                break;
                            case "Camioneta":
                                tipoEnum = TipoVehiculoEnum.Camioneta;
                                break;
                            case "Bicicleta":
                                tipoEnum = TipoVehiculoEnum.Bicicleta;
                                break;
                            default:
                                tipoEnum = vehiculo.TipoVehiculo;
                                break;
                        }

                        if (tipoEnum != vehiculo.TipoVehiculo)
                        {
                            vehiculo.TipoVehiculo = tipoEnum;
                            await _vehiculoService.UpdateAsync(vehiculo);
                            await CargarVehiculosUsuario();
                            await Shell.Current.DisplayAlert("✅ Actualizado", "Tipo actualizado", "OK");
                        }
                    }
                    break;

                case "Eliminar":
                    var confirmar = await Shell.Current.DisplayAlert(
                        "Eliminar Vehículo", 
                        $"¿Eliminar {vehiculo.Patente}?\n\nEsto también eliminará todas las reservas asociadas.", 
                        "Eliminar", "Cancelar");
                    
                    if (confirmar)
                    {
                        await _vehiculoService.DeleteAsync(vehiculo.Id);
                        await CargarVehiculosUsuario();
                        await Shell.Current.DisplayAlert("✅ Eliminado", "Vehículo eliminado", "OK");
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo actualizar: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EditProfile()
    {
        if (!IsLoggedIn || Usuario == null)
        {
            await Shell.Current.DisplayAlert("Error", "Debe iniciar sesión para editar su perfil", "OK");
            return;
        }

        var action = await Shell.Current.DisplayActionSheet(
            "Editar Perfil",
            "Cancelar",
            null,
            "Editar Nombre",
            "Editar Email");

        if (action == "Cancelar" || string.IsNullOrEmpty(action)) return;

        try
        {
            IsBusy = true;
            bool actualizado = false;

            switch (action)
            {
                case "Editar Nombre":
                    var nuevoNombre = await Shell.Current.DisplayPromptAsync(
                        "Editar Nombre", 
                        "Nuevo nombre:", 
                        "Guardar", "Cancelar", 
                        Usuario.Nombre);
                    
                    if (!string.IsNullOrWhiteSpace(nuevoNombre) && nuevoNombre != Usuario.Nombre)
                    {
                        Usuario.Nombre = nuevoNombre;
                        actualizado = true;
                    }
                    break;

                case "Editar Email":
                    var nuevoEmail = await Shell.Current.DisplayPromptAsync(
                        "Editar Email", 
                        "Nuevo email:", 
                        "Guardar", "Cancelar", 
                        Usuario.Email,
                        keyboard: Keyboard.Email);
                    
                    if (!string.IsNullOrWhiteSpace(nuevoEmail) && nuevoEmail != Usuario.Email)
                    {
                        Usuario.Email = nuevoEmail;
                        actualizado = true;
                    }
                    break;
            }

            if (actualizado)
            {
                await _usuarioService.UpdateAsync(Usuario);
                await CargarDatosUsuario();
                await Shell.Current.DisplayAlert("✅ Actualizado", "Perfil actualizado correctamente", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo actualizar: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ChangePassword()
    {
        await Shell.Current.DisplayAlert("Cambiar Contraseña", "Funcionalidad próximamente", "OK");
    }

    [RelayCommand]
    private async Task Logout()
    {
        var result = await Shell.Current.DisplayAlert("Cerrar Sesión", 
            "¿Está seguro de que desea cerrar sesión?", "Sí", "No");

        if (result)
        {
            // Limpiar datos de sesión
            Preferences.Remove("UserLoginId");
            Service.Services.GenericService<object>.jwtToken = string.Empty;
            
            await Shell.Current.DisplayAlert("✅ Sesión Cerrada", "Ha cerrado sesión correctamente", "OK");
            
            // Navegar al login
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    [RelayCommand]
    private async Task ShowSettings()
    {
        await Shell.Current.DisplayAlert("Configuración", "Funcionalidad próximamente", "OK");
    }

    [RelayCommand]
    private async Task ShowHelp()
    {
        var mensaje = "📱 ParkAR - Sistema de Reservas\n\n" +
                     "🅿️ Reserva plazas de estacionamiento\n" +
                     "🚗 Gestiona tus vehículos\n" +
                     "📊 Ve tu historial de reservas\n" +
                     "🎫 Accede a tus tickets digitales\n\n" +
                     "Para soporte contacta:\n" +
                     "📧 soporte@parkar.com";
        
        await Shell.Current.DisplayAlert("ℹ️ Ayuda", mensaje, "OK");
    }
}