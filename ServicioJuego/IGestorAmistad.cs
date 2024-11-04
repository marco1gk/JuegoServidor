﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego
{
    [ServiceContract]
    public interface IGestorAmistad
    {
        [OperationContract]
        
        List<string> GetListUsernameFriends(int idPlayer);

        [OperationContract]
       
        bool ValidateFriendRequestSending(int idPlayerSender, string usernamePlayerRequested);

        [OperationContract]

        int AddRequestFriendship(int idPlayerSender, string usernamePlayerRequested);

        [OperationContract]
        List<string> GetUsernamePlayersRequesters(int idPlayer);
    }
}
