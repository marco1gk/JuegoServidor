using AccesoDatos;
using AccesoDatos.DAO;
using AccesoDatos.Excepciones;
using AccesoDatos.Modelo;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Xunit;

public class PruebaJugadorDao
{
    [Fact]
    public void ObtenerJugador_DebeRetornarJugador_CuandoElIdEsValido()
    {
        using (var scope = new TransactionScope())
        {
            var jugador = new Jugador
            {
                NombreUsuario = "JugadorTest",
                NumeroFotoPerfil = 1,
                Cuenta = new Cuenta
                {
                    Correo = "jugadorTest@example.com",
                    ContraseniaHash = "hashed_password",
                    Salt = "random_salt"
                }
            };

            using (var contexto = new ContextoBaseDatos())
            {
                contexto.Jugadores.Add(jugador);
                contexto.SaveChanges();
            }

            var jugadorDao = new JugadorDao();


            var jugadorObtenido = jugadorDao.ObtenerJugador(jugador.JugadorId);

            Assert.NotNull(jugadorObtenido);
            Assert.Equal(jugador.JugadorId, jugadorObtenido.JugadorId);
            Assert.Equal(jugador.NombreUsuario, jugadorObtenido.NombreUsuario);
            Assert.NotNull(jugadorObtenido.Cuenta);
            Assert.Equal(jugador.Cuenta.Correo, jugadorObtenido.Cuenta.Correo);
        }
    }

    [Fact]
    public void EditarNombreUsuario_DebeRetornarFalse_CuandoElNombreEsVacioONulo()
    {
        using (var scope = new TransactionScope())
        {
            var jugador = new Jugador
            {
                NombreUsuario = "JugadorTest",
                NumeroFotoPerfil = 1,
                Cuenta = new Cuenta
                {
                    Correo = "test@example.com",
                    ContraseniaHash = "hashed_password",
                    Salt = "random_salt"
                }
            };

            using (var contexto = new ContextoBaseDatos())
            {
                contexto.Jugadores.Add(jugador);
                contexto.SaveChanges();
            }

            var jugadorDao = new JugadorDao();

            var resultadoNulo = jugadorDao.EditarNombreUsuario(jugador.JugadorId, null);
            var resultadoVacio = jugadorDao.EditarNombreUsuario(jugador.JugadorId, "");

            Assert.False(resultadoNulo);
            Assert.False(resultadoVacio);
        }
    }

    [Fact]
    public void ObtenerJugador_DebeLanzarExcepcion_CuandoElIdEsInvalido()
    {
        using (var scope = new TransactionScope())
        {
            var jugadorDao = new JugadorDao();

            var excepcion = Assert.Throws<ExcepcionAccesoDatos>(() => jugadorDao.ObtenerJugador(9999));

            Assert.Equal("Jugador con ID 9999 no existe.", excepcion.Message);
        }
    }


    [Fact]
    public void EditarNombreUsuario_DebeActualizarNombreUsuario_CuandoElIdEsValido()
    {
        using (var scope = new TransactionScope())
        {
            var jugador = new Jugador
            {
                NombreUsuario = "JugadorAntiguo",
                NumeroFotoPerfil = 1,
                Cuenta = new Cuenta
                {
                    Correo = "jugadorAntiguo@example.com",
                    ContraseniaHash = "hashed_password",
                    Salt = "random_salt"
                }
            };

            using (var contexto = new ContextoBaseDatos())
            {
                contexto.Jugadores.Add(jugador);
                contexto.SaveChanges();
            }

            var jugadorDao = new JugadorDao();

            var resultado = jugadorDao.EditarNombreUsuario(jugador.JugadorId, "JugadorNuevo");

            Assert.True(resultado);

            using (var contexto = new ContextoBaseDatos())
            {
                var jugadorActualizado = contexto.Jugadores
                    .FirstOrDefault(j => j.JugadorId == jugador.JugadorId);
                Assert.NotNull(jugadorActualizado);
                Assert.Equal("JugadorNuevo", jugadorActualizado.NombreUsuario);
            }
        }
    }

    [Fact]
    public void EditarNombreUsuario_DebeRetornarFalse_CuandoElIdEsInvalido()
    {
        using (var scope = new TransactionScope())
        {
            var jugadorDao = new JugadorDao();

            var resultado = jugadorDao.EditarNombreUsuario(9999, "JugadorInvalido");

            Assert.False(resultado);
        }
    }

    [Fact]
    public void ObtenerJugador_DebeLanzarExcepcionDeBaseDeDatos_CuandoHayProblemaEnLaBaseDeDatos()
    {
        using (var scope = new TransactionScope())
        {
            var jugadorDao = new JugadorDao();
            var excepcion = Assert.Throws<ExcepcionAccesoDatos>(() => jugadorDao.ObtenerJugador(-1));

            Assert.Equal("Jugador con ID -1 no existe.", excepcion.Message);
        }
    }



    [Fact]
    public void ObtenerJugador_DebeLanzarExcepcion_CuandoElIdEsNuloOInvalido()
    {
        using (var scope = new TransactionScope())
        {
            var jugadorDao = new JugadorDao();
            var excepcion = Assert.Throws<ExcepcionAccesoDatos>(() => jugadorDao.ObtenerJugador(0));

            Assert.Equal("Jugador con ID 0 no existe.", excepcion.Message);
        }
    }


  
    [Fact]
    public void EditarNombreUsuario_DebeRetornarFalse_CuandoSeProducenErroresAlGuardar()
    {
        var data = new List<Jugador>
    {
        new Jugador { JugadorId = 1, NombreUsuario = "UsuarioOriginal" } }.AsQueryable();

        var mockSet = new Mock<DbSet<Jugador>>();
        mockSet.As<IQueryable<Jugador>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<Jugador>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<Jugador>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<Jugador>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        var mockContexto = new Mock<ContextoBaseDatos>();
        mockContexto.Setup(c => c.Jugadores).Returns(mockSet.Object);
        mockContexto.Setup(m => m.SaveChanges()).Throws(new DbUpdateException("Error al guardar"));

        var jugadorDao = new JugadorDao();

        var resultado = jugadorDao.EditarNombreUsuario(1, "NuevoNombre");

        Assert.False(resultado);
    }
  



}

