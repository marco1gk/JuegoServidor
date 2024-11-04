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
        void RegisterUserToOnlineUsers(int idPlayer, string username);

        [OperationContract(IsOneWay = true)]
        void UnregisterUserToOnlineUsers(string username);
    }
    [ServiceContract]
    public interface IGestorUsuarioCallback
    {
        [OperationContract]
        void NotifyUserLoggedIn(string username);

        [OperationContract]
        void NotifyUserLoggedOut(string username);

        [OperationContract]
        void NotifyOnlineFriends(List<string> onlineUsernames);
    }
}
