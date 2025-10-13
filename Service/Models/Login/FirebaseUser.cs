using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Models.Login
{
    public class FirebaseUser
    {
        public string Uid { get; set; }             // Identificador único del usuario
        public string DisplayName { get; set; }      // Nombre del usuario (puede ser null)
        public string Email { get; set; }            // Correo electrónico del usuario
        public bool EmailVerified { get; set; }      // Indica si el correo ha sido verificado
        public string PhoneNumber { get; set; }      // Número de teléfono del usuario (puede ser null)
        public string PhotoUrl { get; set; }         // URL de la foto de perfil del usuario (puede ser null)
        public string ProviderId { get; set; }       // Identificador del proveedor (por ejemplo, "firebase")
        public bool IsAnonymous { get; set; }        // Indica si el usuario inició sesión de manera anónima
        public string TenantId { get; set; }         // ID del tenant si se utiliza multitenancy (puede ser null)
    }
}