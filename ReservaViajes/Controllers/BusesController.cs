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
            var bus = new Bus 
            { 
                idBus = idBus 
            };
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
    }
}
