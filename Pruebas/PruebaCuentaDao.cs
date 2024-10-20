using AccesoDatos;
using AccesoDatos.DAO;
using AccesoDatos.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace Pruebas
{
    public class PruebaCuentaDao
    {
        [Theory]
        [InlineData ("vaomarco052@gmail.com","marco", true)]
        [InlineData ("correoYaExiste@gmail.com","NoExisteUsuario", false)]
        [InlineData("correoNoExiste@gmail.com", "YaExisteUsuario", false)]


        public void agregarCuenta(string mail,string nombreUsuario, bool salidaEsperada)
        {
            bool resultado;

            using (var contexto = new ContextoBaseDatos())
            {
                var cuentaExistente = new Cuenta
                {
                    Correo = "correoYaExiste@gmail.com",
                    ContraseniaHash = "hashed_password",
                    Jugador = new Jugador
                    {
                        NombreUsuario = "YaExisteUsuario",
                        NumeroFotoPerfil = 1
                    }
                };
                contexto.Cuentas.Add(cuentaExistente);
                contexto.SaveChanges();
                CuentaDao cuentaDao = new CuentaDao();

                Jugador jugadorPrueba = new Jugador();
                jugadorPrueba.NombreUsuario = nombreUsuario;
                jugadorPrueba.NumeroFotoPerfil = 1;

                Cuenta cuentaPrueba = new Cuenta();
                cuentaPrueba.ContraseniaHash = "12345";
                cuentaPrueba.Correo = mail;

                resultado = cuentaDao.AgregarJugadorConCuenta(jugadorPrueba, cuentaPrueba);

                contexto.Cuentas.RemoveRange(contexto.Cuentas);
                contexto.Jugadores.RemoveRange(contexto.Jugadores);
                contexto.SaveChanges();

            }
            Assert.Equal(resultado, salidaEsperada);


        }
    }
}
