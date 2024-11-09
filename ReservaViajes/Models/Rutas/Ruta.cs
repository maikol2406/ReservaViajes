using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ReservaViajes.Models.Rutas
{
    public class Ruta
    {
        [Required]
        [Display(Name = "Id Ruta")]
        public int idRuta { get; set; }
        [Display(Name = "Nombre de ruta")]
        [Required]
        public string nombreRuta { get; set; }
        [Display(Name = "Origen")]
        [Required]
        public string origen { get; set; }
        [Display(Name = "Destino")]
        [Required]
        public string destino { get; set; }
        [Display(Name = "Fecha y hora")]
        [Required]
        public DateTime fechaRuta { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Costo")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal costo { get; set; }

        public string FormattedCosto => string.Format(new CultureInfo("es-CR"), "{0:C}", costo);
    }
}
