using System.ComponentModel.DataAnnotations;

namespace ReservaViajes.Models.Buses
{
    public class Bus
    {
        [Required]
        [Display(Name = "Id Bus")]
        public int idBus { get; set; }
        [Required]
        [Display(Name = "Nombre Bus")]
        public string nombre { get; set; }
        [Required]
        [Display(Name = "Placa")]
        public string placa { get; set; }
        [Required]
        [Display(Name = "Cantidad de asientos")]
        [Range(0, 52, ErrorMessage = "La cantidad ingresada no es correcta")]
        public int asientos { get; set; }
        //[Required]
        public int idRuta { get; set; }
        public string nombreRuta { get; set; }
    }
}
