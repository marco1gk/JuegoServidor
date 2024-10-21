using AccesoDatos.DAO;
using AccesoDatos.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
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

            jugadorAux.NombreUsuario= jugador.NombreUsuario;
            jugadorAux.NumeroFotoPerfil= jugador.NumeroFotoPerfil;
            cuentaAux.Correo= jugador.Correo;   
            cuentaAux.ContraseniaHash= jugador.ContraseniaHash;
            return cuentaDao.AgregarJugadorConCuenta(jugadorAux, cuentaAux);
          
            /*Jugador Jugador = new Jugador();
            Jugador.NombreUsuario = jugador.NombreUsuario;
            Jugador.NumeroFotoPerfil = jugador.NumeroFotoPerfil;
            Cuenta Cuenta = new Cuenta();
            Cuenta.Correo = cuenta.Correo;
            Cuenta.ContraseniaHash=cuenta.ContraseniaHash;  
            CuentaDao CuentaDao = new CuentaDao();
            return CuentaDao.AgregarJugadorConCuenta(Jugador, Cuenta);*/
        }
         public JugadorDataContract ValidarInicioSesion(string nombreUsuario, string contraseniaHash)
        {
            CuentaDao cuentaDao = new CuentaDao();
            var cuenta = cuentaDao.ValidarInicioSesion(nombreUsuario, contraseniaHash); // Aquí devuelves la cuenta completa

            if (cuenta != null)
            {
                // Si se encuentra la cuenta, retornas los datos del jugador en un DataContract
                return new JugadorDataContract
                {
                    JugadorId = cuenta.Jugador.JugadorId,
                    NombreUsuario = cuenta.Jugador.NombreUsuario,
                    Correo = cuenta.Correo, 
                    NumeroFotoPerfil = cuenta.Jugador.NumeroFotoPerfil
                };
            }

            return null; // Si no existe, retornas null
        }
        public bool EditarContraseña(string correo, string nuevaContraseña)
        {
            throw new NotImplementedException();
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
                    Correo = jugador.Cuenta.Correo
                };
            }
            return null;
        }


    }


}

