using System.ComponentModel.DataAnnotations;

namespace ReservaViajes.Models.Reservas
{
    public class ListaReservas
    {
        public List<Reserva>? listaReservas { get; set; }
    }
    
    public class Reserva
    {
        [Required]
        [Display(Name = "# Reserva")]
        public int idReserva { get; set; }
        [Required]
        [Display(Name = "Usuario")]
        public int idUsuario { get; set; }
        [Display(Name = "Nombre Usuario")]
        public string nombreUsuario { get; set; }
        [Required]
        [Display(Name = "Id Bus")]
        public int idBus { get; set; }
        [Required]
        [Display(Name = "Nombre Bus")]
        public string nombreBus { get; set; }
        [Required]
        [Display(Name = "Id Ruta")]
        public int idRuta { get; set; }
        [Display(Name = "Nombre Ruta")]
        public string nombreRuta { get; set; }
        [Required]
        [Display(Name = "Asientos seleccionados")]
        public List<int> asientosSeleccionados { get; set; } = new List<int>();
        [Required]
        [Display(Name = "Monto total")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public int costo{ get; set; }
        public bool estado { get; set; }
    }
}
