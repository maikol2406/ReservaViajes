using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ReservaViajes.Data;
using ReservaViajes.Models.Usuarios;
using System.Security.Claims;

namespace ReservaViajes.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly BaseDatos _baseDatos;

        public UsuariosController(BaseDatos baseDatos)
        {
            _baseDatos = baseDatos;
        }

        // GET: UsuariosController/Create
        public ActionResult Login()
        {
            return View();
        }

        // POST: UsuariosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Login login)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(login);
                }

                bool isValidUser = await _baseDatos.ValidarUsuario(new Models.Usuarios.Login
                {
                    idUsuario = login.idUsuario,
                    password = login.password
                });

                if (!isValidUser)
                {
                    ModelState.AddModelError(string.Empty, "Cédula o contraseña incorrectos.");
                    return RedirectToAction("Index", "Home");
                }

                Usuario? usuario = await _baseDatos.ObtenerUsuario(login);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.nombre)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                return RedirectToAction("Index", "Home");

            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Usuarios");
        }

        // GET: UsuariosController/Create
        public ActionResult Registro()
        {
            ViewBag.pass = true;
            return View();
        }

        // POST: UsuariosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Registro(Usuario usuario)
        {
            try
            {
                await Task.Run(() => _baseDatos.agregarUsuario(usuario));
                return RedirectToAction(nameof(VerUsuarios));
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> VerUsuarios()
        {
            List<Usuario> usuarios = await _baseDatos.obtenerUsuarios();
            return View(usuarios);
        }

        [HttpGet]
        public ActionResult CambiarContrasena(string idUsuario)
        {
            CambioPassword cambioPassword = new CambioPassword();
            cambioPassword.idUsuario = int.Parse(idUsuario);
            return (View(cambioPassword));
        }

        [HttpPost]
        public async Task<IActionResult> CambiarContrasena(CambioPassword cambio) //int idUsuario, string contrasenaActual, string nuevaContrasena)
        {
            bool resultado = await _baseDatos.CambioContrasena(cambio.idUsuario, cambio.ContrasenaActual, cambio.NuevaContrasena);

            if (!resultado)
            {
                ModelState.AddModelError(string.Empty, "La contraseña actual es incorrecta.");
                return View();
            }

            return RedirectToAction(nameof(Login));
        }
    }
}
