using AccesoDatos.DAO;
using AccesoDatos.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServicioJuego
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código y en el archivo de configuración a la vez.
    public partial class ImplementacionServicio : IGestionCuentaServicio
    {
        public bool AgregarJugador(JugadorDataContract jugador)
        {
            Jugador jugadorAux = new Jugador();
            Cuenta cuentaAux = new Cuenta();
            CuentaDao cuentaDao = new CuentaDao();

            string salt = Recursos.GenerarSalt();
            string contraseniaHash = Recursos.HashearContrasena(jugador.ContraseniaHash, salt);

            jugadorAux.NombreUsuario= jugador.NombreUsuario;
            jugadorAux.NumeroFotoPerfil= jugador.NumeroFotoPerfil;
            cuentaAux.Correo= jugador.Correo;
            cuentaAux.Salt = salt;
            cuentaAux.ContraseniaHash = contraseniaHash;
            return cuentaDao.AgregarJugadorConCuenta(jugadorAux, cuentaAux);
          
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

        public JugadorDataContract ValidarInicioSesion(string nombreUsuario, string contraseniaHash)
        {
            CuentaDao cuentaDao = new CuentaDao();
            var cuenta = cuentaDao.ObtenerCuentaPorNombreUsuario(nombreUsuario);

            if (cuenta != null)
            {
                bool esValido = Recursos.VerificarContrasena(contraseniaHash, cuenta);
                return new JugadorDataContract
                {
                    JugadorId = cuenta.Jugador.JugadorId,
                    NombreUsuario = cuenta.Jugador.NombreUsuario,
                    Correo = cuenta.Correo, 
                    NumeroFotoPerfil = cuenta.Jugador.NumeroFotoPerfil
                };
            }

            return null; 
        }
        public bool EditarContraseña(string correo, string nuevaContraseña)
        {
            CuentaDao cuenta = new CuentaDao();
            return cuenta.EditarContraseñaPorCorreo(correo, nuevaContraseña);
        }

        public JugadorDataContract ObtenerJugador(int idJugador)
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
            return null;
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

