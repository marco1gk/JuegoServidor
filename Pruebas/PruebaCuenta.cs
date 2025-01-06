using System;
using System.Linq;
using System.Transactions;
using AccesoDatos;
using AccesoDatos.DAO;
using AccesoDatos.Excepciones;
using AccesoDatos.Modelo;
using Xunit;

namespace Pruebas
{
    public class PruebaCuentaDao
    {
        [Fact]
        public void AgregarJugadorConCuenta_DebeRegresarTrue_CuandoSeAgreganJugadorYCuenta()
        {
            using (var scope = new TransactionScope())
            {
                var jugador = new Jugador
                {
                    NombreUsuario = "JugadorTest",
                    NumeroFotoPerfil = 2
                };

                var cuenta = new Cuenta
                {
                    Correo = "jugadortest@example.com",
                    ContraseniaHash = "hashed_password_test",
                    Salt = "random_salt_test"
                };

                var cuentaDao = new CuentaDao();


                var resultado = cuentaDao.AgregarJugadorConCuenta(jugador, cuenta);


                Assert.True(resultado, "El método no agregó correctamente el jugador y la cuenta.");


            }
        }

        [Fact]
        public void AgregarJugadorConCuenta_DebeRegresarFalse_CuandoHayRestriccionDeUnicidad()
        {
            using (var scope = new TransactionScope())
            {

                var jugador1 = new Jugador
                {
                    NombreUsuario = "JugadorDuplicado",
                    NumeroFotoPerfil = 1
                };

                var cuenta1 = new Cuenta
                {
                    Correo = "duplicado@example.com",
                    ContraseniaHash = "hashed_password1",
                    Salt = "salt1"
                };

                var jugador2 = new Jugador
                {
                    NombreUsuario = "JugadorDuplicado",
                    NumeroFotoPerfil = 2
                };

                var cuenta2 = new Cuenta
                {
                    Correo = "duplicado@example.com",
                    ContraseniaHash = "hashed_password2",
                    Salt = "salt2"
                };

                var cuentaDao = new CuentaDao();

                cuentaDao.AgregarJugadorConCuenta(jugador1, cuenta1);
                var resultado = cuentaDao.AgregarJugadorConCuenta(jugador2, cuenta2);

                Assert.False(resultado, "El método no detectó correctamente la violación de restricciones de unicidad.");
            }
        }

        [Fact]
        public void ValidarInicioSesion_DebeRetornarCuenta_CuandoCredencialesSonCorrectas()
        {
            using (var scope = new TransactionScope())
            {
                var cuentaValida = new Cuenta
                {
                    Correo = "usuario@example.com",
                    ContraseniaHash = "hashed_password",
                    Salt = "random_salt",
                    Jugador = new Jugador
                    {
                        NombreUsuario = "UsuarioValido",
                        NumeroFotoPerfil = 1
                    }
                };

                using (var contexto = new ContextoBaseDatos())
                {
                    contexto.Cuentas.Add(cuentaValida);
                    contexto.SaveChanges();
                }

                var cuentaDao = new CuentaDao();

                var resultado = cuentaDao.ValidarInicioSesion("usuario@example.com", "hashed_password");

                Assert.NotNull(resultado);
                Assert.Equal("usuario@example.com", resultado.Correo);
                Assert.Equal("UsuarioValido", resultado.Jugador.NombreUsuario);
            }
        }
        [Fact]
        public void ValidarInicioSesion_DebeLanzarExcepcion_CuandoCorreoEsIncorrecto()
        {
            using (var scope = new TransactionScope())
            {
                var cuentaValida = new Cuenta
                {
                    Correo = "usuario@example.com",
                    ContraseniaHash = "hashed_password",
                    Salt = "random_salt",
                    Jugador = new Jugador
                    {
                        NombreUsuario = "UsuarioValido",
                        NumeroFotoPerfil = 1
                    }
                };

                using (var contexto = new ContextoBaseDatos())
                {
                    contexto.Cuentas.Add(cuentaValida);
                    contexto.SaveChanges();
                }

                var cuentaDao = new CuentaDao();
                var exception = Assert.Throws<ExcepcionAccesoDatos>(() => cuentaDao.ValidarInicioSesion("incorrecto@example.com", "hashed_password"));
                Assert.Contains("No se encontró una cuenta con el correo: incorrecto@example.com", exception.Message);
            }
        }



        [Fact]
        public void ValidarInicioSesion_DebeRetornarNull_CuandoContraseniaEsIncorrecta()
        {
            using (var scope = new TransactionScope())
            {
                var cuentaValida = new Cuenta
                {
                    Correo = "usuario@example.com",
                    ContraseniaHash = "hashed_password",
                    Salt = "random_salt",
                    Jugador = new Jugador
                    {
                        NombreUsuario = "UsuarioValido",
                        NumeroFotoPerfil = 1
                    }
                };

                using (var contexto = new ContextoBaseDatos())
                {
                    contexto.Cuentas.Add(cuentaValida);
                    contexto.SaveChanges();
                }

                var cuentaDao = new CuentaDao();

                var resultado = cuentaDao.ValidarInicioSesion("usuario@example.com", "incorrect_password");

                Assert.Null(resultado);
            }
        }


        [Fact]
        public void ObtenerCuentaPorNombreUsuario_DebeRetornarCuenta_CuandoExiste()
        {
            using (var scope = new TransactionScope())
            {
                var cuentaValida = new Cuenta
                {
                    Correo = "usuario@example.com",
                    ContraseniaHash = "hashed_password",
                    Salt = "random_salt",
                    Jugador = new Jugador
                    {
                        NombreUsuario = "UsuarioValido",
                        NumeroFotoPerfil = 1
                    }
                };

                using (var contexto = new ContextoBaseDatos())
                {
                    contexto.Cuentas.Add(cuentaValida);
                    contexto.SaveChanges();
                }

                var cuentaDao = new CuentaDao();

                var resultado = cuentaDao.ObtenerCuentaPorNombreUsuario("usuario@example.com");

                Assert.NotNull(resultado);
                Assert.Equal("usuario@example.com", resultado.Correo);
                Assert.Equal("UsuarioValido", resultado.Jugador.NombreUsuario);
            }
        }

        [Fact]
        public void ObtenerCuentaPorNombreUsuario_DebeRetornarNull_CuandoNoExiste()
        {
            using (var scope = new TransactionScope())
            {
                var cuentaDao = new CuentaDao();

                var resultado = cuentaDao.ObtenerCuentaPorNombreUsuario("noexiste@example.com");

                Assert.Null(resultado);
            }
        }


        [Fact]
        public void EditarContraseñaPorCorreo_DebeActualizarContrasena_CuandoCorreoExiste()
        {
            using (var scope = new TransactionScope())
            {
                var cuenta = new Cuenta
                {
                    Correo = "usuario@example.com",
                    ContraseniaHash = "hashed_password",
                    Salt = "random_salt",
                    Jugador = new Jugador
                    {
                        NombreUsuario = "UsuarioValido",
                        NumeroFotoPerfil = 1
                    }
                };

                using (var contexto = new ContextoBaseDatos())
                {
                    contexto.Cuentas.Add(cuenta);
                    contexto.SaveChanges();
                }

                var cuentaDao = new CuentaDao();
                string nuevaContrasenia = "nueva_password";

                var resultado = cuentaDao.EditarContraseñaPorCorreo("usuario@example.com", nuevaContrasenia);

                Assert.True(resultado);

                using (var contexto = new ContextoBaseDatos())
                {
                    var cuentaActualizada = contexto.Cuentas.SingleOrDefault(c => c.Correo == "usuario@example.com");
                    Assert.NotNull(cuentaActualizada);

                    Assert.NotEqual("hashed_password", cuentaActualizada.ContraseniaHash);
                    Assert.NotEqual("random_salt", cuentaActualizada.Salt);
                }
            }
        }

        [Fact]
        public void EditarContraseñaPorCorreo_DebeRetornarFalse_CuandoCorreoNoExiste()
        {
            using (var scope = new TransactionScope())
            {
                var cuentaDao = new CuentaDao();
                string nuevaContrasenia = "nueva_password";

                var resultado = cuentaDao.EditarContraseñaPorCorreo("noexiste@example.com", nuevaContrasenia);

                Assert.False(resultado);
            }
        }

        [Fact]
        public void ExistenciaCorreo_DebeRetornarTrue_CuandoElCorreoExiste()
        {
            using (var scope = new TransactionScope())
            {
                var cuenta = new Cuenta
                {
                    Correo = "usuario@example.com",
                    ContraseniaHash = "hashed_password",
                    Salt = "random_salt",
                    Jugador = new Jugador
                    {
                        NombreUsuario = "UsuarioValido",
                        NumeroFotoPerfil = 1
                    }
                };

                using (var contexto = new ContextoBaseDatos())
                {
                    contexto.Cuentas.Add(cuenta);
                    contexto.SaveChanges();
                }

                var cuentaDao = new CuentaDao();

                var resultado = cuentaDao.ExistenciaCorreo("usuario@example.com");

                Assert.True(resultado);
            }
        }

        [Fact]
        public void ExistenciaCorreo_DebeRetornarFalse_CuandoElCorreoNoExiste()
        {
            using (var scope = new TransactionScope())
            {
                var cuentaDao = new CuentaDao();

                var resultado = cuentaDao.ExistenciaCorreo("noexiste@example.com");

                Assert.False(resultado);
            }
        }


        [Fact]
        public void EditarCorreo_DebeActualizarCorreo_CuandoCuentaExiste()
        {
            using (var scope = new TransactionScope())
            {
                var cuenta = new Cuenta
                {
                    Correo = "usuario@example.com",
                    ContraseniaHash = "hashed_password",
                    Salt = "random_salt",
                    Jugador = new Jugador
                    {
                        NombreUsuario = "UsuarioValido",
                        NumeroFotoPerfil = 1
                    }
                };

                using (var contexto = new ContextoBaseDatos())
                {
                    contexto.Cuentas.Add(cuenta);
                    contexto.SaveChanges();
                }

                var cuentaDao = new CuentaDao();
                string nuevoCorreo = "nuevoCorreo@example.com";

                var resultado = cuentaDao.EditarCorreo(cuenta.JugadorId, nuevoCorreo);

                Assert.True(resultado);

                using (var contexto = new ContextoBaseDatos())
                {
                    var cuentaActualizada = contexto.Cuentas
                        .SingleOrDefault(c => c.JugadorId == cuenta.JugadorId);
                    Assert.NotNull(cuentaActualizada);
                    Assert.Equal(nuevoCorreo, cuentaActualizada.Correo);
                }
            }
        }

        [Fact]
        public void EditarCorreo_DebeRetornarFalse_CuandoCuentaNoExiste()
        {
            using (var scope = new TransactionScope())
            {
                var cuentaDao = new CuentaDao();
                string nuevoCorreo = "nuevoCorreo@example.com";

                var resultado = cuentaDao.EditarCorreo(9999, nuevoCorreo);

                Assert.False(resultado);
            }
        }


        [Fact]
        public void ExisteNombreUsuario_DebeRetornarTrue_CuandoElNombreUsuarioExiste()
        {
            using (var scope = new TransactionScope())
            {
                var jugador = new Jugador
                {
                    NombreUsuario = "JugadorExistente",
                    NumeroFotoPerfil = 1,
                    Cuenta = new Cuenta
                    {
                        Correo = "jugador@example.com",
                        ContraseniaHash = "hashed_password",
                        Salt = "random_salt"
                    }
                };

                using (var contexto = new ContextoBaseDatos())
                {
                    contexto.Jugadores.Add(jugador);
                    contexto.SaveChanges();
                }

                var cuentaDao = new CuentaDao();

                var resultado = cuentaDao.ExisteNombreUsuario("JugadorExistente");

                Assert.True(resultado);
            }
        }

        [Fact]
        public void ExisteNombreUsuario_DebeRetornarFalse_CuandoElNombreUsuarioNoExiste()
        {
            using (var scope = new TransactionScope())
            {
                var cuentaDao = new CuentaDao();

                var resultado = cuentaDao.ExisteNombreUsuario("NombreUsuarioInexistente");

                Assert.False(resultado);
            }
        }

        [Fact]
        public void AgregarJugadorConCuenta_DebeLanzarExcepcion_CuandoJugadorOCuentaEsNulo()
        {
            var cuentaDao = new CuentaDao();

            Assert.Throws<ArgumentNullException>(() => cuentaDao.AgregarJugadorConCuenta(null, null));
        }


        [Fact]
        public void ValidarInicioSesion_DebeLanzarExcepcion_CuandoContraseñaEsIncorrecta()
        {
            using (var scope = new TransactionScope())
            {
                var cuentaValida = new Cuenta
                {
                    Correo = "usuario@example.com",
                    ContraseniaHash = "hashed_password",
                    Salt = "random_salt",
                    Jugador = new Jugador
                    {
                        NombreUsuario = "UsuarioValido",
                        NumeroFotoPerfil = 1
                    }
                };

                using (var contexto = new ContextoBaseDatos())
                {
                    contexto.Cuentas.Add(cuentaValida);
                    contexto.SaveChanges();
                }

                var cuentaDao = new CuentaDao();

                var exception = Assert.Throws<ExcepcionAccesoDatos>(() => cuentaDao.ValidarInicioSesion("usuario@example.com", "wrong_password"));
                Assert.Contains("No se encontró una cuenta con el correo: usuario@example.com", exception.Message);
            }
        }

        [Fact]
        public void ObtenerCuentaPorNombreUsuario_DebeLanzarExcepcion_CuandoCuentaTieneCamposNulos()
        {
            using (var scope = new TransactionScope())
            {
                var cuentaValida = new Cuenta
                {
                    Correo = "usuario@example.com",
                    ContraseniaHash = "hashed_password",
                    Salt = "random_salt",
                    Jugador = null  // Jugador nulo
                };

                using (var contexto = new ContextoBaseDatos())
                {
                    contexto.Cuentas.Add(cuentaValida);
                    contexto.SaveChanges();
                }

                var cuentaDao = new CuentaDao();

                Assert.Throws<ExcepcionAccesoDatos>(() => cuentaDao.ObtenerCuentaPorNombreUsuario("usuario@example.com"));
            }
        }

        [Fact]
        public void ExistenciaCorreo_DebeRetornarFalse_CuandoLaBaseDeDatosEstaVacia()
        {
            using (var scope = new TransactionScope())
            {
                var cuentaDao = new CuentaDao();
                var resultado = cuentaDao.ExistenciaCorreo("noexiste@example.com");

                Assert.False(resultado);
            }
        }

        [Fact]
        public void ExisteNombreUsuario_DebeRetornarFalse_CuandoNoHayJugadores()
        {
            using (var scope = new TransactionScope())
            {
                var cuentaDao = new CuentaDao();
                var resultado = cuentaDao.ExisteNombreUsuario("JugadorInexistente");

                Assert.False(resultado);
            }
        }

    }
}

















































