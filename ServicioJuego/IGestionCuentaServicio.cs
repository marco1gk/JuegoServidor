﻿using AccesoDatos.Modelo;
using ServicioJuego.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServicioJuego
{
    [ServiceContract]
    public interface IGestionCuentaServicio
    {

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        bool AgregarJugador(JugadorDataContract jugador);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        string ObtenerNombreUsuarioPorIdJugador(int idJugador);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        bool EditarContraseña(string correo, string nuevaContraseña);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        JugadorDataContract ValidarInicioSesion(string nombreUsuario, string contraseniaHash);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        JugadorDataContract ObtenerJugador(int idJugador);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        bool EditarNombreUsuario(int idJugador, string nuevoNombreUsuario);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        bool EditarCorreo(int idCuenta, string nuevoCorreo);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        string EnviarCodigoConfirmacion(string correo);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        bool ValidarCodigo(string codigoIngresado, string codigoEnviado);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        bool ExisteCorreo(string correo);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        bool ExisteNombreUsuario(string nombreUsuario);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        bool VerificarContrasena(string contraseniaIngresada, string correo);

        [OperationContract]
        bool ValidarUsuarioEnLinea(string nombreUsuario);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        int ObtenerIdJugadorPorNombreUsuario(string nombreUsuario);

    }

    [DataContract]
    public class JugadorDataContract
    {

        [DataMember]
        public int JugadorId { get; set; }

        [DataMember]
        public string NombreUsuario { get; set; }

        [DataMember]
        public int NumeroFotoPerfil { get; set; }

        [DataMember]
        public string Correo { get; set; }

        [DataMember]
        public string ContraseniaHash { get; set; }

        [DataMember]
        public bool EsInvitado { get; set; }    
    }

}




