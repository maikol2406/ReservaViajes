using MySqlConnector;
using ReservaViajes.Models.Buses;
using ReservaViajes.Models.Rutas;
using ReservaViajes.Models.Usuarios;
using System.Security.Cryptography;
using System.Text;

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

        public async Task<List<Bus>> RetornaBuses()
        {
            var listaBuses = new List<Bus>();
            var stringConeccion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConeccion))
            {
                await conexion.OpenAsync();
                var query = "SELECT * FROM buses";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            var bus = new Bus
                            {
                                idBus = lector.GetInt32("idBus"),
                                nombre = lector.GetString("nombre"),
                                placa = lector.GetString("placa"),
                                asientos = lector.GetInt32("asientos"),
                                idRuta = lector.GetInt32("idRuta"),
                                //nombreRuta = lector.GetString("nombreRuta")
                                nombreRuta = lector.IsDBNull(lector.GetOrdinal("nombreRuta")) ? null : lector.GetString("nombreRuta")
                            };
                            listaBuses.Add(bus);
                        }
                    }
                }
            }
            return listaBuses;
        }

        public async Task agregarBus(Bus bus)
        {
            var stringConexion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConexion))
            {
                try
                {
                    await conexion.OpenAsync();
                    var query = "INSERT INTO buses (idBus, nombre, placa, asientos, idRuta, nombreRuta) VALUES (@idBus, @nombre, @placa, @asientos, @idRuta, @nombreRuta);";
                    using (var comando = new MySqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@idBus", bus.idBus);
                        comando.Parameters.AddWithValue("@nombre", bus.nombre);
                        comando.Parameters.AddWithValue("@placa", bus.placa);
                        comando.Parameters.AddWithValue("@asientos", bus.asientos);
                        comando.Parameters.AddWithValue("@idRuta", bus.idRuta);
                        comando.Parameters.AddWithValue("@nombreRuta", bus.nombreRuta);

                        await comando.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception e)
                {

                    throw new Exception("Error al guardar los datos del bus. " + e.Message);
                }
            }
        }

        public async Task<List<Ruta>> ObtenerRutas()
        {
            var listaRutas = new List<Ruta>();
            var stringConeccion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConeccion))
            {
                await conexion.OpenAsync();
                var query = "SELECT * FROM rutas";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            var ruta = new Ruta
                            {
                                idRuta = lector.GetInt32("idRuta"),
                                nombreRuta = lector.GetString("nombreRuta"),
                                origen = lector.GetString("origen"),
                                destino = lector.GetString("destino"),
                                fechaRuta = lector.GetDateTime("fechaRuta")
                            };
                            listaRutas.Add(ruta);
                        }
                    }
                }
            }
            return listaRutas;
        }

        public async Task<Ruta> ObtenerRuta(int idRuta)
        {
            var Ruta = new Ruta();
            var stringConeccion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConeccion))
            {
                await conexion.OpenAsync();
                var query = "SELECT * FROM rutas WHERE idRuta = @idRuta";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("idRuta", idRuta);
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            Ruta = new Ruta
                            {
                                idRuta = lector.GetInt32("idRuta"),
                                nombreRuta = lector.GetString("nombreRuta"),
                                origen = lector.GetString("origen"),
                                destino = lector.GetString("destino"),
                                fechaRuta = lector.GetDateTime("fechaRuta")
                            };
                        }
                    }
                }
            }
            return Ruta;
        }

        public async Task agregarRuta(Ruta ruta)
        {
            var stringConexion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConexion))
            {
                try
                {
                    await conexion.OpenAsync();
                    var query = "INSERT INTO rutas (idRuta, nombreRuta, origen, destino, fechaRuta) VALUES (@idRuta, @nombreRuta, @origen, @destino, @fechaRuta);";
                    using (var comando = new MySqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@idRuta", ruta.idRuta);
                        comando.Parameters.AddWithValue("@nombreRuta", ruta.nombreRuta);
                        comando.Parameters.AddWithValue("@origen", ruta.origen);
                        comando.Parameters.AddWithValue("@destino", ruta.destino);
                        comando.Parameters.AddWithValue("@fechaRuta", ruta.fechaRuta);

                        await comando.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception e)
                {

                    throw new Exception("Error al guardar los datos de la ruta. " + e.Message);
                }
            }
        }
    }
}
