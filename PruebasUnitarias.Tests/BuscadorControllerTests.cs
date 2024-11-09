using Microsoft.AspNetCore.Mvc;
using Moq;
using ReservaViajes.Controllers;
using ReservaViajes.Data;
using ReservaViajes.Models.Rutas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasUnitarias.Tests
{
    
    public class BuscadorControllerTests
    {
        [Fact]
        public async Task ObtenerRutasFiltro_Test()
        {
            // Arrange
            var mockBaseDatos = new Mock<BaseDatos>();
            var origen = "San José";
            var rutas = new List<Ruta>
            {
                new Ruta { idRuta = 1, nombreRuta = "Ruta 1", origen = origen, destino = "Alajuela", costo = 1500, fechaRuta = DateTime.Now },
                new Ruta { idRuta = 2, nombreRuta = "Ruta 2", origen = origen, destino = "Cartago", costo = 2000, fechaRuta = DateTime.Now }
            };

            mockBaseDatos.Setup(db => db.ObtenerRutasFiltradas(origen)).ReturnsAsync(rutas);
            var controller = new BuscadorController(mockBaseDatos.Object);

            // Act
            var result = await controller.ObtenerRutasFiltro(origen) as PartialViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("_RutasFiltradas", result.ViewName);
            Assert.Equal(rutas, result.Model);
        }
    }
}
