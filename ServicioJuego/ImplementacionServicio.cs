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
        public bool AgregarJugador(JugadorDataContract jugador, CuentaDataContract cuenta)
        {

          
            Jugador Jugador = new Jugador();
            Jugador.NombreUsuario = jugador.NombreUsuario;
            Jugador.NumeroFotoPerfil = jugador.NumeroFotoPerfil;
            Cuenta Cuenta = new Cuenta();
            Cuenta.Correo = cuenta.Correo;
            Cuenta.ContraseniaHash=cuenta.ContraseniaHash;  
            CuentaDao CuentaDao = new CuentaDao();
            return CuentaDao.AgregarJugadorConCuenta(Jugador, Cuenta);
        }

        public bool GetData(JugadorDataContract jugador, CuentaDataContract cuent)
        {
            Jugador Jugador = new Jugador();
            Cuenta Cuenta = new Cuenta();
            CuentaDao CuentaDao = new CuentaDao();
            return CuentaDao.AgregarJugadorConCuenta(Jugador, Cuenta);
        }
        }


    }

