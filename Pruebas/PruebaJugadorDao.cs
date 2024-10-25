using AccesoDatos;
using AccesoDatos.DAO;
using AccesoDatos.Modelo;
using System;
using Xunit;

namespace Pruebas
{
    public class PruebaJugadorDao
    {
    
        [Theory]
        [InlineData(1, "UsuarioExistente", "correoExistente@gmail.com")]
        [InlineData(2, null, null)] 
        public void ObtenerJugador(int idJugador, string nombreEsperado, string correoEsperado)
        {
            Jugador resultado;

            using (var contexto = new ContextoBaseDatos())
            {
             
                if (idJugador == 1)
                {
                    var cuenta = new Cuenta { Correo = "correoExistente@gmail.com" };
                    var jugador = new Jugador { JugadorId = 1, NombreUsuario = "UsuarioExistente", Cuenta = cuenta };
                    contexto.Jugadores.Add(jugador);
                    contexto.SaveChanges();
                }

                JugadorDao jugadorDao = new JugadorDao();
                resultado = jugadorDao.ObtenerJugador(idJugador);

              
                contexto.Jugadores.RemoveRange(contexto.Jugadores);
                contexto.SaveChanges();
            }

            if (nombreEsperado != null)
            {
                Assert.NotNull(resultado);
                Assert.Equal(nombreEsperado, resultado.NombreUsuario);
                Assert.Equal(correoEsperado, resultado.Cuenta.Correo);
            }
            else
            {
                Assert.Null(resultado);
            }
        }

       
        [Theory]
        [InlineData(1, "NuevoNombreUsuario", true)]
        [InlineData(2, "UsuarioInexistente", false)] // Caso donde no existe el jugador
        public void EditarNombreUsuario(int idJugador, string nuevoNombreUsuario, bool salidaEsperada)
        {
            bool resultado;

            using (var contexto = new ContextoBaseDatos())
            {
                // Prepara datos de prueba
                if (idJugador == 1)
                {
                    var jugador = new Jugador { JugadorId = 1, NombreUsuario = "UsuarioExistente" };
                    contexto.Jugadores.Add(jugador);
                    contexto.SaveChanges();
                }

                JugadorDao jugadorDao = new JugadorDao();
                resultado = jugadorDao.EditarNombreUsuario(idJugador, nuevoNombreUsuario);

                // Limpia los datos para evitar interferencias
                contexto.Jugadores.RemoveRange(contexto.Jugadores);
                contexto.SaveChanges();
            }

            Assert.Equal(salidaEsperada, resultado);
        }
    }
}
