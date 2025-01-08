using AccesoDatos;
using AccesoDatos.DAO;
using AccesoDatos.Excepciones;
using AccesoDatos.Modelo;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using Xunit;

public class PruebaEstadisticasDao
{

    [Fact]
    public void ActualizarVictoriasJugador_DebeAgregarNuevoRegistro_CuandoJugadorNoExiste()
    {
        using (var scope = new TransactionScope())
        {
            int idJugador;
            using (var contexto = new ContextoBaseDatos())
            {
                var jugador = new Jugador
                {
                    NombreUsuario = "NuevoJugador",
                    NumeroFotoPerfil = 1
                };

                contexto.Jugadores.Add(jugador);
                contexto.SaveChanges();
                idJugador = jugador.JugadorId;
            }

            var dao = new EstadisticasDao();

            var filasAfectadas = dao.ActualizarVictoriasJugador(idJugador);

            Assert.Equal(1, filasAfectadas);

            using (var contexto = new ContextoBaseDatos())
            {
                var estadisticas = contexto.Estadisticas.FirstOrDefault(e => e.IdJugador == idJugador);
                Assert.NotNull(estadisticas);
                Assert.Equal(1, estadisticas.NumeroVictorias);
            }
        }
    }

    [Fact]
    public void ActualizarVictoriasJugador_DebeRetornarMenosUno_CuandoIdJugadorNoEsValido()
    {
        var dao = new EstadisticasDao();

        var filasAfectadas = dao.ActualizarVictoriasJugador(-1);

        Assert.Equal(-1, filasAfectadas);
    }


    [Fact]
<<<<<<< HEAD
    public void ObtenerEstadisticasGlobales_DebeRetornarListaVacia_CuandoNoHayEstadisticas()
    {
        using (var scope = new TransactionScope())
        {
            var dao = new EstadisticasDao();

            var estadisticas = dao.ObtenerEstadisticasGlobales();

            Assert.NotNull(estadisticas);
            Assert.Empty(estadisticas);
        }
    }


=======
    public void ActualizarVictoriasJugador_DebeInsertarNuevoRegistro_CuandoEsElPrimerRegistro()
    {
        using (var scope = new TransactionScope())
        {
            int idJugador;
            using (var contexto = new ContextoBaseDatos())
            {
                var jugador = new Jugador
                {
                    NombreUsuario = "NuevoJugador",
                    NumeroFotoPerfil = 1
                };
                contexto.Jugadores.Add(jugador);
                contexto.SaveChanges();
                idJugador = jugador.JugadorId;
            }

            var dao = new EstadisticasDao();
            var filasAfectadas = dao.ActualizarVictoriasJugador(idJugador);
            Assert.Equal(1, filasAfectadas);

            using (var contexto = new ContextoBaseDatos())
            {
                var estadisticas = contexto.Estadisticas.FirstOrDefault(e => e.IdJugador == idJugador);
                Assert.NotNull(estadisticas);
                Assert.Equal(1, estadisticas.NumeroVictorias);
            }
        }
    }
>>>>>>> cambios de estandar y codigo
    [Fact]
    public void ObtenerEstadisticasGlobales_DebeRetornarEstadisticas_CuandoLaBaseDeDatosEstaDisponible()
    {
        var mockSet = new Mock<DbSet<Estadisticas>>();
        var data = new List<Estadisticas>
    {
        new Estadisticas { NumeroVictorias = 5 },
        new Estadisticas { NumeroVictorias = 10 }
    }.AsQueryable(); 

<<<<<<< HEAD
    [Fact]
    public void ActualizarVictoriasJugador_DebeLanzarExcepcion_CuandoOcurreExcepcionGeneral()
    {
        var mockSet = new Mock<DbSet<Estadisticas>>();
        mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Estadisticas, bool>>()))
               .Throws(new Exception("Simulando una excepción general"));
=======
        mockSet.As<IQueryable<Estadisticas>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<Estadisticas>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<Estadisticas>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<Estadisticas>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        var mockContexto = new Mock<ContextoBaseDatos>();
        mockContexto.Setup(c => c.Estadisticas).Returns(mockSet.Object);

        var estadisticasDao = new EstadisticasDao();
>>>>>>> cambios de estandar y codigo

        var result = estadisticasDao.ObtenerEstadisticasGlobales();

        Assert.Equal(2, result.Count);  
        Assert.Equal(10, result[0].NumeroVictorias);  
    }





    [Fact]
    public void ActualizarVictoriasJugador_DebeRetornarMinusUno_CuandoElIdJugadorEsNoValido()
    {
        var estadisticasDao = new EstadisticasDao();

        var resultado = estadisticasDao.ActualizarVictoriasJugador(-1);
    
        Assert.Equal(-1, resultado); 
    }

}
