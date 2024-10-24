﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego
{
    [ServiceContract(CallbackContract = typeof(ILobbyManagerCallback))]
    public interface ILobbyManager
    {
        [OperationContract(IsOneWay = true)]
        void CreateLobby(LobbyPlayer jugador);

        [OperationContract(IsOneWay = true)]
        void JoinLobby(string lobbyCode, LobbyPlayer lobbyPlayer);

        [OperationContract(IsOneWay = true)]
        void JoinLobbyAsHost(string lobbyCode);

        [OperationContract]
        void ExitLobby(string lobbyCode, string username);

        [OperationContract]
        void sendMessage(string mensaje);

        [OperationContract]
        string BuscarLobbyDisponible();





    }

    [ServiceContract]
    public interface ILobbyManagerCallback
    {
        [OperationContract]
        void NotifyLobbyCreated(string lobbyCode);

        [OperationContract]
        void NotifyPlayersInLobby(string lobbyCode, List<LobbyPlayer> lobbyPlayers);

        [OperationContract]
        void NotifyPlayerJoinToLobby(LobbyPlayer lobbyPlayer, int numOfPlayersInLobby);

        [OperationContract]
        void NotifyPlayerLeftLobby(string username);

        [OperationContract]
        void NotifyHostPlayerLeftLobby();

        [OperationContract]
        void NotifyStartOfMatch();

        [OperationContract]
        void NotifyLobbyIsFull();

        [OperationContract]
        void NotifyLobbyDoesNotExist();

        [OperationContract]
        void NotifyExpulsedFromLobby();
        // Nuevo método para recibir mensajes de chat
        [OperationContract]
        void ReceiveMessage(string username, string message);
    }

    [DataContract]
    public class LobbyPlayer
    {
        [DataMember]
        public string Username { get; set; }

  

        public ILobbyManagerCallback CallbackChannel { get; set; }


        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                LobbyPlayer otherPlayer = (LobbyPlayer)obj;
                return Username == otherPlayer.Username;
            }
        }

        public override int GetHashCode()
        {
            return Username.GetHashCode();
        }
    }
}
