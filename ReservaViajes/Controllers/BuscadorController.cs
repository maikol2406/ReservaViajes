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

        // GET: BuscadorController
        public ActionResult Index()
        {
            return View();
        }

        // GET: BuscadorController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BuscadorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BuscadorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BuscadorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BuscadorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BuscadorController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BuscadorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
