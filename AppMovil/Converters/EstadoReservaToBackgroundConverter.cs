using Service.Enums;
using System.Globalization;

namespace AppMovil.Converters
{
    public class EstadoReservaToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EstadoReservaEnum estado)
            {
                return estado switch
                {
                    EstadoReservaEnum.Activa => Colors.White,           // Blanco para activas
                    EstadoReservaEnum.Cancelada => Color.FromRgb(255, 240, 240), // Fondo rojizo claro para canceladas
                    EstadoReservaEnum.Finalizada => Color.FromRgb(245, 245, 245), // Fondo gris claro para finalizadas
                    _ => Colors.White
                };
            }
            return Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}