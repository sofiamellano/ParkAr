using Service.Enums;
using System.Globalization;

namespace AppMovil.Converters
{
    public class EstadoReservaToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EstadoReservaEnum estado)
            {
                return estado switch
                {
                    EstadoReservaEnum.Activa => Colors.Black,        // Negro para activas
                    EstadoReservaEnum.Cancelada => Colors.Red,       // Rojo para canceladas
                    EstadoReservaEnum.Finalizada => Colors.Gray,     // Gris para finalizadas
                    _ => Colors.Black
                };
            }
            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}