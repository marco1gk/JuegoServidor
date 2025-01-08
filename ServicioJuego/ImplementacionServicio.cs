using AccesoDatos;
using AccesoDatos.DAO;
using AccesoDatos.Modelo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.Entity.Core;
using AccesoDatos.Excepciones;
using ServicioJuego.Excepciones;
using AccesoDatos.Utilidades;

namespace ServicioJuego
{
    public partial class ImplementacionServicio : IGestionCuentaServicio
    {
        public string ObtenerNombreUsuarioPorIdJugador(int idJugador)
        {
            string nombreUsuario = string.Empty;

            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugador = contexto.Jugadores
                        .FirstOrDefault(p => p.JugadorId == idJugador);

                    if (jugador != null)
                    {
                        nombreUsuario = jugador.NombreUsuario;
                    }
                }
            }
            catch (ExcepcionAccesoDatos ex)
            {
                HuntersTrophyExcepcion respuestaExcepcion = new HuntersTrophyExcepcion
                {
                    Mensaje = ex.Message,
                    StackTrace = ex.StackTrace
                };
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new FaultException<HuntersTrophyExcepcion>(respuestaExcepcion, new FaultReason(respuestaExcepcion.Mensaje));
            }
            catch (EntityException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
            }
            catch (SqlException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(ex);
                
            }

            return nombreUsuario;
        }

        //se regresa null debido a que solo se encuentra un sentido en que no existe, de esa manera se valida de forma más fácil
        public JugadorDataContract ObtenerJugador(int idJugador)
        {

            try
            {
                JugadorDao JugadorDao = new JugadorDao();
                Jugador jugador = JugadorDao.ObtenerJugador(idJugador);

                if (jugador != null && jugador.Cuenta != null)
                {
                    return new JugadorDataContract
                    {
                        NombreUsuario = jugador.NombreUsuario,
                        NumeroFotoPerfil = jugador.NumeroFotoPerfil,
                        Correo = jugador.Cuenta.Correo,
                        ContraseniaHash = jugador.Cuenta.ContraseniaHash
                    };
                }
            }
            catch (ExcepcionAccesoDatos ex)
            {
                HuntersTrophyExcepcion respuestaExcepcion = new HuntersTrophyExcepcion
                {
                    Mensaje = ex.Message,
                    StackTrace = ex.StackTrace
                };
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new FaultException<HuntersTrophyExcepcion>(respuestaExcepcion, new FaultReason(respuestaExcepcion.Mensaje));
            }

            return null;
        }

        public bool ValidarUsuarioEnLinea(string nombreUsuario)
        {
            bool esEnLinea = true;

            if (!usuariosEnLinea.ContainsKey(nombreUsuario))
            {
                esEnLinea = false;
            }

            return esEnLinea;
        }
        public bool AgregarJugador(JugadorDataContract jugador)
        {
            try
            {
                Jugador jugadorAux = new Jugador();
                Cuenta cuentaAux = new Cuenta();
                CuentaDao cuentaDao = new CuentaDao();

                string salt = Recursos.GenerarSalt();
                string contraseniaHash = Recursos.HashearContrasena(jugador.ContraseniaHash, salt);

                jugadorAux.NombreUsuario = jugador.NombreUsuario;
                jugadorAux.NumeroFotoPerfil = jugador.NumeroFotoPerfil;
                cuentaAux.Correo = jugador.Correo;
                cuentaAux.Salt = salt;
                cuentaAux.ContraseniaHash = contraseniaHash;

                return cuentaDao.AgregarJugadorConCuenta(jugadorAux, cuentaAux);
            }
            catch (SqlException ex) when (ex.Number == 18456) 
            {
                HuntersTrophyExcepcion respuestaExcepcion = new HuntersTrophyExcepcion
                {
                    Mensaje = "No se pudo conectar a la base de datos. Verifique las credenciales.",
                    StackTrace = ex.StackTrace
                };
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new FaultException<HuntersTrophyExcepcion>(respuestaExcepcion, new FaultReason(respuestaExcepcion.Mensaje));
            }
            catch (ExcepcionAccesoDatos ex)
            {
                HuntersTrophyExcepcion respuestaExcepcion = new HuntersTrophyExcepcion
                {
                    Mensaje = ex.Message,
                    StackTrace = ex.StackTrace
                };
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new FaultException<HuntersTrophyExcepcion>(respuestaExcepcion, new FaultReason(respuestaExcepcion.Mensaje));
            }
            catch (Exception ex)
            {
                HuntersTrophyExcepcion respuestaExcepcion = new HuntersTrophyExcepcion
                {
                    Mensaje = "Ocurrió un error inesperado. Por favor, intente más tarde.",
                    StackTrace = ex.StackTrace
                };
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new FaultException<HuntersTrophyExcepcion>(respuestaExcepcion, new FaultReason(respuestaExcepcion.Mensaje));
            }
        }

        public bool ExisteCorreo(string correo)
        {
            CuentaDao cuentaDao = new CuentaDao();
            return cuentaDao.ExisteCorreo(correo);
        }

        public bool ExisteNombreUsuario(string nombreUsuario)
        {
            CuentaDao cuentaDao = new CuentaDao();
            return cuentaDao.ExisteNombreUsuario(nombreUsuario);
        }


        //se regresa null debido a que solo se encuentra un sentido en que no existe, de esa manera se valida de forma más fácil
        public JugadorDataContract ValidarInicioSesion(string nombreUsuario, string contraseniaHash)
        {
           
            try
            {
                CuentaDao cuentaDao = new CuentaDao();
                var cuenta = cuentaDao.ObtenerCuentaPorNombreUsuario(nombreUsuario);
                if (cuenta != null)
                {
                    bool esValido = Recursos.VerificarContrasena(contraseniaHash, cuenta);
                    Console.WriteLine($"Verificando contraseña para el usuario: {nombreUsuario}");
                    Console.WriteLine($"Salt: {cuenta.Salt}");
                    Console.WriteLine($"Hash de la contraseña ingresada: {Recursos.HashearContrasena(contraseniaHash, cuenta.Salt)}");
                    if (esValido)
                    {
                        return new JugadorDataContract
                        {
                            JugadorId = cuenta.Jugador.JugadorId,
                            NombreUsuario = cuenta.Jugador.NombreUsuario,
                            Correo = cuenta.Correo,
                            NumeroFotoPerfil = cuenta.Jugador.NumeroFotoPerfil
                        };
                    }

                }

            }
            catch (ExcepcionAccesoDatos ex)
            {
                HuntersTrophyExcepcion respuestaExcepcion = new HuntersTrophyExcepcion
                {
                    Mensaje = ex.Message,
                    StackTrace = ex.StackTrace
                };
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new FaultException<HuntersTrophyExcepcion>(respuestaExcepcion, new FaultReason(respuestaExcepcion.Mensaje));
            }
           

            return null; 
        }

        public bool EditarContraseña(string correo, string nuevaContraseña)
        {
            CuentaDao cuentaDao = new CuentaDao();
            return cuentaDao.EditarContraseñaPorCorreo(correo, nuevaContraseña);
        }

        public bool VerificarContrasena(string contraseniaIngresada, string correo)
        {
            CuentaDao cuentaDao = new CuentaDao();
            var cuenta = cuentaDao.ObtenerCuentaPorNombreUsuario(correo);
            if (cuenta != null)
            {
                if (Recursos.VerificarContrasena(contraseniaIngresada, cuenta))
                    return true;
                else
                    return false;
            }
            else
                return false;
        
        }


        public int ObtenerIdJugadorPorNombreUsuario(string nombreUsuario)
        {
            CuentaDao dataAccess = new CuentaDao();
            try
            {
                return dataAccess.ObtenerIdJugadorPorNombreUsuario(nombreUsuario);
            }
            catch (ExcepcionAccesoDatos ex)
            {
                HuntersTrophyExcepcion exceptionResponse = new HuntersTrophyExcepcion
                {
                    Mensaje = ex.Message,
                    StackTrace = ex.StackTrace
                };
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new FaultException<HuntersTrophyExcepcion>(exceptionResponse, new FaultReason(exceptionResponse.Mensaje));
            }
        }


        public bool EditarNombreUsuario(int idJugador, string nuevoNombreUsuario)
        {
            JugadorDao jugador = new JugadorDao();
            return jugador.EditarNombreUsuario(idJugador, nuevoNombreUsuario);
        }

        public bool EditarCorreo(int idCuenta, string nuevoCorreo)
        {
            CuentaDao cuenta = new CuentaDao();
            return cuenta.EditarCorreo(idCuenta, nuevoCorreo);
        }

        public string EnviarCodigoConfirmacion(string correo)
        {
            Recursos recurso = new Recursos();
            return recurso.EnviarCodigoConfirmacion(correo);
        }

        public bool ValidarCodigo(string codigoIngresado, string codigoEnviado)
        {
            Recursos recurso = new Recursos();
            return recurso.ValidarCodigo(codigoIngresado, codigoEnviado);
        }


    }


}

