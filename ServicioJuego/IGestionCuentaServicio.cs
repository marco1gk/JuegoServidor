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
         bool GetData(JugadorDataContract jugador, CuentaDataContract cuent);

        [OperationContract]
         bool AgregarJugador(JugadorDataContract jugador, CuentaDataContract cuenta);
    }

    [DataContract]
    public class JugadorDataContract
    {
        [DataMember]
        public string NombreUsuario { get; set; }
       
        [DataMember]
        public int NumeroFotoPerfil  { get; set; }



    }

    //falta ver si se necesita la cuenta y el correo

    [DataContract]
     
    public class CuentaDataContract
    {
        [DataMember]
        public string Correo {  get; set; }

        [DataMember]
        public string ContraseniaHash { get; set; }



    }
}




