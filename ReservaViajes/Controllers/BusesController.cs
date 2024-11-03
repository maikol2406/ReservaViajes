using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservaViajes.Data;
using ReservaViajes.Models.Buses;
using ReservaViajes.Models.Rutas;

namespace ReservaViajes.Controllers
{
    public class BusesController : Controller
    {
        private readonly BaseDatos _baseDatos;

        public BusesController(BaseDatos baseDatos)
        {
            _baseDatos = baseDatos;
        }

        // GET: BusesController
        public async Task<ActionResult> VerBuses()
        {
            List<Bus> listaBuses = await _baseDatos.RetornaBuses();
            
            return View(listaBuses);
        }

        // GET: BusesController/Create
        public async Task<ActionResult> AgregarBus()
        {
            int idBus = 0;
            List<Bus> listaBuses = await _baseDatos.RetornaBuses();
            foreach (var item in listaBuses)
            {
                idBus = item.idBus;
            }
            idBus++;
            var bus = new Bus { idBus = idBus };
            List<SelectListItem> rutas = new List<SelectListItem>();
            foreach (var item in await _baseDatos.ObtenerRutas())
            {
                rutas.Add(new SelectListItem
                {
                    Text = item.nombreRuta,
                    Value = item.idRuta.ToString()
                });
            }
            ViewBag.rutas = rutas;
            
            return View(bus);
        }

        // POST: BusesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AgregarBus(Bus bus)
        {
            try
            {
                Ruta ruta = await _baseDatos.ObtenerRuta(bus.idRuta);
                bus.nombreRuta = ruta.nombreRuta;

                await _baseDatos.agregarBus(bus);
                return RedirectToAction(nameof(VerBuses));
            }
            catch
            {
                return View();
            }
        }

        // GET: BusesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: BusesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BusesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BusesController/Create
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

        // GET: BusesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BusesController/Edit/5
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

        // GET: BusesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BusesController/Delete/5
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
