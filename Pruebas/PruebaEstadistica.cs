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
using System.Transactions;
using Xunit;

public class PruebaEstadisticasDao
{
    [Fact]
    public void ObtenerEstadisticasGlobales_DebeRetornarListaOrdenadaPorVictorias()
    {
        using (var scope = new TransactionScope())
        {
            using (var contexto = new ContextoBaseDatos())
            {
                contexto.Estadisticas.Add(new Estadisticas { IdJugador = 1, NumeroVictorias = 10 });
                contexto.Estadisticas.Add(new Estadisticas { IdJugador = 2, NumeroVictorias = 20 });
                contexto.SaveChanges();
            }

            var dao = new EstadisticasDao();
        
            var estadisticas = dao.ObtenerEstadisticasGlobales();

            // Assert
            Assert.NotNull(estadisticas);
            Assert.True(estadisticas.Count >= 2);
            Assert.True(estadisticas[0].NumeroVictorias >= estadisticas[1].NumeroVictorias);
        }
    }
    [Fact]
    public void ObtenerEstadisticasGlobales_DebeLanzarExcepcion_CuandoOcurreEntityException()
    {
        var mockSet = new Mock<DbSet<Estadisticas>>();

        mockSet.Setup(m => m.ToList()).Throws(new EntityException("Error simulado"));

        var mockContexto = new Mock<ContextoBaseDatos>();

        mockContexto.Setup(c => c.Estadisticas).Returns(mockSet.Object);

        var dao = new EstadisticasDao();

        var ex = Assert.Throws<ExcepcionAccesoDatos>(() => dao.ObtenerEstadisticasGlobales());
        Assert.Contains("Error simulado", ex.Message); 
    }


    [Fact]
    public void ActualizarVictoriasJugador_DebeIncrementarVictorias_CuandoJugadorExiste()
    {
        using (var scope = new TransactionScope())
        {
            using (var contexto = new ContextoBaseDatos())
            {
                contexto.Estadisticas.Add(new Estadisticas { IdJugador = 1, NumeroVictorias = 5 });
                contexto.SaveChanges();
            }

            var dao = new EstadisticasDao();

            var filasAfectadas = dao.ActualizarVictoriasJugador(1);

            Assert.Equal(1, filasAfectadas);

            using (var contexto = new ContextoBaseDatos())
            {
                var estadisticas = contexto.Estadisticas.FirstOrDefault(e => e.IdJugador == 1);
                Assert.NotNull(estadisticas);
                Assert.Equal(6, estadisticas.NumeroVictorias);
            }
        }
    }

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
    public void ActualizarVictoriasJugador_DebeLanzarExcepcion_CuandoOcurreEntityException()
    { 
        var mockSet = new Mock<DbSet<Estadisticas>>();

        mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Estadisticas, bool>>()))
               .Throws(new EntityException("Error simulado"));

        var mockContexto = new Mock<ContextoBaseDatos>();

        mockContexto.Setup(c => c.Estadisticas).Returns(mockSet.Object);

        var dao = new EstadisticasDao();

        var ex = Assert.Throws<ExcepcionAccesoDatos>(() => dao.ActualizarVictoriasJugador(1));
        Assert.Contains("Error simulado", ex.Message); 
    }

    [Fact]
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


    [Fact]
    public void ActualizarVictoriasJugador_DebeNoHacerNada_CuandoJugadorNoExiste()
    {
        var dao = new EstadisticasDao();
        int idJugadorInexistente = 9999;
        var filasAfectadas = dao.ActualizarVictoriasJugador(idJugadorInexistente);
        Assert.Equal(-1, filasAfectadas);
    }

    [Fact]
    public void ActualizarVictoriasJugador_NoDebeLanzarExcepcion_CuandoLaBaseDeDatosEstaVacia()
    {
        var dao = new EstadisticasDao();
        int idJugador = 1;
        var filasAfectadas = dao.ActualizarVictoriasJugador(idJugador);
        Assert.Equal(1, filasAfectadas);
    }

    [Fact]
    public void ActualizarVictoriasJugador_DebeLanzarExcepcion_CuandoOcurreExcepcionGeneral()
    {
        var mockSet = new Mock<DbSet<Estadisticas>>();
        mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Estadisticas, bool>>()))
               .Throws(new Exception("Simulando una excepción general"));

        var mockContexto = new Mock<ContextoBaseDatos>();
        mockContexto.Setup(c => c.Estadisticas).Returns(mockSet.Object);

        var dao = new EstadisticasDao();
        var ex = Assert.Throws<ExcepcionAccesoDatos>(() => dao.ActualizarVictoriasJugador(1));
        Assert.Contains("Simulando una excepción general", ex.Message);
    }

    [Fact]
    public void ObtenerEstadisticasGlobales_DebeLanzarExcepcion_CuandoOcurreExcepcionGeneral()
    {
        var mockSet = new Mock<DbSet<Estadisticas>>();
        mockSet.Setup(m => m.ToList()).Throws(new Exception("Error general"));

        var mockContexto = new Mock<ContextoBaseDatos>();
        mockContexto.Setup(c => c.Estadisticas).Returns(mockSet.Object);

        var dao = new EstadisticasDao();
        var ex = Assert.Throws<ExcepcionAccesoDatos>(() => dao.ObtenerEstadisticasGlobales());
        Assert.Contains("Error general", ex.Message);
    }

    [Fact]
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

}
