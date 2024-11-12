using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservaViajes.Data;
using ReservaViajes.Models.Rutas;

namespace ReservaViajes.Controllers
{
    public class RutasController : Controller
    {
        private readonly BaseDatos _baseDatos;

        public RutasController(BaseDatos baseDatos)
        {
            _baseDatos = baseDatos;
        }

        // GET: RutasController
        public async Task<ActionResult> VerRutas()
        {
            var listaRutas = await _baseDatos.ObtenerRutas();
            return View(listaRutas);
        }

        // GET: RutasController/Create
        public async Task<ActionResult> CrearRuta()
        {
            int idruta = 0;
            var listaRutas = await _baseDatos.ObtenerRutas();
            foreach (var item in listaRutas)
            {
                idruta = item.idRuta;
            }
            idruta++;
            Ruta ruta = new Ruta
            { 
                idRuta = idruta 
            };
            return View(ruta);
        }

        // POST: RutasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearRuta(Ruta ruta)
        {
            try
            {
                await _baseDatos.agregarRuta(ruta);
                return RedirectToAction(nameof(VerRutas));
            }
            catch
            {
                return View();
            }
        }
    }
}
