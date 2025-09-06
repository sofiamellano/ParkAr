using System;

namespace Service.Models
{
    public class Configuracion
    {
        public int Id { get; set; }
        public string NombreEmpresa { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
        public TimeSpan HorarioApertura { get; set; }
        public TimeSpan HorarioCierre { get; set; }
        public decimal PrecioHora { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
