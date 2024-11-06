using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json.Linq;
using ReservaViajes.Data;
using ReservaViajes.Models.Reservas;
using ReservaViajes.Models.Usuarios;

namespace ReservaViajes.Controllers
{
    public class ReservasController : Controller
    {
        private readonly BaseDatos _baseDatos;

        public ReservasController(BaseDatos baseDatos)
        {
            _baseDatos = baseDatos;
        }

        // GET: ReservasController
        public async Task<ActionResult> VerReservas()
        {
            var listaReservas = await _baseDatos.ObtenerReservas();
            return View(listaReservas);
        }

        // GET: ReservasController/Create
        public async Task<ActionResult> CrearReservas(string idRuta)
        {
            var listaReservas = await _baseDatos.ObtenerReservas();
            int idReserva = 0;
            foreach (var item in listaReservas)
            {
                idReserva = item.idReserva;
            }
            idReserva++;
            var reserva = new Reserva 
            {
                idReserva = idReserva 
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

            reserva.idRuta = int.Parse(idRuta);

            reserva.idUsuario = int.Parse(User.FindFirst("idUsuario")?.Value);

            return View(reserva);
        }

        // POST: ReservasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearReservas(Reserva reserva)
        {
            try
            {
                var ruta = await _baseDatos.ObtenerRuta(reserva.idRuta);
                reserva.nombreRuta = ruta.nombreRuta;
                var bus = await _baseDatos.ObtenerBus(reserva.idBus);
                reserva.nombreBus = bus.nombre;
                List<int> asientosSeleccionados = reserva.asientosSeleccionados;
                if (ViewBag.Costo != null)
                {
                    reserva.costo = decimal.Parse(ViewBag.Costo.ToString());
                }

                await _baseDatos.AgregarReserva(reserva);

                return RedirectToAction("VerReservas", "Reservas");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerBusesPorRuta(int idRuta)
        {
            var buses = await _baseDatos.ObtenerBusesXRuta(idRuta);
            var ruta = await _baseDatos.ObtenerRuta(idRuta);

            if (buses == null || ruta == null)
            {
                return NotFound();
            }

            return Json(new { buses = buses, costo = ruta.costo });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCantidadAsientos(int idBus)
        {
            var bus = await _baseDatos.ObtenerBusPorId(idBus);
            if (bus != null)
            {
                var asientosReservados = await _baseDatos.ObtenerAsientosOcupados(idBus) ?? new List<int>();
                return Json(new
                {
                    cantidadAsientos = bus.asientos,
                    asientosReservados = asientosReservados
                });
            }

            return NotFound();
        }

        public async Task<JsonResult> ObtenerAsientosOcupados(int idBus)
        {
            var asientosOcupados = await _baseDatos.ObtenerAsientosOcupados(idBus);
            return Json(asientosOcupados);
        }

        // GET: ReservasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReservasController/Edit/5
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

        // GET: ReservasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReservasController/Delete/5
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
