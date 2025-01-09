using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AccesoDatos;
using AccesoDatos.DAO;
using AccesoDatos.Modelo;
using Xunit;


namespace Pruebas
{
    public class PruebaAmistadDao
    {
        [Fact]
        public void VerificarAmistad_DebeRegresarTrue_CuandoAmistadExiste()
        {
            int jugadorId;
            int amigoId;

            using (var scope = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugador = new Jugador
                    {
                        NombreUsuario = "Jugador1",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador1@example.com",
                            ContraseniaHash = "hashed_password",
                            Salt = "random_salt"
                        }
                    };

                    var amigo = new Jugador
                    {
                        NombreUsuario = "Jugador2",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador2@example.com",
                            ContraseniaHash = "hashed_password",
                            Salt = "random_salt"
                        }
                    };

                    contexto.Jugadores.Add(jugador);
                    contexto.Jugadores.Add(amigo);
                    contexto.SaveChanges();

                    jugadorId = jugador.JugadorId;
                    amigoId = amigo.JugadorId;

                    var amistad = new Amistad
                    {
                        JugadorId = jugadorId,
                        AmigoId = amigoId,
                        EstadoAmistad = "Amigo"
                    };

                    contexto.Amistades.Add(amistad);
                    contexto.SaveChanges();
                }

                var amistadDao = new AmistadDao();
                var resultado = amistadDao.VerificarAmistad(jugadorId, amigoId);

                Assert.True(resultado);

            }
        }

        [Fact]
        public void VerificarAmistad_DebeRegresarFalse_CuandoAmistadNoExiste()
        {
            int jugadorId;
            int amigoId;

            using (var scope = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugador = new Jugador
                    {
                        NombreUsuario = "Jugador1",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador1@example.com",
                            ContraseniaHash = "hashed_password",
                            Salt = "random_salt"
                        }
                    };

                    var amigo = new Jugador
                    {
                        NombreUsuario = "Jugador2",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador2@example.com",
                            ContraseniaHash = "hashed_password",
                            Salt = "random_salt"
                        }
                    };

                    contexto.Jugadores.Add(jugador);
                    contexto.Jugadores.Add(amigo);
                    contexto.SaveChanges();

                    jugadorId = jugador.JugadorId;
                    amigoId = amigo.JugadorId;
                }
                var amistadDao = new AmistadDao();
                var resultado = amistadDao.VerificarAmistad(jugadorId, amigoId);


                Assert.False(resultado);
            }
        }

        [Fact]
        public void VerificarAmistad_DebeRegresarTrue_CuandoAmistadEsSolicitudPendiente()
        {
            int jugadorId;
            int amigoId;

            using (var scope = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugador = new Jugador
                    {
                        NombreUsuario = "Jugador1",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador1@example.com",
                            ContraseniaHash = "hashed_password",
                            Salt = "random_salt"
                        }
                    };

                    var amigo = new Jugador
                    {
                        NombreUsuario = "Jugador2",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador2@example.com",
                            ContraseniaHash = "hashed_password",
                            Salt = "random_salt"
                        }
                    };

                    contexto.Jugadores.Add(jugador);
                    contexto.Jugadores.Add(amigo);
                    contexto.SaveChanges();

                    jugadorId = jugador.JugadorId;
                    amigoId = amigo.JugadorId;

                    var amistad = new Amistad
                    {
                        JugadorId = jugadorId,
                        AmigoId = amigoId,
                        EstadoAmistad = "Solicitud"
                    };

                    contexto.Amistades.Add(amistad);
                    contexto.SaveChanges();
                }

                var amistadDao = new AmistadDao();
                var resultado = amistadDao.VerificarAmistad(jugadorId, amigoId);

                Assert.True(resultado);
            }
        }


        [Fact]
        public void DebeAgregarSolicitudDeAmistad_CuandoIdsSonValidos()
        {
            int jugadorId;
            int amigoId;

            using (var scope = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugador = new Jugador
                    {
                        NombreUsuario = "Jugador1",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador1@example.com",
                            ContraseniaHash = "hashed_password",
                            Salt = "random_salt"
                        }
                    };

                    var amigo = new Jugador
                    {
                        NombreUsuario = "Jugador2",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador2@example.com",
                            ContraseniaHash = "hashed_password",
                            Salt = "random_salt"
                        }
                    };

                    contexto.Jugadores.Add(jugador);
                    contexto.Jugadores.Add(amigo);
                    contexto.SaveChanges();

                    jugadorId = jugador.JugadorId;
                    amigoId = amigo.JugadorId;
                }

                var amistadDao = new AmistadDao();
                var resultado = amistadDao.AgregarSolicitudAmistad(jugadorId, amigoId);

                Assert.Equal(1, resultado);

                using (var contexto = new ContextoBaseDatos())
                {
                    var solicitud = contexto.Amistades.FirstOrDefault(a =>
                        a.JugadorId == jugadorId &&
                        a.AmigoId == amigoId &&
                        a.EstadoAmistad == "Request");

                    Assert.NotNull(solicitud);
                }
            }
        }

        [Fact]
        public void NoDebeAgregarSolicitudDeAmistad_CuandoIdsNoSonValidos()
        {
            int idJugadorInvalido = -1;
            int idAmigoInvalido = 0;

            var amistadDao = new AmistadDao();
            var resultado = amistadDao.AgregarSolicitudAmistad(idJugadorInvalido, idAmigoInvalido);

            Assert.Equal(-1, resultado);
        }



        [Fact]
        public void EsAmigo_DebeDevolverTrue_CuandoSonAmigos()
        {
            int idJugador1, idJugador2;

            using (var transaccion = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugador1 = new Jugador
                    {
                        NombreUsuario = "Jugador1",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador1@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    var jugador2 = new Jugador
                    {
                        NombreUsuario = "Jugador2",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador2@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    contexto.Jugadores.Add(jugador1);
                    contexto.Jugadores.Add(jugador2);
                    contexto.SaveChanges();

                    idJugador1 = jugador1.JugadorId;
                    idJugador2 = jugador2.JugadorId;

                    contexto.Amistades.Add(new Amistad
                    {
                        JugadorId = idJugador1,
                        AmigoId = idJugador2,
                        EstadoAmistad = "Amigo",
                        EnLinea = true
                    });

                    contexto.SaveChanges();
                }

                var amistadDao = new AmistadDao();


                bool resultado = amistadDao.EsAmigo(idJugador1, idJugador2);

                Assert.True(resultado);
            }
        }

        [Fact]
        public void EsAmigo_DebeDevolverFalse_CuandoNoSonAmigos()
        {
            int idJugador1, idJugador2;

            using (var transaccion = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugador1 = new Jugador
                    {
                        NombreUsuario = "Jugador1",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador1@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    var jugador2 = new Jugador
                    {
                        NombreUsuario = "Jugador2",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador2@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    contexto.Jugadores.Add(jugador1);
                    contexto.Jugadores.Add(jugador2);
                    contexto.SaveChanges();

                    idJugador1 = jugador1.JugadorId;
                    idJugador2 = jugador2.JugadorId;
                }

                var amistadDao = new AmistadDao();

                bool resultado = amistadDao.EsAmigo(idJugador1, idJugador2);

                Assert.False(resultado);
            }
        }

        [Fact]
        public void EsAmigo_DebeDevolverFalse_CuandoJugadorNoExiste()
        {
            int idJugadorExistente;

            using (var transaccion = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugador = new Jugador
                    {
                        NombreUsuario = "JugadorExistente",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    contexto.Jugadores.Add(jugador);
                    contexto.SaveChanges();

                    idJugadorExistente = jugador.JugadorId;
                }

                var amistadDao = new AmistadDao();

                bool resultado = amistadDao.EsAmigo(idJugadorExistente, -1);

                Assert.False(resultado);
            }
        }
        
        [Fact]
        public void ObtenerIdJugadorSolicitantesAmistad_DebeDevolverSolicitantes_CuandoExistenSolicitudesDeAmistad()
        {
            int idJugador, idJugadorSolicitante1, idJugadorSolicitante2;

            using (var transaccion = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugador = new Jugador
                    {
                        NombreUsuario = "Jugador",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    var solicitante1 = new Jugador
                    {
                        NombreUsuario = "Solicitante1",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "solicitante1@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    var solicitante2 = new Jugador
                    {
                        NombreUsuario = "Solicitante2",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "solicitante2@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    contexto.Jugadores.Add(jugador);
                    contexto.Jugadores.Add(solicitante1);
                    contexto.Jugadores.Add(solicitante2);
                    contexto.SaveChanges();

                    idJugador = jugador.JugadorId;
                    idJugadorSolicitante1 = solicitante1.JugadorId;
                    idJugadorSolicitante2 = solicitante2.JugadorId;

                    // Crear solicitudes de amistad
                    contexto.Amistades.Add(new Amistad
                    {
                        JugadorId = idJugadorSolicitante1,
                        AmigoId = idJugador,
                        EstadoAmistad = "Request",
                        EnLinea = true
                    });

                    contexto.Amistades.Add(new Amistad
                    {
                        JugadorId = idJugadorSolicitante2,
                        AmigoId = idJugador,
                        EstadoAmistad = "Request",
                        EnLinea = true
                    });

                    contexto.SaveChanges();
                }

                var amistadDao = new AmistadDao();

                List<int> solicitantes = amistadDao.ObtenerIdJugadorSolicitantesAmistad(idJugador);

                Assert.Contains(idJugadorSolicitante1, solicitantes);
                Assert.Contains(idJugadorSolicitante2, solicitantes);
                Assert.Equal(2, solicitantes.Count);  // Esperamos 2 solicitantes
            }
        }

        [Fact]
        public void ObtenerIdJugadorSolicitantesAmistad_DebeDevolverListaVacia_CuandoNoExistenSolicitudesDeAmistad()
        {
            int idJugador;

            using (var transaccion = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugador = new Jugador
                    {
                        NombreUsuario = "Jugador",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugador@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    contexto.Jugadores.Add(jugador);
                    contexto.SaveChanges();

                    idJugador = jugador.JugadorId;
                }

                var amistadDao = new AmistadDao();


                List<int> solicitantes = amistadDao.ObtenerIdJugadorSolicitantesAmistad(idJugador);

                Assert.Empty(solicitantes);
            }
        }

        [Fact]
        public void ObtenerIdJugadorSolicitantesAmistad_DebeDevolverListaVacia_CuandoJugadorNoExistenSolicitudes()
        {
            int idJugadorInexistente = -1;

            var amistadDao = new AmistadDao();

            List<int> solicitantes = amistadDao.ObtenerIdJugadorSolicitantesAmistad(idJugadorInexistente);


            Assert.Empty(solicitantes);
        }



        [Fact]
        public void ActualizarSolicitudAmistad_Aceptada_DebeActualizarEstadoDeAmistad_CuandoLaSolicitudExiste()
        {
            int idJugadorActual, idJugadorAceptado;

            using (var transaccion = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {

                    var jugadorActual = new Jugador
                    {
                        NombreUsuario = Guid.NewGuid().ToString(),
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugadoractual@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    var jugadorAceptado = new Jugador
                    {
                        NombreUsuario = "JugadorAceptado",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugadoraceptado@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    contexto.Jugadores.Add(jugadorActual);
                    contexto.Jugadores.Add(jugadorAceptado);
                    contexto.SaveChanges();

                    idJugadorActual = jugadorActual.JugadorId;
                    idJugadorAceptado = jugadorAceptado.JugadorId;

                    contexto.Amistades.Add(new Amistad
                    {
                        JugadorId = idJugadorActual,
                        AmigoId = idJugadorAceptado,
                        EstadoAmistad = "Request",
                        EnLinea = true
                    });

                    contexto.SaveChanges();
                }

                var amistadDao = new AmistadDao();

                int rowsAffected = amistadDao.ActualizarSolicitudAmistad_Aceptada(idJugadorActual, idJugadorAceptado);

                using (var contexto = new ContextoBaseDatos())
                {
                    var amistadActualizada = contexto.Amistades
                        .FirstOrDefault(fs => fs.JugadorId == idJugadorActual && fs.AmigoId == idJugadorAceptado);

                    Assert.NotNull(amistadActualizada);
                    Assert.Equal("Amigo", amistadActualizada.EstadoAmistad);
                    Assert.Equal(1, rowsAffected);
                }
            }
        }

        [Fact]
        public void ActualizarSolicitudAmistad_Aceptada_NoDebeActualizarEstado_CuandoLaSolicitudNoExiste()
        {
            int idJugadorActual = 1;
            int idJugadorAceptado = 2;

            var amistadDao = new AmistadDao();

            int rowsAffected = amistadDao.ActualizarSolicitudAmistad_Aceptada(idJugadorActual, idJugadorAceptado);

            Assert.Equal(-1, rowsAffected);
        }

        [Fact]
        public void ActualizarSolicitudAmistad_Aceptada_NoDebeActualizarEstado_CuandoEstadoNoEsSolicitud()
        {
            int idJugadorActual, idJugadorAceptado;

            using (var transaccion = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugadorActual = new Jugador
                    {
                        NombreUsuario = "JugadorActual",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugadoractual@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    var jugadorAceptado = new Jugador
                    {
                        NombreUsuario = "JugadorAceptado",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugadoraceptado@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    contexto.Jugadores.Add(jugadorActual);
                    contexto.Jugadores.Add(jugadorAceptado);
                    contexto.SaveChanges();

                    idJugadorActual = jugadorActual.JugadorId;
                    idJugadorAceptado = jugadorAceptado.JugadorId;

                    contexto.Amistades.Add(new Amistad
                    {
                        JugadorId = idJugadorActual,
                        AmigoId = idJugadorAceptado,
                        EstadoAmistad = "Amigo",
                        EnLinea = true
                    });

                    contexto.SaveChanges();
                }

                var amistadDao = new AmistadDao();

                int rowsAffected = amistadDao.ActualizarSolicitudAmistad_Aceptada(idJugadorActual, idJugadorAceptado);

                Assert.Equal(-1, rowsAffected);
            }
        }
        
        [Fact]
        public void BorrarSolicitudAmistad_DeberiaEliminarSolicitud_CuandoLaSolicitudExiste()
        {
            int idJugadorActual, idJugadorRechazado;

            using (var transaccion = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugadorActual = new Jugador
                    {
                        NombreUsuario = "JugadorActual",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugadoractual@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    var jugadorRechazado = new Jugador
                    {
                        NombreUsuario = "JugadorRechazado",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugadorrechazado@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    contexto.Jugadores.Add(jugadorActual);
                    contexto.Jugadores.Add(jugadorRechazado);
                    contexto.SaveChanges();

                    idJugadorActual = jugadorActual.JugadorId;
                    idJugadorRechazado = jugadorRechazado.JugadorId;

                    contexto.Amistades.Add(new Amistad
                    {
                        JugadorId = idJugadorActual,
                        AmigoId = idJugadorRechazado,
                        EstadoAmistad = "Request",
                        EnLinea = true
                    });

                    contexto.SaveChanges();
                }

                var amistadDao = new AmistadDao();

                int rowsAffected = amistadDao.BorrarSolicitudAmistad(idJugadorActual, idJugadorRechazado);

                using (var contexto = new ContextoBaseDatos())
                {
                    var solicitudEliminada = contexto.Amistades
                        .FirstOrDefault(fs => fs.JugadorId == idJugadorActual && fs.AmigoId == idJugadorRechazado);

                    Assert.Null(solicitudEliminada);
                    Assert.Equal(1, rowsAffected);
                }
            }
        }

        [Fact]
        public void BorrarSolicitudAmistad_NoDebeEliminarSolicitud_CuandoLaSolicitudNoExiste()
        {
            int idJugadorActual = 1;
            int idJugadorRechazado = 2;

            var amistadDao = new AmistadDao();

            int rowsAffected = amistadDao.BorrarSolicitudAmistad(idJugadorActual, idJugadorRechazado);

            Assert.Equal(-1, rowsAffected);
        }

        [Fact]
        public void BorrarSolicitudAmistad_NoDebeEliminarSolicitud_CuandoElEstadoNoEsSolicitud()
        {
            int idJugadorActual, idJugadorRechazado;

            using (var transaccion = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugadorActual = new Jugador
                    {
                        NombreUsuario = "JugadorActual",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugadoractual@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    var jugadorRechazado = new Jugador
                    {
                        NombreUsuario = "JugadorRechazado",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugadorrechazado@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    contexto.Jugadores.Add(jugadorActual);
                    contexto.Jugadores.Add(jugadorRechazado);
                    contexto.SaveChanges();

                    idJugadorActual = jugadorActual.JugadorId;
                    idJugadorRechazado = jugadorRechazado.JugadorId;

                    contexto.Amistades.Add(new Amistad
                    {
                        JugadorId = idJugadorActual,
                        AmigoId = idJugadorRechazado,
                        EstadoAmistad = "Amigo",
                        EnLinea = true
                    });

                    contexto.SaveChanges();
                }

                var amistadDao = new AmistadDao();

                int rowsAffected = amistadDao.BorrarSolicitudAmistad(idJugadorActual, idJugadorRechazado);

                Assert.Equal(-1, rowsAffected);
            }
        }

        [Fact]
        public void BorrarAmistad_DeberiaEliminarAmistad_CuandoLaAmistadExiste()
        {
            int idJugadorActual, idJugadorAmigo;

            using (var transaccion = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugadorActual = new Jugador
                    {
                        NombreUsuario = "JugadorActual",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugadoractual@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    var jugadorAmigo = new Jugador
                    {
                        NombreUsuario = "JugadorAmigo",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugadoramigo@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    contexto.Jugadores.Add(jugadorActual);
                    contexto.Jugadores.Add(jugadorAmigo);
                    contexto.SaveChanges();

                    idJugadorActual = jugadorActual.JugadorId;
                    idJugadorAmigo = jugadorAmigo.JugadorId;

                    contexto.Amistades.Add(new Amistad
                    {
                        JugadorId = idJugadorActual,
                        AmigoId = idJugadorAmigo,
                        EstadoAmistad = "Amigo",
                        EnLinea = true
                    });

                    contexto.SaveChanges();
                }

                var amistadDao = new AmistadDao();

                int rowsAffected = amistadDao.BorrarAmistad(idJugadorActual, idJugadorAmigo);

                using (var contexto = new ContextoBaseDatos())
                {
                    var amistadEliminada = contexto.Amistades
                        .FirstOrDefault(fs => fs.JugadorId == idJugadorActual && fs.AmigoId == idJugadorAmigo);

                    Assert.Null(amistadEliminada);
                    Assert.Equal(1, rowsAffected);
                }
            }
        }

        [Fact]
        public void BorrarAmistad_NoDebeEliminarAmistad_CuandoLaAmistadNoExiste()
        {
            int idJugadorActual = 1;
            int idJugadorAmigo = 2;

            var amistadDao = new AmistadDao();

            int rowsAffected = amistadDao.BorrarAmistad(idJugadorActual, idJugadorAmigo);

            Assert.Equal(-1, rowsAffected);
        }

        [Fact]
        public void BorrarAmistad_NoDebeEliminarAmistad_CuandoElEstadoNoEsAmistad()
        {
            int idJugadorActual, idJugadorAmigo;

            using (var transaccion = new TransactionScope())
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugadorActual = new Jugador
                    {
                        NombreUsuario = "JugadorActual",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugadoractual@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    var jugadorAmigo = new Jugador
                    {
                        NombreUsuario = "JugadorAmigo",
                        NumeroFotoPerfil = 1,
                        Cuenta = new Cuenta
                        {
                            Correo = "jugadoramigo@example.com",
                            ContraseniaHash = "hash_password",
                            Salt = "salt"
                        }
                    };

                    contexto.Jugadores.Add(jugadorActual);
                    contexto.Jugadores.Add(jugadorAmigo);
                    contexto.SaveChanges();

                    idJugadorActual = jugadorActual.JugadorId;
                    idJugadorAmigo = jugadorAmigo.JugadorId;

                    contexto.Amistades.Add(new Amistad
                    {
                        JugadorId = idJugadorActual,
                        AmigoId = idJugadorAmigo,
                        EstadoAmistad = "Request",
                        EnLinea = true
                    });

                    contexto.SaveChanges();
                }

                var amistadDao = new AmistadDao();

                int rowsAffected = amistadDao.BorrarAmistad(idJugadorActual, idJugadorAmigo);

                Assert.Equal(-1, rowsAffected);
            }
        }


        [Fact]
        public void ObtenerAmigos_DeberiaDevolverAmigos_CuandoElJugadorTieneAmigos()
        {
            int idJugadorActual;

            using (var contexto = new ContextoBaseDatos())
            {
                var jugadorActual = new Jugador
                {
                    NombreUsuario = "JugadorActual",
                    NumeroFotoPerfil = 1,
                    Cuenta = new Cuenta
                    {
                        Correo = "jugadoractual@example.com",
                        ContraseniaHash = "hash_password",
                        Salt = "salt"
                    }
                };

                var jugadorAmigo1 = new Jugador
                {
                    NombreUsuario = "Amigo1",
                    NumeroFotoPerfil = 1,
                    Cuenta = new Cuenta
                    {
                        Correo = "amigo1@example.com",
                        ContraseniaHash = "hash_password",
                        Salt = "salt"
                    }
                };

                var jugadorAmigo2 = new Jugador
                {
                    NombreUsuario = "Amigo2",
                    NumeroFotoPerfil = 1,
                    Cuenta = new Cuenta
                    {
                        Correo = "amigo2@example.com",
                        ContraseniaHash = "hash_password",
                        Salt = "salt"
                    }
                };

                contexto.Jugadores.Add(jugadorActual);
                contexto.Jugadores.Add(jugadorAmigo1);
                contexto.Jugadores.Add(jugadorAmigo2);
                contexto.SaveChanges();

                idJugadorActual = jugadorActual.JugadorId;

                contexto.Amistades.Add(new Amistad
                {
                    JugadorId = idJugadorActual,
                    AmigoId = jugadorAmigo1.JugadorId,
                    EstadoAmistad = "Amigo",
                    EnLinea = true
                });

                contexto.Amistades.Add(new Amistad
                {
                    JugadorId = idJugadorActual,
                    AmigoId = jugadorAmigo2.JugadorId,
                    EstadoAmistad = "Amigo",
                    EnLinea = true
                });

                contexto.SaveChanges();
            }

            var amistadDao = new AmistadDao();

            List<string> amigos = amistadDao.ObtenerAmigos(idJugadorActual);

            Assert.Contains("Amigo1", amigos);
            Assert.Contains("Amigo2", amigos);


        }





    }
}
