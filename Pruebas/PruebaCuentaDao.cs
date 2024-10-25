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


        public void AgregarCuenta(string mail,string nombreUsuario, bool salidaEsperada)
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

        public void ValidarInicioSesion(string correo, string usuario, string contraseña)
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

          
            [Theory]
            [InlineData("correoExistente@gmail.com", "nuevaContraseña", true)]
            [InlineData("correoInexistente@gmail.com", "nuevaContraseña", false)]
            public void EditarContraseñaPorCorreo(string correo, string nuevaContraseña, bool salidaEsperada)
            {
                bool resultado;

                using (var contexto = new ContextoBaseDatos())
                {
                    
                    var cuenta = new Cuenta
                    {
                        Correo = "correoExistente@gmail.com",
                        ContraseniaHash = "contraseñaAntigua",
                        Jugador = new Jugador { NombreUsuario = "UsuarioExistente", NumeroFotoPerfil = 1 }
                    };
                    contexto.Cuentas.Add(cuenta);
                    contexto.SaveChanges();

                    CuentaDao cuentaDao = new CuentaDao();
                    resultado = cuentaDao.EditarContraseñaPorCorreo(correo, nuevaContraseña);

                    contexto.Cuentas.RemoveRange(contexto.Cuentas);
                    contexto.SaveChanges();
                }

                Assert.Equal(salidaEsperada, resultado);
            }

            
            [Theory]
            [InlineData("correoExistente@gmail.com", true)]
            [InlineData("correoNoExistente@gmail.com", false)]
            public void ExistenciaCorreo(string correo, bool salidaEsperada)
            {
                bool resultado;

                using (var contexto = new ContextoBaseDatos())
                {
                    
                    var cuenta = new Cuenta { Correo = "correoExistente@gmail.com" };
                    contexto.Cuentas.Add(cuenta);
                    contexto.SaveChanges();

                    CuentaDao cuentaDao = new CuentaDao();
                    resultado = cuentaDao.ExistenciaCorreo(correo);

                    contexto.Cuentas.RemoveRange(contexto.Cuentas);
                    contexto.SaveChanges();
                }

                Assert.Equal(salidaEsperada, resultado);
            }

            
            [Theory]
            [InlineData(1, "nuevoCorreo@gmail.com", true)]
            [InlineData(2, "correoInexistente@gmail.com", false)]
            public void EditarCorreo(int idCuenta, string nuevoCorreo, bool salidaEsperada)
            {
                bool resultado;

                using (var contexto = new ContextoBaseDatos())
                {
                    
                    var cuenta = new Cuenta
                    {
                        JugadorId = 1,
                        Correo = "correoAntiguo@gmail.com",
                        Jugador = new Jugador { NombreUsuario = "UsuarioExistente", NumeroFotoPerfil = 1 }
                    };
                    contexto.Cuentas.Add(cuenta);
                    contexto.SaveChanges();

                    CuentaDao cuentaDao = new CuentaDao();
                    resultado = cuentaDao.EditarCorreo(idCuenta, nuevoCorreo);

                    contexto.Cuentas.RemoveRange(contexto.Cuentas);
                    contexto.SaveChanges();
                }

                Assert.Equal(salidaEsperada, resultado);
            }

            [Theory]
            [InlineData("correoExistente@gmail.com", true)]
            [InlineData("correoNoExistente@gmail.com", false)]
            public void ExisteCorreo(string correo, bool salidaEsperada)
            {
                bool resultado;

                using (var contexto = new ContextoBaseDatos())
                {
                    var cuenta = new Cuenta { Correo = "correoExistente@gmail.com" };
                    contexto.Cuentas.Add(cuenta);
                    contexto.SaveChanges();

                    CuentaDao cuentaDao = new CuentaDao();
                    resultado = cuentaDao.ExisteCorreo(correo);

                    contexto.Cuentas.RemoveRange(contexto.Cuentas);
                    contexto.SaveChanges();
                }

                Assert.Equal(salidaEsperada, resultado);
            }

            [Theory]
            [InlineData("UsuarioExistente", true)]
            [InlineData("UsuarioInexistente", false)]
            public void ExisteNombreUsuario(string nombreUsuario, bool salidaEsperada)
            {
                bool resultado;

                using (var contexto = new ContextoBaseDatos())
                {
                    var jugador = new Jugador { NombreUsuario = "UsuarioExistente" };
                    contexto.Jugadores.Add(jugador);
                    contexto.SaveChanges();

                    CuentaDao cuentaDao = new CuentaDao();
                    resultado = cuentaDao.ExisteNombreUsuario(nombreUsuario);

                    contexto.Jugadores.RemoveRange(contexto.Jugadores);
                    contexto.SaveChanges();
                }

                Assert.Equal(salidaEsperada, resultado);
            }
    }
}
