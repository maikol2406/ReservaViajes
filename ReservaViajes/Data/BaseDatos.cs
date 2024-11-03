using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MySqlConnector;
using NuGet.Protocol.Plugins;
using ReservaViajes.Models.Usuarios;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ReservaViajes.Data
{
    public class BaseDatos
    {
        private readonly IConfiguration _configuration;

        public BaseDatos(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        string conexion = "DefaultConnection";

        public static string EncriptaContrasena(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        public async Task<List<Usuario>> obtenerUsuarios()
        {
            var listaUsuarios = new List<Usuario>();

            var connectionString = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(connectionString))
            {
                await conexion.OpenAsync();
                var query = "SELECT * FROM usuarios;";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            var usuario = new Usuario
                            {
                                idUsuario = lector.GetInt32("idUsuario"),
                                nombre = lector.GetString("nombre"),
                                password = lector.GetString("password")
                            };
                            listaUsuarios.Add(usuario);
                        }
                    }
                }
            }
            return listaUsuarios;
        }

        public async void agregarUsuario(Usuario usuario)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString(conexion);
                using (var conexion = new MySqlConnection(connectionString))
                {
                    await conexion.OpenAsync();
                    var query = "INSERT INTO usuarios (idUsuario, nombre, password) VALUES (@idUsuario, @nombre, @password);";

                    using (var comando = new MySqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@idUsuario", usuario.idUsuario);
                        comando.Parameters.AddWithValue("@nombre", usuario.nombre);
                        comando.Parameters.AddWithValue("@password", EncriptaContrasena(usuario.password));

                        await comando.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception e)
            {

                throw new Exception("Error al guardar los datos del usuario. " + e.Message);
            }
        }

        public async Task<bool> CambioContrasena(int idUsuario, string passwordActual, string passwordNuevo)
        {
            var connectionString = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(connectionString))
            {
                await conexion.OpenAsync();

                var queryConsulta = "SELECT password FROM usuarios WHERE idUsuario = @idUsuario";
                using (var comando = new MySqlCommand(queryConsulta, conexion))
                {
                    comando.Parameters.AddWithValue("@idUsuario", idUsuario);
                    var passIngresado = EncriptaContrasena(passwordActual);
                    var passAlmacenado = (string)await comando.ExecuteScalarAsync();

                    if (passAlmacenado != passIngresado)
                    {
                        return false;
                    }
                }

                var queryInsertar = "UPDATE usuarios SET password = @nuevaContrasena WHERE idUsuario = @idUsuario";
                using (var comandoInsertar = new MySqlCommand(queryInsertar, conexion))
                {
                    comandoInsertar.Parameters.AddWithValue("@idUsuario", idUsuario);
                    comandoInsertar.Parameters.AddWithValue("@nuevaContrasena", EncriptaContrasena(passwordNuevo));
                    await comandoInsertar.ExecuteNonQueryAsync();
                }
            }

            return true;
        }

        public async Task<bool> ValidarUsuario(Models.Usuarios.Login login)
        {
            var connectionString = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(connectionString))
            {
                await conexion.OpenAsync();

                var passwordEncriptada = EncriptaContrasena(login.password);

                var query = "SELECT COUNT(1) FROM usuarios WHERE idUsuario = @idUsuario AND password = @password";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@idUsuario", login.idUsuario);
                    comando.Parameters.AddWithValue("@password", passwordEncriptada);

                    var resultado = (long)await comando.ExecuteScalarAsync();
                    return resultado > 0;
                }
            }
        }

        public async Task<Usuario?> ObtenerUsuario(Models.Usuarios.Login login)
        {
            var connectionString = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(connectionString))
            {
                await conexion.OpenAsync();

                var passwordEncriptada = EncriptaContrasena(login.password);

                var query = "SELECT idUsuario, nombre FROM usuarios WHERE idUsuario = @idUsuario AND password = @password";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@idUsuario", login.idUsuario);
                    comando.Parameters.AddWithValue("@password", passwordEncriptada);

                    using (var reader = await comando.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Usuario
                            {
                                idUsuario = reader.GetInt32("idUsuario"),
                                nombre = reader.GetString("nombre")
                            };
                        }
                    }
                }
            }
            return null;
        }


    }
}
