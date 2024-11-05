using AccesoDatos;
using AccesoDatos.DAO;
using AccesoDatos.Modelo;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Xunit;

public class PruebaCuentaDao
{
    private readonly Mock<ContextoBaseDatos> _contextMock;
    private readonly Mock<DbSet<Cuenta>> _cuentasMock;
    private readonly Mock<DbSet<Jugador>> _jugadoresMock;
    private readonly CuentaDao _cuentaDao;

    public PruebaCuentaDao()
    {
        _contextMock = new Mock<ContextoBaseDatos>();
        _cuentasMock = new Mock<DbSet<Cuenta>>();
        _jugadoresMock = new Mock<DbSet<Jugador>>();

        // Configuración del contexto simulado
        _contextMock.Setup(c => c.Cuentas).Returns(_cuentasMock.Object);
        _contextMock.Setup(c => c.Jugadores).Returns(_jugadoresMock.Object);

        _cuentaDao = new CuentaDao();
    }

    [Fact]
    public void AgregarJugadorConCuenta_DebeRetornarTrue_SiSeAgregaCorrectamente()
    {
        var jugador = new Jugador { JugadorId = 1, NombreUsuario = "testUser" };
        var cuenta = new Cuenta { JugadorId = 1, Correo = "test@example.com", ContraseniaHash = "hashedPassword", Salt = "saltValue" };

        _cuentasMock.Setup(m => m.Add(cuenta)).Returns(cuenta);
        _jugadoresMock.Setup(m => m.Add(jugador)).Returns(jugador);
        _contextMock.Setup(c => c.SaveChanges()).Returns(2);

        var resultado = _cuentaDao.AgregarJugadorConCuenta(jugador, cuenta);

        Assert.True(resultado);
    }


    [Fact]
    public void ValidarInicioSesion_DebeRetornarCuenta_SiCorreoYContraseñaSonCorrectos()
    {
        var cuentaExistente = new Cuenta
        {
            JugadorId = 1,
            Correo = "test@example.com",
            ContraseniaHash = "hashedPassword",
            Jugador = new Jugador { NombreUsuario = "testUser" }
        };
        var data = new List<Cuenta> { cuentaExistente }.AsQueryable();

        // Configura el DbSet de `Cuentas` para que se comporte como `IQueryable`
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.Provider).Returns(data.Provider);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.Expression).Returns(data.Expression);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.ElementType).Returns(data.ElementType);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        var resultado = _cuentaDao.ValidarInicioSesion("test@example.com", "hashedPassword");

        Assert.NotNull(resultado);
        Assert.Equal("test@example.com", resultado.Correo);
    }

    [Fact]
    public void ObtenerCuentaPorNombreUsuario_DebeRetornarCuenta_SiNombreUsuarioExiste()
    {
        var cuentaExistente = new Cuenta { JugadorId = 1, Correo = "test@example.com", Jugador = new Jugador { NombreUsuario = "testUser" } };
        var data = new List<Cuenta> { cuentaExistente }.AsQueryable();

        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.Provider).Returns(data.Provider);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.Expression).Returns(data.Expression);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.ElementType).Returns(data.ElementType);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        var resultado = _cuentaDao.ObtenerCuentaPorNombreUsuario("test@example.com");

        Assert.NotNull(resultado);
        Assert.Equal("test@example.com", resultado.Correo);
    }

    [Fact]
    public void EditarContraseñaPorCorreo_DebeRetornarTrue_SiActualizaLaContraseña()
    {
        var cuentaExistente = new Cuenta { JugadorId = 1, Correo = "test@example.com", ContraseniaHash = "oldPassword" };
        var data = new List<Cuenta> { cuentaExistente }.AsQueryable();

        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.Provider).Returns(data.Provider);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.Expression).Returns(data.Expression);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.ElementType).Returns(data.ElementType);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        _contextMock.Setup(c => c.SaveChanges()).Returns(1);

        var resultado = _cuentaDao.EditarContraseñaPorCorreo("test@example.com", "newPassword");

        Assert.True(resultado);
        Assert.Equal("newPassword", cuentaExistente.ContraseniaHash);
    }

    [Fact]
    public void ExistenciaCorreo_DebeRetornarTrue_SiCorreoExiste()
    {
        var cuentaExistente = new Cuenta { Correo = "test@example.com" };
        var data = new List<Cuenta> { cuentaExistente }.AsQueryable();

        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.Provider).Returns(data.Provider);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.Expression).Returns(data.Expression);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.ElementType).Returns(data.ElementType);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        var resultado = _cuentaDao.ExistenciaCorreo("test@example.com");

        Assert.True(resultado);
    }

    [Fact]
    public void EditarCorreo_DebeRetornarTrue_SiActualizaElCorreo()
    {
        var cuentaExistente = new Cuenta { JugadorId = 1, Correo = "old@example.com" };
        var data = new List<Cuenta> { cuentaExistente }.AsQueryable();

        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.Provider).Returns(data.Provider);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.Expression).Returns(data.Expression);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.ElementType).Returns(data.ElementType);
        _cuentasMock.As<IQueryable<Cuenta>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        _contextMock.Setup(c => c.SaveChanges()).Returns(1);

        var resultado = _cuentaDao.EditarCorreo(1, "new@example.com");

        Assert.True(resultado);
        Assert.Equal("new@example.com", cuentaExistente.Correo);
    }

    [Fact]
    public void ExisteNombreUsuario_DebeRetornarTrue_SiNombreUsuarioExiste()
    {
        var jugadorExistente = new Jugador { NombreUsuario = "testUser" };
        var data = new List<Jugador> { jugadorExistente }.AsQueryable();

        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.Provider).Returns(data.Provider);
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.Expression).Returns(data.Expression);
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.ElementType).Returns(data.ElementType);
        _jugadoresMock.As<IQueryable<Jugador>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        var resultado = _cuentaDao.ExisteNombreUsuario("testUser");

        Assert.True(resultado);
    }
}
