using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using NuGet.Common;
using ReservaViajes.Data;
using ReservaViajes.Models.Pagos;
using ReservaViajes.Models.Reservas;
using ReservaViajes.Models.Rutas;
using ReservaViajes.Models.Usuarios;
using System.Diagnostics;
using System.Security.Policy;
using System.Text;
using static System.Net.WebRequestMethods;
using Token = ReservaViajes.Data.Token;

namespace ReservaViajes.Controllers
{
    public class PagosController : Controller
    {
        private readonly BaseDatos _baseDatos;

        public PagosController(BaseDatos baseDatos)
        {
            _baseDatos = baseDatos;
        }

        // GET: PagosController
        public async Task<ActionResult> VerPagos()
        {
            int idCliente = int.Parse(User.FindFirst("idUsuario")?.Value);
            List<Pago> listaPagos = await _baseDatos.ObtenerPagos(idCliente);
            return View(listaPagos);
        }

        // GET: PagosController/Create
        public async Task<ActionResult> CrearPago(string idReserva)
        {
            var factura = "0060000601";
            var reserva = await _baseDatos.ObtenerReserva(int.Parse(idReserva));
            string? correo = User.FindFirst("correo")?.Value;
            Pago pago = new Pago();
            int idCliente = int.Parse(User.FindFirst("idUsuario")?.Value);
            List<Pago> listaPagos = await _baseDatos.ObtenerPagos(idCliente);
            
            int nro = 0;
            if (listaPagos.Count() == 0)
            {
                nro++;
                pago.pk_tsal001 = factura + nro.ToString().PadLeft(10, '0');
            }
            else
            {
                string ultimoPago = listaPagos.LastOrDefault().pk_tsal001;
                pago.pk_tsal001 = factura + (long.Parse(ultimoPago.Substring(ultimoPago.Length - 10)) + 1).ToString().PadLeft(10, '0');
            }
            pago.idReserva = int.Parse(idReserva);
            pago.terminalId = "EMVSBAT1";
            pago.transactionType = "SALE";
            pago.invoice = pago.pk_tsal001.Substring(pago.pk_tsal001.Length - 10);
            pago.totalAmount = reserva.costo.ToString();
            double costo = reserva.costo;
            double impuesto = costo * 0.13;
            pago.taxAmount = impuesto.ToString();
            pago.tipAmount = "0";
            pago.clientEmail = correo;
            pago.idCliente = idCliente;

            return View(pago);
        }

        // POST: PagosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearPago(Pago pago)
        {
            try
            {
                //string URL_SB2B = "http://localhost:62278/api/";
                string URL_SB2B = "http://sb2b.superbaterias.com:44323/api/";
                string token = "";
                using (HttpClient client = new HttpClient())
                {
                    //string url = "http://localhost:62278/generaToken";
                    string url = "http://sb2b.superbaterias.com:44323/generaToken";
                    Token objetoToken = new Token();

                    string jsonToken = JsonConvert.SerializeObject(objetoToken);
                    StringContent contentToken = new StringContent(jsonToken, Encoding.UTF8, "application/json");
                    HttpResponseMessage responsePago = await client.PostAsync(url, contentToken);
                    if (responsePago.IsSuccessStatusCode)
                    {
                        string respuestatoken = await responsePago.Content.ReadAsStringAsync();
                        token = JsonConvert.DeserializeObject<string>(respuestatoken);
                    }
                }
                using (HttpClient cliente = new HttpClient())
                {
                    cliente.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    string link = URL_SB2B + "/bac/open-script-page";

                    string jsonPago = JsonConvert.SerializeObject(pago);
                    StringContent contenido = new StringContent(jsonPago, Encoding.UTF8, "application/json");

                    HttpResponseMessage responsePago = await cliente.PostAsync(link, contenido);
                    if (responsePago.IsSuccessStatusCode)
                    {
                        string respuestaPago = await responsePago.Content.ReadAsStringAsync();
                        UrlPago respuestaLinkPago = JsonConvert.DeserializeObject<UrlPago>(respuestaPago);

                        string scriptPath = @"C:\EqualRP\IntegracionBAC\run-sdk.ps1";
                        string parametro = respuestaLinkPago.URL.ToString();

                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            FileName = "powershell.exe",
                            Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\" \"{parametro}\"",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };

                        using (Process process = new Process { StartInfo = psi })
                        {
                            process.Start();

                            // Captura la salida del proceso (opcional)
                            string output = process.StandardOutput.ReadToEnd();
                            string error = process.StandardError.ReadToEnd();

                            process.WaitForExit();

                            // Muestra la salida o error si lo necesitas
                            Console.WriteLine("Output:");
                            Console.WriteLine(output);
                            Console.WriteLine("Error:");
                            Console.WriteLine(error);
                        }

                        using (HttpClient clientePago = new HttpClient())
                        {
                            clientePago.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                            string linkPago = URL_SB2B + "ConsultaEstado";
                            ConsultaPago consultaPago = new ConsultaPago();
                            consultaPago.pk_tsal001 = pago.pk_tsal001;
                            string jsonConsultaPago = JsonConvert.SerializeObject(consultaPago);
                            StringContent contenidoPago = new StringContent(jsonConsultaPago, Encoding.UTF8, "application/json");
                            //HttpResponseMessage responseConsultaPago = await clientePago.PostAsync(linkPago, contenidoPago);
                            bool seguir = true;
                            while (seguir)
                            {
                                HttpResponseMessage responseConsultaPago = await clientePago.PostAsync(linkPago, contenidoPago);
                                //await Task.Delay(3000);
                                if (responseConsultaPago.IsSuccessStatusCode)
                                {
                                    string respuestaConsulta = await responseConsultaPago.Content.ReadAsStringAsync();
                                    ConsultaPago consulta = JsonConvert.DeserializeObject<ConsultaPago>(respuestaConsulta);
                                    if (consulta.codigo == 1)
                                    {
                                        await _baseDatos.AgregarPago(pago);
                                        Reserva reserva = await _baseDatos.ObtenerReserva(pago.idReserva);
                                        reserva.estado = true;
                                        seguir = false;
                                        await _baseDatos.ActualizarReserva(pago.idReserva);
                                        return RedirectToAction("VerReservas", "Reservas");
                                    }
                                }
                            }
                        }

                        //using (HttpClient cliente2 = new HttpClient())
                        //{
                        //    string link2 = URL_SB2B + "/chrome/open-chrome";
                        //    string jsonPago2 = JsonConvert.SerializeObject(respuestaLinkPago);
                        //    StringContent contenido2 = new StringContent(jsonPago2, Encoding.UTF8, "application/json");

                        //    HttpResponseMessage responseChrome = await cliente2.PostAsync(link2, contenido2);
                        //    if (responsePago.IsSuccessStatusCode)
                        //    {
                        //        string respuestatoken = await responsePago.Content.ReadAsStringAsync();
                        //        //await Task.Delay(30000);
                        //        using (HttpClient clientePago = new HttpClient())
                        //        {
                        //            clientePago.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        //            string linkPago = URL_SB2B + "ConsultaEstado";
                        //            ConsultaPago consultaPago = new ConsultaPago();
                        //            consultaPago.pk_tsal001 = pago.pk_tsal001;
                        //            string jsonConsultaPago = JsonConvert.SerializeObject(consultaPago);
                        //            StringContent contenidoPago = new StringContent(jsonConsultaPago, Encoding.UTF8, "application/json");
                        //            //HttpResponseMessage responseConsultaPago = await clientePago.PostAsync(linkPago, contenidoPago);
                        //            bool seguir = true;
                        //            while (seguir)
                        //            {
                        //                HttpResponseMessage responseConsultaPago = await clientePago.PostAsync(linkPago, contenidoPago);
                        //                await Task.Delay(3000);
                        //                if (responseConsultaPago.IsSuccessStatusCode)
                        //                {
                        //                    string respuestaConsulta = await responseConsultaPago.Content.ReadAsStringAsync();
                        //                    ConsultaPago consulta = JsonConvert.DeserializeObject<ConsultaPago>(respuestaConsulta);
                        //                    if (consulta.codigo == 1)
                        //                    {
                        //                        await _baseDatos.AgregarPago(pago);
                        //                        Reserva reserva = await _baseDatos.ObtenerReserva(pago.idReserva);
                        //                        reserva.estado = true;
                        //                        seguir = false;
                        //                        await _baseDatos.ActualizarReserva(pago.idReserva);
                        //                        return RedirectToAction("VerReservas", "Reservas");
                        //                    }
                        //                }
                        //            }
                        //        }

                        //    }
                        //}
                        //return RedirectToAction("VerReservas", "Reservas");
                        //return View();
                    }
                }
                return RedirectToAction("VerReservas", "Reservas");
            }
            catch
            {
                return View();
            }
        }

        // GET: PagosController
        public async Task<ActionResult> VerComprobante(string idReserva)
        {
            Pago pago = await _baseDatos.ObtenerPago(int.Parse(idReserva));
            Reserva datosReserva = await _baseDatos.ObtenerReserva(int.Parse(idReserva));
            ViewBag.reserva = datosReserva;
            Ruta datosRuta = await _baseDatos.ObtenerRuta(int.Parse(idReserva));
            ViewBag.ruta = datosRuta;
            return View(pago);
        }

        // GET: PagosController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PagosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PagosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PagosController/Create
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

        // GET: PagosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PagosController/Edit/5
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

        // GET: PagosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PagosController/Delete/5
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
