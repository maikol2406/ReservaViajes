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
            int idCliente = int.Parse(User.FindFirst("idUsuario")?.Value);
            List<Reserva> listaFiltrada = new List<Reserva>();
            foreach (var item in listaReservas)
            {
                if (item.idUsuario == idCliente && !item.estado)
                {
                    listaFiltrada.Add(item);
                }
            }
            //List<Reserva> lista = new List<Reserva>();
            //foreach (var item in listaReservas)
            //{
            //    if (item.estado == false)
            //    {
            //        lista.Add(item);
            //    }
            //}

            return View(listaFiltrada);
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
                idReserva = idReserva ,
                idRuta = int.Parse(idRuta),
                idUsuario = int.Parse(User.FindFirst("idUsuario")?.Value)
            };

            var ruta = await _baseDatos.ObtenerRuta(reserva.idRuta);
            if (ruta != null)
            {
                reserva.nombreRuta = ruta.nombreRuta;
            }

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
                reserva.estado = false;

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

        // GET: ReservasController/Delete/5
        public async Task<ActionResult> EliminarReserva(string idReserva)
        {
            try
            {
                await _baseDatos.EliminarReserva(int.Parse(idReserva));
                return RedirectToAction("VerReservas", "Reservas");
            }
            catch
            {
                return RedirectToAction("VerReservas", "Reservas");
            }
        }
    }
}
