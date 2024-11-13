using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego
{
    public partial class ImplementacionServicio : IJuegoService
    {
        private readonly Dictionary<string, List<MatchPlayer>> games = new Dictionary<string, List<MatchPlayer>>();
        private readonly Dictionary<string, int> currentTurnIndex = new Dictionary<string, int>();

        /*public void StartGame(List<MatchPlayer> players, string gameId)
        {
            if (!games.ContainsKey(gameId))
            {
                games[gameId] = players;
                currentTurnIndex[gameId] = 0;
                StartTurn(gameId); // Iniciar el turno para el primer jugador
            }
        }*/

        public void StartMatch(List<MatchPlayer> players, string gameId)
        {
            if (!games.ContainsKey(gameId))
            {
                games[gameId] = players;
                currentTurnIndex[gameId] = 0;
                StartTurn(gameId); // Iniciar el turno para el primer jugador
            }
        }

        public void StartTurn(string gameId)
        {
            if (games.ContainsKey(gameId))
            {
                List<MatchPlayer> players = games[gameId];
                int turnIndex = currentTurnIndex[gameId];
                MatchPlayer currentPlayer = players[turnIndex];
                IClienteJuegoCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IClienteJuegoCallback>();
                currentPlayer.CallbackChannel = currentUserCallbackChannel;

                try
                {
                    currentPlayer.CallbackChannel.NotifyTurnStarted(currentPlayer.Username);
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine($"Error al notificar el inicio del turno: {ex.Message}");
                    // Manejo de errores adicional si es necesario
                }
            }
        }
        public void EndTurn(string gameId, string playerId)
        {
            if (games.ContainsKey(gameId))
            {
                List<MatchPlayer> players = games[gameId];
                int turnIndex = currentTurnIndex[gameId];
                IClienteJuegoCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IClienteJuegoCallback>();
                players[turnIndex].CallbackChannel = currentUserCallbackChannel;

                if (players[turnIndex].Username == playerId)
                {
                    try
                    {
                        players[turnIndex].CallbackChannel.NotifyTurnEnded(playerId);
                    }
                    catch (CommunicationException ex)
                    {
                        Console.WriteLine($"Error al notificar el inicio del turno: {ex.Message}");
                        // Manejo de errores adicional si es necesario
                    }
                    players[turnIndex].CallbackChannel.NotifyTurnEnded(playerId);
                    currentTurnIndex[gameId] = (turnIndex + 1) % players.Count;
                    StartTurn(gameId);
                }
            }
        }
    }
}
