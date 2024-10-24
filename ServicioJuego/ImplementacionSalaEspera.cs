using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServicioJuego
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.PerSession)]
    public partial class ImplementacionServicio : ILobbyManager
    {
        private static readonly Dictionary<string, List<LobbyPlayer>> lobbies = new Dictionary<string, List<LobbyPlayer>>();

        public void CreateLobby(LobbyPlayer lobbyPlayer)
        {

            ILobbyManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();
            lobbyPlayer.CallbackChannel = currentUserCallbackChannel;

            List<LobbyPlayer> players = new List<LobbyPlayer> { lobbyPlayer };
            string lobbyCode = GenerateLobbyCode();

            lobbies.Add(lobbyCode, players);

            try
            {
                currentUserCallbackChannel.NotifyLobbyCreated(lobbyCode);
            }
            catch (CommunicationException ex)
            
            {

                Console.WriteLine(ex.ToString());
                PerformExitLobby(lobbyCode, lobbyPlayer.Username, false);
            }
            catch (TimeoutException ex)
            {

                Console.WriteLine(ex.ToString());

                PerformExitLobby(lobbyCode, lobbyPlayer.Username, false);
            }
            Console.WriteLine("esta jalando alv");

        }

        public string BuscarLobbyDisponible()
        {
            // Si hay un lobby disponible, devolver su código
            if (lobbies.Any())
            {
                return lobbies.Keys.First(); // Devuelve el primer lobby disponible
            }

            // Si no hay lobbies disponibles, devuelve null
            return null;
        }


        public void JoinLobbyAsHost(string lobbyCode)
        {
            if (lobbies.ContainsKey(lobbyCode))
            {
                ILobbyManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();
                List<LobbyPlayer> players = lobbies[lobbyCode];
                LobbyPlayer hostPlayer = players[0];

                hostPlayer.CallbackChannel = currentUserCallbackChannel;

                try
                {
                    currentUserCallbackChannel.NotifyLobbyCreated(lobbyCode);
                }
                catch (CommunicationException ex)
                {

                    Console.WriteLine(ex.ToString());
                    PerformExitLobby(lobbyCode, hostPlayer.Username, false);
                }
            }
        }

        public void JoinLobby(string lobbyCode, LobbyPlayer lobbyPlayer)
        {
            ILobbyManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();
            lobbyPlayer.CallbackChannel = currentUserCallbackChannel;
            int maxSizePlayers = 4;

            try
            {
                if (lobbies.ContainsKey(lobbyCode))
                {
                    List<LobbyPlayer> playersInLobby = lobbies[lobbyCode];
                    int numOfPlayersInLobby = playersInLobby.Count;

                    if (numOfPlayersInLobby < maxSizePlayers)
                    {
                        lobbyPlayer.CallbackChannel.NotifyPlayersInLobby(lobbyCode, playersInLobby);
                        NotifyPlayerJoinToLobby(playersInLobby, lobbyPlayer, numOfPlayersInLobby, lobbyCode);
                        playersInLobby.Add(lobbyPlayer);
                    }
                    else
                    {
                        lobbyPlayer.CallbackChannel.NotifyLobbyIsFull();
                    }
                }
                else
                {
                    lobbyPlayer.CallbackChannel.NotifyLobbyDoesNotExist();
                }
            }
            catch (CommunicationException ex)
            {
            Console.WriteLine(ex.ToString());
            }
        }

        private void NotifyPlayerJoinToLobby(List<LobbyPlayer> playersInLobby, LobbyPlayer playerEntering, int numOfPlayersInLobby, string lobbyCode)
        {
            foreach (var player in playersInLobby.ToList())
            {
                try
                {
                    player.CallbackChannel.NotifyPlayerJoinToLobby(playerEntering, numOfPlayersInLobby);
                }
                catch (CommunicationException ex)
                {

                    Console.WriteLine(ex.ToString());
                    PerformExitLobby(lobbyCode, player.Username, false);
                }
            }
        }

  

        public void ExitLobby(string lobbyCode, string username)
        {
            PerformExitLobby(lobbyCode, username, false);
        }

        public void ExpulsePlayerFromLobby(string lobbyCode, string username)
        {
            PerformExitLobby(lobbyCode, username, true);
        }


        private void PerformExitLobby(string lobbyCode, string username, bool isExpulsed)
        {
            if (lobbies.ContainsKey(lobbyCode))
            {
                List<LobbyPlayer> players = lobbies[lobbyCode];
                LobbyPlayer playerToEliminate = null;

                int hostIndex = 0;
                int eliminatedPlayerIndex = hostIndex;

                foreach (LobbyPlayer player in players)
                {
                    if (player.Username.Equals(username))
                    {
                        playerToEliminate = player;
                        break;
                    }
                    else
                    {
                        eliminatedPlayerIndex++;
                    }
                }

                if (isExpulsed)
                {
                    
                }

                players.Remove(playerToEliminate);
                lobbies[lobbyCode] = players;

                NotifyPlayerLeftLobby(players, username, eliminatedPlayerIndex, lobbyCode, isExpulsed);

                if (eliminatedPlayerIndex == hostIndex)
                {
                    lobbies.Remove(lobbyCode);
                }
            }
        }

      

        private void NotifyPlayerLeftLobby(List<LobbyPlayer> players, string username, int eliminatedPlayerIndex, string lobbyCode, bool isExpulsed)
        {
            int hostIndex = 0;

            foreach (var callbackChannel in players.Select(p => p.CallbackChannel).ToList())
            {
                try
                {
                    if (eliminatedPlayerIndex != hostIndex)
                    {
                        callbackChannel.NotifyPlayerLeftLobby(username);
                    }
                    else
                    {
                        callbackChannel.NotifyHostPlayerLeftLobby();
                    }
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine(ex.ToString());
                    PerformExitLobby(lobbyCode, username, isExpulsed);
                }
            }
        }

        private string GenerateLobbyCode()
        {
            int length = 6;
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();

            char[] code = new char[length];

            for (int i = 0; i < length; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }

            string lobbyCode = new string(code);

            return lobbies.ContainsKey(lobbyCode) ? GenerateLobbyCode() : lobbyCode;
        }

        public void sendMessage(string mensaje)
        {
            // Obtener el canal de callback del jugador que envía el mensaje
            ILobbyManagerCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyManagerCallback>();

            // Buscar el lobby en el que está el jugador que envía el mensaje
            string lobbyCode = lobbies
                .Where(lobby => lobby.Value.Any(player => player.CallbackChannel == currentUserCallbackChannel)) // Buscar el lobby del jugador
                .Select(lobby => lobby.Key) // Obtener el código del lobby
                .FirstOrDefault();

            // Si el lobby no existe, el jugador no está en un lobby válido
            if (lobbyCode == null)
            {
                Console.WriteLine("No se pudo encontrar el lobby del jugador que envía el mensaje.");
                return;
            }

            // Obtener el nombre del jugador que envía el mensaje
            string sendingUsername = lobbies[lobbyCode]
                .Where(player => player.CallbackChannel == currentUserCallbackChannel)
                .Select(player => player.Username)
                .FirstOrDefault();

            // Verificar si el jugador fue encontrado
            if (sendingUsername == null)
            {
                Console.WriteLine("No se pudo encontrar al jugador que envía el mensaje.");
                return;
            }

            // Enviar el mensaje a todos los jugadores en el mismo lobby
            List<LobbyPlayer> playersInLobby = lobbies[lobbyCode];
            foreach (var player in playersInLobby)
            {
                try
                {
                    // Enviar el mensaje a cada jugador en el lobby
                    player.CallbackChannel.ReceiveMessage(sendingUsername, mensaje);
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine($"Error al enviar mensaje a {player.Username}: {ex.Message}");
                }
            }
        }




    }
}
