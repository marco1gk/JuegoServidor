using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego
{

    [ServiceContract(CallbackContract = typeof(IGestorDeSolicitudesDeAmistadCallBack))]
    public interface IGestorDeSolicitudesDeAmistad
    {
        [OperationContract(IsOneWay = true)]
        void AgregarADiccionarioAmistadesEnLinea(string nombreUsuarioDeActualJugador);

        [OperationContract(IsOneWay = true)]
        void EnviarSolicitudAmistad(string nombreUsuarioJugadorRemitente, string nombreUsuarioJugadorSolicitado);

        [OperationContract(IsOneWay = true)]
        void AceptarSolicitudAmistad(int idJugadorSolicitado, string nombreUsuarioJugadorSolicitado, string nombreUsuarioJugadorRemitente);

        [OperationContract(IsOneWay = true)]
        void RechazarSolicitudAmistad(int idJugadorActual, string nombreUsuario);

        [OperationContract(IsOneWay = true)]
        void EliminarAmigo(int idJugadorActual, string nombreUsuarioDeActualJugador, string nombreUsuarioAmigoEliminado);

        [OperationContract(IsOneWay = true)]
        void EliminarDeDiccionarioAmistadesEnLinea(string nombreUsuario);

    }

    [ServiceContract]
    public interface IGestorDeSolicitudesDeAmistadCallBack
    {
        [OperationContract]
        void NotificarNuevaSolicitudAmistad(string nombreUsuario);

        [OperationContract]
        void NotificarSolicitudAmistadAceptada(string nombreUsuario);

        [OperationContract]
        void NotificarAmigoEliminado(string nombreUsuario);
    }
}
