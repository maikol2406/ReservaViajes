using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReservaViajes.Controllers;
using ReservaViajes.Data;
using ReservaViajes.Models.Buses;
using ReservaViajes.Models.Reservas;
using ReservaViajes.Models.Rutas;
using ReservaViajes.Models.Usuarios;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

public class ReservasControllerTests
{
    [Fact]
    public async Task CrearReservas_Test()
    {
        // Arrange
        var mockBaseDatos = new Mock<BaseDatos>();

        // Datos simulados
        var idRuta = "1";
        var reservaMock = new Reserva { idReserva = 1, nombreRuta = "Naranjo - San José", idUsuario = 1 };
        var listaReservas = new List<Reserva> { reservaMock };
        var rutas = new List<Ruta> { new Ruta { idRuta = 1, nombreRuta = "Naranjo - San José", origen = "Naranjo", destino = "San José", costo = 1500.00M } };

        // Configuración de los métodos del mock
        mockBaseDatos.Setup(db => db.ObtenerReservas()).ReturnsAsync(listaReservas);
        mockBaseDatos.Setup(db => db.ObtenerRutas()).ReturnsAsync(rutas);
        mockBaseDatos.Setup(db => db.ObtenerRuta(It.IsAny<int>())).ReturnsAsync(new Ruta
        {
            idRuta = 1,
            nombreRuta = "Naranjo - San José",
            origen = "Naranjo",
            destino = "San José",
            fechaRuta = DateTime.Parse("2024-11-04 08:15:00"),
            costo = 1500.00m
        });
        mockBaseDatos.Setup(db => db.ObtenerBus(It.IsAny<int>())).ReturnsAsync(new Bus { idBus = 1, nombre = "Bus A", placa = "XYZ123", asientos = 40 });
        mockBaseDatos.Setup(db => db.AgregarReserva(It.IsAny<Reserva>())).Returns(Task.CompletedTask);

        // Creación del controlador con el mock de BaseDatos
        var controller = new ReservasController(mockBaseDatos.Object);

        // Simula un usuario autenticado con el idUsuario
        var claims = new List<Claim> { new Claim("idUsuario", "1") };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act
        var result = await controller.CrearReservas(idRuta) as ViewResult;

        // Assert
        Assert.NotNull(result);
        var model = result.Model as Reserva;
        Assert.NotNull(model);
        Assert.Equal("Naranjo - San José", model.nombreRuta);
    }

}
