using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservaViajes.Data;
using ReservaViajes.Models.Rutas;

namespace ReservaViajes.Controllers
{
    public class BuscadorController : Controller
    {
        private readonly BaseDatos _baseDatos;

        public BuscadorController(BaseDatos baseDatos)
        {
            _baseDatos = baseDatos;
        }

        // GET: BuscadorController
        public async Task<ActionResult> BuscarRutas()
        {
            var origenes = await _baseDatos.ObtenerOrigenes();
            ViewBag.Origenes = origenes.Select(o => new SelectListItem { Value = o, Text = o }).ToList();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ObtenerRutasFiltro(string origen)
        {
            var rutas = await _baseDatos.ObtenerRutasFiltradas(origen);
            return PartialView("_RutasFiltradas", rutas);
        }
    }
}
