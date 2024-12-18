using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using AccesoDatos.DAO;
using AccesoDatos.Excepciones;
using AccesoDatos.Modelo;
using AccesoDatos;
using System.Linq;
using Moq;
using Xunit;
using System.Data.SqlClient;
public class EstadisticasDaoTests
{
    private readonly Mock<ContextoBaseDatos> _mockContexto;
    private readonly EstadisticasDao _dao;

    public EstadisticasDaoTests()
    {
        _mockContexto = new Mock<ContextoBaseDatos>();
        _dao = new EstadisticasDao();
    }


    [Fact]
    public void ObtenerEstadisticasGlobales_DeberiaLanzarExcepcionEntityException()
    {
        _mockContexto.Setup(ctx => ctx.Estadisticas).Throws(new EntityException("Error de entidad"));

        Assert.Throws<ExcepcionAccesoDatos>(() => _dao.ObtenerEstadisticasGlobales());
    }


    [Fact]
    public void ObtenerEstadisticasGlobales_DeberiaLanzarExcepcionGenerica()
    {
        _mockContexto.Setup(ctx => ctx.Estadisticas).Throws(new Exception("Error inesperado"));

        Assert.Throws<ExcepcionAccesoDatos>(() => _dao.ObtenerEstadisticasGlobales());
    }

    [Fact]
    public void ActualizarVictoriasJugador_DeberiaActualizarVictoriasDeJugadorExistente()
    {
        var idJugador = 1;
        var estadisticasExistentes = new Estadisticas { IdJugador = idJugador, NumeroVictorias = 5 };

        _mockContexto.Setup(ctx => ctx.Estadisticas.FirstOrDefault(It.IsAny<Func<Estadisticas, bool>>()))
            .Returns(estadisticasExistentes);

        var filasAfectadas = _dao.ActualizarVictoriasJugador(idJugador);

        Assert.Equal(1, filasAfectadas);
        Assert.Equal(6, estadisticasExistentes.NumeroVictorias);
    }

    [Fact]
    public void ActualizarVictoriasJugador_DeberiaAgregarNuevaEstadisticaSiJugadorNoExiste()
    {
        var idJugador = 2;
        _mockContexto.Setup(ctx => ctx.Estadisticas.FirstOrDefault(It.IsAny<Func<Estadisticas, bool>>()))
            .Returns((Estadisticas)null);

        var filasAfectadas = _dao.ActualizarVictoriasJugador(idJugador);

        Assert.Equal(1, filasAfectadas);
    }

    [Fact]
    public void ActualizarVictoriasJugador_DeberiaNoHacerNadaSiIdJugadorEsInvalido()
    {
        var idJugador = 0;

        var filasAfectadas = _dao.ActualizarVictoriasJugador(idJugador);

        Assert.Equal(-1, filasAfectadas);
    }

    [Fact]
    public void ActualizarVictoriasJugador_DeberiaLanzarExcepcionEntityException()
    {
        var idJugador = 1;
        _mockContexto.Setup(ctx => ctx.Estadisticas.FirstOrDefault(It.IsAny<Func<Estadisticas, bool>>()))
            .Throws(new EntityException("Error de entidad"));

        Assert.Throws<ExcepcionAccesoDatos>(() => _dao.ActualizarVictoriasJugador(idJugador));
    }

    

    [Fact]
    public void ActualizarVictoriasJugador_DeberiaLanzarExcepcionGenerica()
    {
        var idJugador = 1;
        _mockContexto.Setup(ctx => ctx.Estadisticas.FirstOrDefault(It.IsAny<Func<Estadisticas, bool>>()))
            .Throws(new Exception("Error inesperado"));

        Assert.Throws<ExcepcionAccesoDatos>(() => _dao.ActualizarVictoriasJugador(idJugador));
    }
}
