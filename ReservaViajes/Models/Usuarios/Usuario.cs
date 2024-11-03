using System.ComponentModel.DataAnnotations;

namespace ReservaViajes.Models.Usuarios
{
    public class Usuario
    {
        [Required]
        [Display(Name = "Cédula")]
        public int idUsuario { get; set; }
        [Required]
        [Display(Name = "Nombre completo")]
        public string nombre { get; set; }
        [Required]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        [Required]
        [Display(Name = "Confirmar contraseña")]
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Las contraseñas no coinciden")]
        public string confirmaPassword { get; set; }
    }
}
