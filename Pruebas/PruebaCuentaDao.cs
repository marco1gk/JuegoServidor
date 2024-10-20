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
        [Theory]
        [InlineData("correoYaExiste@gmail.com", "YaExisteUsuario", "hashed_password")]
        [InlineData("correoYaExiste2@gmail.com", "YaExisteUsuario2", "1234")]

        public void validarInicioSesion(string correo, string usuario, string contraseña)
        {
            Cuenta resultado = new Cuenta();
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
                var cuentaExistente2 = new Cuenta
                {
                    Correo = "correoYaExiste2@gmail.com",
                    ContraseniaHash = "1234",
                    Jugador = new Jugador
                    {
                        NombreUsuario = "YaExisteUsuario2",
                        NumeroFotoPerfil = 1
                    }
                };
                contexto.Cuentas.Add(cuentaExistente);
                contexto.Cuentas.Add(cuentaExistente2);
                contexto.SaveChanges();
                CuentaDao cuentaDao = new CuentaDao();



                resultado = cuentaDao.ValidarInicioSesion(correo, contraseña);

                contexto.Cuentas.RemoveRange(contexto.Cuentas);
                contexto.Jugadores.RemoveRange(contexto.Jugadores);
                contexto.SaveChanges();

            }
            Assert.NotNull(resultado);
            Assert.Equal(resultado.Correo, correo);
            Assert.Equal(resultado.Jugador.NombreUsuario, usuario);

        }

    }

   
}



