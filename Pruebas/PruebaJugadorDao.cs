using AccesoDatos;
using AccesoDatos.DAO;
using AccesoDatos.Modelo;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Xunit;

public class PruebaJugadorDao
{
    private readonly Mock<DbSet<Jugador>> _jugadoresMock;
    private readonly Mock<ContextoBaseDatos> _contextoMock;
    private readonly JugadorDao _jugadorDao;

    public PruebaJugadorDao()
    {
        _jugadoresMock = new Mock<DbSet<Jugador>>();
        _contextoMock = new Mock<ContextoBaseDatos>();
        _contextoMock.Setup(c => c.Jugadores).Returns(_jugadoresMock.Object);
        _jugadorDao = new JugadorDao();
    }

    [Fact]
    public void ObtenerJugador_DebeRetornarJugador_SiExiste()
    {
        // Configuración
        var jugadorId = 1;
        var jugadorEsperado = new Jugador { JugadorId = jugadorId, NombreUsuario = "TestUser" };

        var data = new List<Jugador> { jugadorEsperado }.AsQueryable();
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.Provider).Returns(data.Provider);
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.Expression).Returns(data.Expression);
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.ElementType).Returns(data.ElementType);
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        // Ejecución
        var resultado = _jugadorDao.ObtenerJugador(jugadorId);

        // Verificación
        Assert.NotNull(resultado);
        Assert.Equal(jugadorEsperado.JugadorId, resultado.JugadorId);
        Assert.Equal(jugadorEsperado.NombreUsuario, resultado.NombreUsuario);
    }

    [Fact]
    public void ObtenerJugador_DebeRetornarNull_SiNoExiste()
    {
        // Configuración
        var jugadorId = 99; // ID que no existe en los datos de prueba

        // Ejecución
        var resultado = _jugadorDao.ObtenerJugador(jugadorId);

        // Verificación
        Assert.Null(resultado);
    }

    [Fact]
    public void EditarNombreUsuario_DebeRetornarTrue_SiActualizacionEsExitosa()
    {
        // Configuración
        var jugadorId = 1;
        var jugador = new Jugador { JugadorId = jugadorId, NombreUsuario = "AntiguoNombre" };

        var data = new List<Jugador> { jugador }.AsQueryable();
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.Provider).Returns(data.Provider);
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.Expression).Returns(data.Expression);
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.ElementType).Returns(data.ElementType);
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        _contextoMock.Setup(c => c.SaveChanges()).Returns(1); // Simulamos que SaveChanges afecta una fila

        // Ejecución
        var resultado = _jugadorDao.EditarNombreUsuario(jugadorId, "NuevoNombre");

        // Verificación
        Assert.True(resultado);
        Assert.Equal("NuevoNombre", jugador.NombreUsuario);
    }

    [Fact]
    public void EditarNombreUsuario_DebeRetornarFalse_SiNoSeEncuentraJugador()
    {
        // Configuración
        var jugadorId = 99; // ID que no existe en los datos de prueba

        // Ejecución
        var resultado = _jugadorDao.EditarNombreUsuario(jugadorId, "NuevoNombre");

        // Verificación
        Assert.False(resultado);
    }

    [Fact]
    public void EditarNombreUsuario_DebeRetornarFalse_SiHayExcepcionEnSaveChanges()
    {
        // Configuración
        var jugadorId = 1;
        var jugador = new Jugador { JugadorId = jugadorId, NombreUsuario = "AntiguoNombre" };

        var data = new List<Jugador> { jugador }.AsQueryable();
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.Provider).Returns(data.Provider);
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.Expression).Returns(data.Expression);
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.ElementType).Returns(data.ElementType);
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        _contextoMock.Setup(c => c.SaveChanges()).Throws(new DbUpdateException()); // Simulamos una excepción

        // Ejecución
        var resultado = _jugadorDao.EditarNombreUsuario(jugadorId, "NuevoNombre");

        // Verificación
        Assert.False(resultado);
    }
}
