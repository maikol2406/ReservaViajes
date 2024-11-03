using System.ComponentModel.DataAnnotations;

namespace ReservaViajes.Models.Usuarios
{
    public class Login
    {
        [Required]
        [Display(Name = "Cédula")]
        public int idUsuario { get; set; }
        [Required]
        [Display(Name = "Contraseña")]
        public string password { get; set; }
    }
}
