using AppMovil.Pages;
using AppMovil.ViewModels;
using Microsoft.Extensions.Logging;
using Service.Interfaces;
using Service.Services;

namespace AppMovil
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Registrar servicios
            builder.Services.AddSingleton<IAuthService, AuthService>();

            // Registrar ViewModels
            builder.Services.AddTransient<LoginPageViewModel>();
            builder.Services.AddTransient<RegisterPageViewModel>();
            builder.Services.AddTransient<ReservasPageViewModel>();
            builder.Services.AddTransient<PagoPageViewModel>();
            builder.Services.AddTransient<SuscripcionesPageViewModel>();
            builder.Services.AddTransient<HistorialPageViewModel>();
            builder.Services.AddTransient<TicketPageViewModel>();
            builder.Services.AddTransient<PerfilPageViewModel>();

            // Registrar páginas
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<ReservasPage>();
            builder.Services.AddTransient<PagoPage>();
            builder.Services.AddTransient<SuscripcionesPage>();
            builder.Services.AddTransient<HistorialPage>();
            builder.Services.AddTransient<TicketPage>();
            builder.Services.AddTransient<PerfilPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
