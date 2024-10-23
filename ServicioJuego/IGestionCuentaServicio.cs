using AccesoDatos.Modelo;
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
         bool AgregarJugador(JugadorDataContract jugador);

        [OperationContract]
        bool EditarContraseña(string correo,string nuevaContraseña);

        [OperationContract]
        JugadorDataContract ValidarInicioSesion(string nombreUsuario, string contraseniaHash);

        [OperationContract]
        JugadorDataContract ObtenerJugador(int idJugador);

        [OperationContract]
        bool EditarNombreUsuario(int idJugador, string nuevoNombreUsuario);

        [OperationContract]
        bool EditarCorreo(int idCuenta, string nuevoCorreo);

        [OperationContract]
        string EnviarCodigoConfirmacion(string correo);

        [OperationContract]
        bool ValidarCodigo(string codigoIngresado, string codigoEnviado);

        [OperationContract]
        bool ExisteCorreo(string correo);

        [OperationContract]
        bool ExisteNombreUsuario(string nombreUsuario);



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
    }

}




