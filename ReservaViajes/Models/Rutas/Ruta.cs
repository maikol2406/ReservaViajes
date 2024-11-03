namespace ReservaViajes.Models.Rutas
{
    public class Ruta
    {
        public int idRuta { get; set; }
        public string nombreRuta { get; set; }
        public string origen { get; set; }
        public string destino { get; set; }
        public DateTime fechaRuta { get; set; }
    }
}
