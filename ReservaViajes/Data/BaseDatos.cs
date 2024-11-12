using MySqlConnector;
using ReservaViajes.Models.Buses;
using ReservaViajes.Models.Pagos;
using ReservaViajes.Models.Reservas;
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

        public BaseDatos() { }

        string conexion = "DefaultConnection";

        public static string EncriptaContrasena(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        public virtual async Task<List<Usuario>> obtenerUsuarios()
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
                                correo = lector.GetString("correo"),
                                password = lector.GetString("password")
                            };
                            listaUsuarios.Add(usuario);
                        }
                    }
                }
            }
            return listaUsuarios;
        }

        public virtual async void agregarUsuario(Usuario usuario)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString(conexion);
                using (var conexion = new MySqlConnection(connectionString))
                {
                    await conexion.OpenAsync();
                    var query = "INSERT INTO usuarios (idUsuario, nombre, correo, password) VALUES (@idUsuario, @nombre, @correo, @password);";

                    using (var comando = new MySqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@idUsuario", usuario.idUsuario);
                        comando.Parameters.AddWithValue("@nombre", usuario.nombre);
                        comando.Parameters.AddWithValue("@correo", usuario.correo);
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

        public virtual async Task<bool> CambioContrasena(int idUsuario, string passwordActual, string passwordNuevo)
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

        public virtual async Task<bool> ValidarUsuario(Login login)
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

        public virtual async Task<Usuario?> ObtenerUsuario(Models.Usuarios.Login login)
        {
            var connectionString = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(connectionString))
            {
                await conexion.OpenAsync();

                var passwordEncriptada = EncriptaContrasena(login.password);

                var query = "SELECT idUsuario, nombre, correo FROM usuarios WHERE idUsuario = @idUsuario AND password = @password";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@idUsuario", login.idUsuario);
                    comando.Parameters.AddWithValue("@password", passwordEncriptada);

                    using (var reader = await comando.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var usuario = new Usuario
                            {
                                idUsuario = reader.GetInt32("idUsuario"),
                                nombre = reader.GetString("nombre"),
                                correo = reader.GetString("correo")
                            };
                            return usuario;
                        }
                    }
                }
            }
            return null;
        }

        public virtual async Task<List<Bus>> RetornaBuses()
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

        public virtual async Task<List<Bus>> ObtenerBusesXRuta(int idRuta)
        {
            var listaBuses = new List<Bus>();
            var stringConeccion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConeccion))
            {
                await conexion.OpenAsync();
                var query = "SELECT * FROM buses WHERE idRuta = @idRuta";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("idRuta", idRuta);
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
                                nombreRuta = lector.IsDBNull(lector.GetOrdinal("nombreRuta")) ? null : lector.GetString("nombreRuta")
                            };
                            listaBuses.Add(bus);
                        }
                    }
                }
            }
            return listaBuses;
        }

        public virtual async Task<Bus> ObtenerBus(int idBus)
        {
            var bus = new Bus();
            var stringConeccion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConeccion))
            {
                await conexion.OpenAsync();
                var query = "SELECT * FROM buses WHERE idBus = @idBus";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("idBus", idBus);
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            bus = new Bus
                            {
                                idBus = lector.GetInt32("idBus"),
                                nombre = lector.GetString("nombre"),
                                placa = lector.GetString("placa"),
                                asientos = lector.GetInt32("asientos"),
                                idRuta = lector.GetInt32("idRuta"),
                                nombreRuta = lector.IsDBNull(lector.GetOrdinal("nombreRuta")) ? null : lector.GetString("nombreRuta")
                            };
                        }
                    }
                }
            }
            return bus;
        }

        public virtual async Task<Bus> ObtenerBusPorId(int idBus)
        {
            var stringConeccion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConeccion))
            {
                await conexion.OpenAsync();
                var query = "SELECT * FROM buses WHERE idBus = @idBus";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@idBus", idBus);
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        if (await lector.ReadAsync())
                        {
                            return new Bus
                            {
                                idBus = lector.GetInt32("idBus"),
                                nombre = lector.GetString("nombre"),
                                placa = lector.GetString("placa"),
                                asientos = lector.GetInt32("asientos"),
                                idRuta = lector.IsDBNull(lector.GetOrdinal("idRuta")) ? 0 : lector.GetInt32("idRuta"),
                                nombreRuta = lector.IsDBNull(lector.GetOrdinal("nombreRuta")) ? null : lector.GetString("nombreRuta")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public virtual async Task agregarBus(Bus bus)
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

        public virtual async Task<List<Ruta>> ObtenerRutas()
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
                                fechaRuta = lector.GetDateTime("fechaRuta"),
                                costo = lector.GetDecimal("costo")
                            };
                            listaRutas.Add(ruta);
                        }
                    }
                }
            }
            return listaRutas;
        }

        public virtual async Task<Ruta> ObtenerRuta(int idRuta)
        {
            var Ruta = new Ruta();
            var stringConeccion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConeccion))
            {
                await conexion.OpenAsync();
                var query = "SELECT * FROM rutas WHERE idRuta = @idRuta";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@idRuta", idRuta);
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
                                fechaRuta = lector.GetDateTime("fechaRuta"),
                                costo = lector.GetDecimal("costo")
                            };
                        }
                    }
                }
            }
            return Ruta;
        }

        public virtual async Task<List<Ruta>> ObtenerRutasFiltradas(string origen)
        {
            var listaRuta = new List<Ruta>();
            var stringConeccion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConeccion))
            {
                await conexion.OpenAsync();
                var query = "SELECT * FROM rutas WHERE origen = @origen";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@origen", origen);
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
                                fechaRuta = lector.GetDateTime("fechaRuta"),
                                costo = lector.GetDecimal("costo")
                            };
                            listaRuta.Add(ruta);
                        }
                    }
                }
            }
            return listaRuta;
        }

        public virtual async Task<List<string>> ObtenerOrigenes()
        {
            var origenes = new List<string>();
            var stringConeccion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConeccion))
            {
                await conexion.OpenAsync();
                var query = "SELECT DISTINCT origen FROM rutas;";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            origenes.Add(lector.GetString("origen"));
                        }
                    }
                }
            }
            return origenes;
        }

        public async Task agregarRuta(Ruta ruta)
        {
            var stringConexion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConexion))
            {
                try
                {
                    await conexion.OpenAsync();
                    var query = "INSERT INTO rutas (idRuta, nombreRuta, origen, destino, fechaRuta, costo) VALUES (@idRuta, @nombreRuta, @origen, @destino, @fechaRuta, @costo);";
                    using (var comando = new MySqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@idRuta", ruta.idRuta);
                        comando.Parameters.AddWithValue("@nombreRuta", ruta.nombreRuta);
                        comando.Parameters.AddWithValue("@origen", ruta.origen);
                        comando.Parameters.AddWithValue("@destino", ruta.destino);
                        comando.Parameters.AddWithValue("@fechaRuta", ruta.fechaRuta);
                        comando.Parameters.AddWithValue("@costo", ruta.costo);

                        await comando.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception e)
                {

                    throw new Exception("Error al guardar los datos de la ruta. " + e.Message);
                }
            }
        }

        public virtual async Task<List<Reserva>> ObtenerReservas()
        {
            var listaReservas = new List<Reserva>();
            var stringConeccion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConeccion))
            {
                await conexion.OpenAsync();
                var query = "SELECT * FROM reservas";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            //string asientosString = lector.IsDBNull(lector.GetOrdinal("asientosSeleccionados")) ? "" : lector.GetString("asientosSeleccionados");

                            string asientosString = lector.IsDBNull(lector.GetOrdinal("asientosSeleccionados"))
                                ? ""
                                : lector.GetString("asientosSeleccionados");

                            List<int> asientosSeleccionados = asientosString
                                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(int.Parse)
                                .ToList();
                            var reserva = new Reserva
                            {
                                idReserva = lector.IsDBNull(lector.GetOrdinal("idReserva")) ? 0 : lector.GetInt32("idReserva"),
                                idUsuario = lector.IsDBNull(lector.GetOrdinal("idUsuario")) ? 0 : lector.GetInt32("idUsuario"),
                                nombreUsuario = lector.IsDBNull(lector.GetOrdinal("nombreUsuario")) ? "" : lector.GetString("nombreUsuario"),
                                idBus = lector.IsDBNull(lector.GetOrdinal("idBus")) ? 0 : lector.GetInt32("idBus"),
                                nombreBus = lector.IsDBNull(lector.GetOrdinal("nombreBus")) ? "" : lector.GetString("nombreBus"),
                                idRuta = lector.IsDBNull(lector.GetOrdinal("idRuta")) ? 0 : lector.GetInt32("idRuta"),
                                nombreRuta = lector.IsDBNull(lector.GetOrdinal("nombreRuta")) ? "" : lector.GetString("nombreRuta"),
                                asientosSeleccionados = asientosSeleccionados,
                                costo = lector.IsDBNull(lector.GetOrdinal("costo")) ? 0 : lector.GetInt32("costo"),
                                estado = lector.IsDBNull(lector.GetOrdinal("estado")) ? false : lector.GetBoolean("estado")
                            };
                            listaReservas.Add(reserva);
                        }
                    }
                }
            }
            return listaReservas;
        }

        public virtual async Task AgregarReserva(Reserva reserva)
        {
            var stringConexion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConexion))
            {
                try
                {
                    await conexion.OpenAsync();
                    string asientosSeleccionados = string.Join(",", reserva.asientosSeleccionados);
                    var query = "INSERT INTO reservas (idReserva, idUsuario, nombreUsuario, idBus, nombreBus, idRuta, nombreRuta, asientosSeleccionados, costo, estado)" +
                        "VALUES (@idReserva, @idUsuario, @nombreUsuario, @idBus, @nombreBus, @idRuta, @nombreRuta, @asientosSeleccionados, @costo, @estado);";
                    using (var comando = new MySqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@idReserva", reserva.idReserva);
                        comando.Parameters.AddWithValue("@idUsuario", reserva.idUsuario);
                        comando.Parameters.AddWithValue("@nombreUsuario", reserva.nombreUsuario);
                        comando.Parameters.AddWithValue("@idBus", reserva.idBus);
                        comando.Parameters.AddWithValue("@nombreBus", reserva.nombreBus);
                        comando.Parameters.AddWithValue("@idRuta", reserva.idRuta);
                        comando.Parameters.AddWithValue("@nombreRuta", reserva.nombreRuta);
                        comando.Parameters.AddWithValue("@asientosSeleccionados", asientosSeleccionados);
                        comando.Parameters.AddWithValue("@costo", reserva.costo);
                        comando.Parameters.AddWithValue("@estado", reserva.estado);

                        await comando.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception e)
                {

                    throw new Exception("Error al guardar los datos de la reserva. " + e.Message);
                }
            }
        }

        public virtual async Task EliminarReserva(int idReserva)
        {
            var stringConexion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConexion))
            {
                try
                {
                    await conexion.OpenAsync();
                    var query = "DELETE FROM reservas WHERE idReserva = @idReserva;";
                    using (var comando = new MySqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@idReserva", idReserva);

                        await comando.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Error al eliminar la reserva. " + e.Message);
                }
            }
        }

        public virtual async Task ActualizarReserva(int idReserva)
        {
            var stringConexion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConexion))
            {
                try
                {
                    await conexion.OpenAsync();
                    var query = "UPDATE reservas SET estado = @estado WHERE idReserva = @idReserva;";
                    using (var comando = new MySqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@estado", 1);
                        comando.Parameters.AddWithValue("@idReserva", idReserva);

                        await comando.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception e)
                {

                    throw new Exception("Error al actualizar el estado de la reserva. " + e.Message);
                }
            }
        }

        public virtual async Task<Reserva> ObtenerReserva(int idReserva)
        {
            Reserva reserva = new Reserva();
            var stringConeccion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConeccion))
            {
                await conexion.OpenAsync();
                var query = "SELECT * FROM reservas WHERE idReserva = @idReserva";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@idReserva", idReserva);
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            string asientosString = lector.IsDBNull(lector.GetOrdinal("asientosSeleccionados")) ? "" : lector.GetString("asientosSeleccionados");
                            List<int> asientosSeleccionados = asientosString
                                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(int.Parse)
                                .ToList();

                            reserva = new Reserva
                            {
                                idReserva = lector.GetInt32("idReserva"),
                                idUsuario = lector.GetInt32("idUsuario"),
                                nombreUsuario = lector.GetString("nombreUsuario"),
                                idBus = lector.GetInt32("idBus"),
                                nombreBus = lector.GetString("nombreBus"),
                                idRuta = lector.GetInt32("idRuta"),
                                nombreRuta = lector.GetString("nombreRuta"),
                                asientosSeleccionados = asientosSeleccionados,
                                costo = lector.GetInt32("costo")
                            };
                        }
                    }
                }
            }
            return reserva;
        }

        public virtual async Task<List<int>> ObtenerAsientosOcupados(int idBus)
        {
            var asientosOcupados = new List<int>();
            var stringConeccion = _configuration.GetConnectionString(conexion);

            using (var conexion = new MySqlConnection(stringConeccion))
            {
                await conexion.OpenAsync();
                var query = "SELECT asientosSeleccionados FROM reservas WHERE idBus = @idBus";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@idBus", idBus);
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            string asientosSeleccionados = lector.GetString("asientosSeleccionados");
                            asientosOcupados.AddRange(asientosSeleccionados.Split(',').Select(int.Parse));
                        }
                    }
                }
            }
            return asientosOcupados;
        }

        public virtual async Task<List<Pago>> ObtenerPagos(int idCliente)
        {
            var listaPagos = new List<Pago>();
            var stringConeccion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConeccion))
            {
                await conexion.OpenAsync();
                var query = "SELECT * FROM pagos";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            var pago = new Pago
                            {
                                pk_tsal001 = lector.IsDBNull(lector.GetOrdinal("pk_tsal001")) ? "" : lector.GetString("pk_tsal001"),
                                idReserva = lector.IsDBNull(lector.GetInt32("idReserva")) ? 0 : lector.GetInt32("idReserva"),
                                terminalId = lector.IsDBNull(lector.GetOrdinal("terminalId")) ? "" : lector.GetString("terminalId"),
                                transactionType = lector.IsDBNull(lector.GetOrdinal("transactionType")) ? "" : lector.GetString("transactionType"),
                                invoice = lector.IsDBNull(lector.GetOrdinal("invoice")) ? "" : lector.GetString("invoice"),
                                totalAmount = lector.IsDBNull(lector.GetOrdinal("totalAmount")) ? "" : lector.GetString("totalAmount"),
                                taxAmount = lector.IsDBNull(lector.GetOrdinal("taxAmount")) ? "" : lector.GetString("taxAmount"),
                                tipAmount = lector.IsDBNull(lector.GetOrdinal("tipAmount")) ? "" : lector.GetString("tipAmount"),
                                clientEmail = lector.IsDBNull(lector.GetOrdinal("clientEmail")) ? "" : lector.GetString("clientEmail"),
                                idCliente = lector.IsDBNull(lector.GetOrdinal("idCliente")) ? 0 : lector.GetInt32("idCliente")
                            };
                            listaPagos.Add(pago);
                        }
                    }
                }
            }
            return listaPagos;
        }

        public virtual async Task AgregarPago(Pago pago)
        {
            var stringConexion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConexion))
            {
                try
                {
                    await conexion.OpenAsync();

                    var query = "INSERT INTO pagos (pk_tsal001, idReserva, terminalId, transactionType, invoice, totalAmount, taxAmount, tipAmount, clientEmail, idCliente)" +
                        "VALUES (@pk_tsal001, @idReserva, @terminalId, @transactionType, @invoice, @totalAmount, @taxAmount, @tipAmount, @clientEmail, @idCliente);";
                    using (var comando = new MySqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@pk_tsal001", pago.pk_tsal001);
                        comando.Parameters.AddWithValue("@idReserva", pago.idReserva);
                        comando.Parameters.AddWithValue("@terminalId", pago.terminalId);
                        comando.Parameters.AddWithValue("@transactionType", pago.transactionType);
                        comando.Parameters.AddWithValue("@invoice", pago.invoice);
                        comando.Parameters.AddWithValue("@totalAmount", pago.totalAmount);
                        comando.Parameters.AddWithValue("@taxAmount", pago.taxAmount);
                        comando.Parameters.AddWithValue("@tipAmount", pago.tipAmount);
                        comando.Parameters.AddWithValue("@clientEmail", pago.clientEmail);
                        comando.Parameters.AddWithValue("@idCliente", pago.idCliente);

                        await comando.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception e)
                {

                    throw new Exception("Error al guardar los datos de la reserva. " + e.Message);
                }
            }
        }

        public virtual async Task<Pago> ObtenerPago(int idReserva)
        {
            var pago = new Pago();
            var stringConeccion = _configuration.GetConnectionString(conexion);
            using (var conexion = new MySqlConnection(stringConeccion))
            {
                await conexion.OpenAsync();
                var query = "SELECT * FROM pagos WHERE idReserva = @idReserva ORDER BY pk_tsal001 DESC LIMIT 1;";
                using (var comando = new MySqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@idReserva", idReserva);
                    using (var lector = await comando.ExecuteReaderAsync())
                    {
                        while (await lector.ReadAsync())
                        {
                            pago = new Pago
                            {
                                pk_tsal001 = lector.IsDBNull(lector.GetOrdinal("pk_tsal001")) ? "" : lector.GetString("pk_tsal001"),
                                idReserva = lector.IsDBNull(lector.GetInt32("idReserva")) ? 0 : lector.GetInt32("idReserva"),
                                terminalId = lector.IsDBNull(lector.GetOrdinal("terminalId")) ? "" : lector.GetString("terminalId"),
                                transactionType = lector.IsDBNull(lector.GetOrdinal("transactionType")) ? "" : lector.GetString("transactionType"),
                                invoice = lector.IsDBNull(lector.GetOrdinal("invoice")) ? "" : lector.GetString("invoice"),
                                totalAmount = lector.IsDBNull(lector.GetOrdinal("totalAmount")) ? "" : lector.GetString("totalAmount"),
                                taxAmount = lector.IsDBNull(lector.GetOrdinal("taxAmount")) ? "" : lector.GetString("taxAmount"),
                                tipAmount = lector.IsDBNull(lector.GetOrdinal("tipAmount")) ? "" : lector.GetString("tipAmount"),
                                clientEmail = lector.IsDBNull(lector.GetOrdinal("clientEmail")) ? "" : lector.GetString("clientEmail"),
                                idCliente = lector.IsDBNull(lector.GetOrdinal("idCliente")) ? 0 : lector.GetInt32("idCliente")
                            };
                        }
                    }
                }
            }
            return pago;
        }

    }
}
