using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
namespace ServicioJuego
{

    [ServiceContract(CallbackContract = typeof(IGestorUsuarioCallback))]
    public interface IGestorUsuariosConectados
    {

        [OperationContract(IsOneWay = true)]
        void RegistrarUsuarioAUsuariosConectados(int idJugador, string nombreUsuario);

        [OperationContract(IsOneWay = true)]
        void DesregistrarUsuarioDeUsuariosEnLinea(string nombreUsuario);
    }
    [ServiceContract]
    public interface IGestorUsuarioCallback
    {

        [OperationContract(IsOneWay = true)]
        void NotificarInvitacionSala(string nombreInvitador, string codigoSalaEspera);

        [OperationContract]
        void NotificarUsuarioConectado(string nombreUsuario);

        [OperationContract]
        void NotificarUsuarioDesconectado(string nombreUsuario);

        [OperationContract]
        void NotificarAmigosEnLinea(List<string> nombresUsuariosEnLinea);

        // Método de ping para verificar conexión
        [OperationContract(IsOneWay = true)]
        void Ping();
    }
}
