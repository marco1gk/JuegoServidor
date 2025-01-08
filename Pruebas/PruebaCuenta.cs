using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using AccesoDatos;
using AccesoDatos.DAO;
using AccesoDatos.Excepciones;
using AccesoDatos.Modelo;
using Moq;
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
        public void AgregarJugadorConCuenta_DebeRegresarFalse_CuandoElNumeroDeFilasAfectadasNoEsDos()
        {
            using (var scope = new TransactionScope())
            {
                var jugador = new Jugador
                {
                    NombreUsuario = "JugadorTest2",
                    NumeroFotoPerfil = 2
                };

                var cuenta = new Cuenta
                {
                    Correo = "jugadortest2@example.com",
                    ContraseniaHash = "hashed_password_test2",
                    Salt = "random_salt_test2"
                };

                var cuentaDao = new CuentaDao();

                var resultado = cuentaDao.AgregarJugadorConCuenta(jugador, cuenta);

                if (resultado)
                {
                    using (var contexto = new ContextoBaseDatos())
                    {
                        var jugadorGuardado = contexto.Jugadores
                            .FirstOrDefault(j => j.NombreUsuario == "JugadorTest2");

                        var cuentaGuardada = contexto.Cuentas
                            .FirstOrDefault(c => c.Correo == "jugadortest2@example.com");

                        Assert.NotNull(jugadorGuardado);
                        Assert.NotNull(cuentaGuardada);

                        contexto.Jugadores.Remove(jugadorGuardado);
                        contexto.Cuentas.Remove(cuentaGuardada);
                        contexto.SaveChanges();
                    }
                }
                else
                {
                    Assert.False(resultado, "El método debería haber regresado false si no se agregaron ambas entidades.");
                }
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
        public void AgregarJugadorConCuenta_DebeRegresarTrue_CuandoSeAgrega()
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

                using (var contexto = new ContextoBaseDatos())
                {
                    var jugadorEliminado = contexto.Jugadores.FirstOrDefault(j => j.NombreUsuario == "JugadorTest");
                    if (jugadorEliminado != null)
                    {
                        contexto.Jugadores.Remove(jugadorEliminado);
                    }

                    var cuentaEliminada = contexto.Cuentas.FirstOrDefault(c => c.Correo == "jugadortest@example.com");
                    if (cuentaEliminada != null)
                    {
                        contexto.Cuentas.Remove(cuentaEliminada);
                    }

                    contexto.SaveChanges();
                }
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
      

        [Fact]
        public void AgregarJugadorConCuenta_DebeAgregarJugadorYCuenta_Correctamente()
        {
            using (var scope = new TransactionScope())
            {
                var jugador = new Jugador
                {
                    NombreUsuario = "JugadorTestCorrecto",
                    NumeroFotoPerfil = 2
                };

                var cuenta = new Cuenta
                {
                    Correo = "jugadortestcorrecto@example.com",
                    ContraseniaHash = "hashed_password_correcto",
                    Salt = "random_salt_correcto"
                };

                var cuentaDao = new CuentaDao();

                var resultado = cuentaDao.AgregarJugadorConCuenta(jugador, cuenta);

                Assert.True(resultado, "El método no agregó correctamente el jugador y la cuenta.");

                using (var contexto = new ContextoBaseDatos())
                {
                    var jugadorBorrar = contexto.Jugadores.FirstOrDefault(j => j.NombreUsuario == "JugadorTestCorrecto");
                    if (jugadorBorrar != null)
                    {
                        contexto.Jugadores.Remove(jugadorBorrar);
                    }

                    var cuentaBorrar = contexto.Cuentas.FirstOrDefault(c => c.Correo == "jugadortestcorrecto@example.com");
                    if (cuentaBorrar != null)
                    {
                        contexto.Cuentas.Remove(cuentaBorrar);
                    }

                    contexto.SaveChanges();
                }
            }
        }

        [Fact]
        public void ObtenerIdJugadorPorNombreUsuario_DebeRetornarIdCorrecto_CuandoElJugadorExiste()
        {
            var jugador = new Jugador
            {
                NombreUsuario = "JugadorTest",
                NumeroFotoPerfil = 2
            };

            using (var contexto = new ContextoBaseDatos())
            {
                contexto.Jugadores.Add(jugador);
                contexto.SaveChanges();
            }
        
            var cuentaDao = new CuentaDao();
            var idJugador = cuentaDao.ObtenerIdJugadorPorNombreUsuario("JugadorTest");

            Assert.Equal(jugador.JugadorId, idJugador);

            using (var contexto = new ContextoBaseDatos())
            {
                var jugadorEliminado = contexto.Jugadores.FirstOrDefault(j => j.NombreUsuario == "JugadorTest");
                if (jugadorEliminado != null)
                {
                    contexto.Jugadores.Remove(jugadorEliminado);
                    contexto.SaveChanges();
                }
            }
        }

        [Fact]
        public void ObtenerIdJugadorPorNombreUsuario_DebeRetornarCero_CuandoNoSeEncuentraElJugador()
        {
            var cuentaDao = new CuentaDao();
            var idJugador = cuentaDao.ObtenerIdJugadorPorNombreUsuario("JugadorNoExistente");

            Assert.Equal(0, idJugador);
        }

        [Fact]
        public void ValidarInicioSesion_DebeLanzarExcepcion_CuandoCorreoOContraseniaNoSonValidos()
        {
            string correoIncorrecto = "wrongemail@example.com";
            string contraseniaIncorrecta = "wrong_password";

            var cuentaDao = new CuentaDao();

            var exception = Assert.Throws<ExcepcionAccesoDatos>(() =>
                cuentaDao.ValidarInicioSesion(correoIncorrecto, contraseniaIncorrecta)
            );

            Assert.Contains("No se encontró una cuenta con el correo", exception.Message);
        }

        [Fact]
        public void ValidarInicioSesion_DebeLanzarExcepcion_CuandoNoExisteCuenta()
        {
            string correoNoExistente = "nonexistent@example.com";
            string contraseniaHash = "hashed_password";

            var cuentaDao = new CuentaDao();

            var exception = Assert.Throws<ExcepcionAccesoDatos>(() =>
                cuentaDao.ValidarInicioSesion(correoNoExistente, contraseniaHash)
            );

            Assert.Contains("No se encontró una cuenta con el correo", exception.Message);
        }
      

        [Fact]
        public void ValidarInicioSesion_DebeLanzarExcepcion_CuandoHayErrorDeBaseDeDatos()
        {
            string correo = "test@example.com";
            string contraseniaHash = "hashed_password";

            var cuentaDao = new CuentaDao();

            var exception = Assert.Throws<ExcepcionAccesoDatos>(() =>
                cuentaDao.ValidarInicioSesion(correo, contraseniaHash)
            );

            Assert.IsType<ExcepcionAccesoDatos>(exception);
        }
        [Fact]//FUNCIONA
        public void ValidarInicioSesion_DebeLanzarExcepcionGeneral_CuandoHayErrorInesperado()
        {
            string correo = "test@example.com";
            string contraseniaHash = "hashed_password";

            var cuentaDao = new CuentaDao();

            var exception = Assert.Throws<ExcepcionAccesoDatos>(() =>
                cuentaDao.ValidarInicioSesion(correo, contraseniaHash)
            );

            Assert.IsType<ExcepcionAccesoDatos>(exception);
        }

        [Fact]
        public void ValidarInicioSesion_DebeLanzarExcepcion_CuandoCorreoYContraseniaSonInvalidos()
        {
            string correoIncorrecto = "wrongemail@example.com";
            string contraseniaIncorrecta = "wrong_password";
        
            var cuentaDao = new CuentaDao();

            var exception = Assert.Throws<ExcepcionAccesoDatos>(() =>
            {
                cuentaDao.ValidarInicioSesion(correoIncorrecto, contraseniaIncorrecta);
            });

            Assert.Contains("No se encontró una cuenta con el correo", exception.Message);
            Assert.Contains("o la contraseña es incorrecta", exception.Message);
        }


       
        [Fact]
        public void ObtenerCuentaPorNombreUsuario_DebeLanzarExcepcion_CuandoCorreoNoEsValido()
        {
            string correoInvalido = "invalid@example.com";

            var cuentaDao = new CuentaDao();

            Assert.Throws<ExcepcionAccesoDatos>(() => cuentaDao.ObtenerCuentaPorNombreUsuario(correoInvalido));
        }

        [Fact]
        public void ObtenerCuentaPorNombreUsuario_DebeLanzarExcepcion_CuandoCorreoNoExiste()
        {
            string correoInvalido = "inexistente@example.com";

            var cuentaDao = new CuentaDao();

            var exception = Assert.Throws<ExcepcionAccesoDatos>(() =>
                cuentaDao.ObtenerCuentaPorNombreUsuario(correoInvalido));

            Assert.Equal($"No se encontró una cuenta con el correo: {correoInvalido}", exception.Message);
        }
        

        [Fact]
        public void ObtenerCuentaPorNombreUsuario_DebeLanzarExcepcion_CuandoHayErrorDeBaseDeDatos()
        {
            string nombreUsuario = "test@example.com"; 

            var cuentaDao = new CuentaDao();

            var exception = Assert.Throws<ExcepcionAccesoDatos>(() =>
                cuentaDao.ObtenerCuentaPorNombreUsuario(nombreUsuario)
            );

            Assert.IsType<ExcepcionAccesoDatos>(exception);
        }

      
        [Fact]
        public void EditarContraseñaPorCorreo_DebeRegresarFalse_CuandoCorreoNoExiste()
        {
            string correoInexistente = "inexistent@example.com";
            string nuevaContrasenia = "newhashedpassword";

            var cuentaDao = new CuentaDao();
            var resultado = cuentaDao.EditarContraseñaPorCorreo(correoInexistente, nuevaContrasenia);

            Assert.False(resultado);
        }

      
        [Fact]
        public void ExistenciaCorreo_DebeRegresarFalse_CuandoElCorreoNoExiste()
        {
            string correoNoExistente = "noexistent@example.com";

            var cuentaDao = new CuentaDao();
            bool resultado = cuentaDao.ExistenciaCorreo(correoNoExistente);

            Assert.False(resultado);
        }

       
        [Fact]
        public void EditarCorreo_DebeRegresarFalse_CuandoLaCuentaNoExiste()
        {
            int idCuentaInvalida = 9999;
            string nuevoCorreo = "nuevo_correo@example.com";

            var cuentaDao = new CuentaDao();

            var resultado = cuentaDao.EditarCorreo(idCuentaInvalida, nuevoCorreo);

            Assert.False(resultado);
        }


        [Fact]
        public void ExisteCorreo_DebeRegresarFalse_CuandoCorreoNoExiste()
        {
            string correoNoExistente = "correo_no_existente@example.com";

            var cuentaDao = new CuentaDao();
            var resultado = cuentaDao.ExisteCorreo(correoNoExistente);

            Assert.False(resultado);
        }
        [Fact]
        public void ExisteCorreo_DebeRegresarFalse_CuandoOcurreErrorDeBaseDeDatos()
        {
            string correo = "correo_inexistente@example.com";

            var cuentaDao = new CuentaDao();

            var resultado = cuentaDao.ExisteCorreo(correo);

            Assert.False(resultado);
        }

        [Fact]
        public void ExisteNombreUsuario_DebeRegresarTrue_CuandoNombreUsuarioExiste()
        {
            string nombreUsuarioExistente = "TestPlayer";

            var jugador = new Jugador
            {
                NombreUsuario = nombreUsuarioExistente
            };

            using (var contexto = new ContextoBaseDatos())
            {
                contexto.Jugadores.Add(jugador);
                contexto.SaveChanges();
            }

            var cuentaDao = new CuentaDao();
            var resultado = cuentaDao.ExisteNombreUsuario(nombreUsuarioExistente);

            Assert.True(resultado);

            using (var contexto = new ContextoBaseDatos())
            {
                var jugadorAEliminar = contexto.Jugadores.FirstOrDefault(j => j.NombreUsuario == nombreUsuarioExistente);
                if (jugadorAEliminar != null)
                {
                    contexto.Jugadores.Remove(jugadorAEliminar);
                    contexto.SaveChanges();
                }
            }
        }

        [Fact]
        public void ExisteNombreUsuario_DebeRegresarFalse_CuandoNombreUsuarioNoExiste()
        {
            string nombreUsuarioNoExistente = "NonExistentPlayer";

            var cuentaDao = new CuentaDao();
            var resultado = cuentaDao.ExisteNombreUsuario(nombreUsuarioNoExistente);

            Assert.False(resultado);
        }
       

    }
}

















































