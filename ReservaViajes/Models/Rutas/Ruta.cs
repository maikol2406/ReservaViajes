using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ReservaViajes.Models.Rutas
{
    public class Ruta
    {
        [Required]
        public int idRuta { get; set; }
        [Required]
        public string nombreRuta { get; set; }
        [Required]
        public string origen { get; set; }
        [Required]
        public string destino { get; set; }
        [Required]
        public DateTime fechaRuta { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal costo { get; set; }

        public string FormattedCosto => string.Format(new CultureInfo("es-CR"), "{0:C}", costo);
    }
}
