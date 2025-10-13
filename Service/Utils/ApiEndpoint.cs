using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Utils
{
    public static class ApiEndpoints
    {
        public static string Usuario { get; set; } = "usuarios";
        public static string Vehiculo { get; set; } = "vehiculos";
        public static string Lugar { get; set; } = "lugares";
        public static string Plan { get; set; } = "planes";
        public static string Suscripcion { get; set; } = "suscripciones";
        public static string Reserva { get; set; } = "reservas";
        public  static string Login { get; set; } = "auth";

        public static string GetEndpoint(string name)
        {
            return name switch
            {
                nameof(Usuario) => Usuario,
                nameof(Vehiculo) => Vehiculo,
                nameof(Lugar) => Lugar,
                nameof(Plan) => Plan,
                nameof(Suscripcion) => Suscripcion,
                nameof(Reserva) => Reserva,
                nameof(Login) => Login,
                _ => throw new ArgumentException($"Endpoint '{name}' no está definido.")
            };
        }
    }
}
