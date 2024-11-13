using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego
{
    [ServiceContract(CallbackContract = typeof(IClienteJuegoCallback))]
    public interface IJuegoService
    {
        [OperationContract]
        void StartTurn(string gameId);

        [OperationContract]
        void EndTurn(string gameId, string playerId);

        [OperationContract]
        void StartMatch(List<MatchPlayer> players, string gameId);
        
    }

    [ServiceContract]
    public interface IClienteJuegoCallback
    {
        [OperationContract]
        void NotifyTurnStarted(string playerId);

        [OperationContract]
        void NotifyTurnEnded(string playerId);

        [OperationContract]
        void NotifyActionResult(string action, bool success);
    }

    [DataContract]
    public class MatchPlayer
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public int NumeroFotoPerfil { get; set; } // Nueva propiedad para la foto de perfil

        public IClienteJuegoCallback CallbackChannel { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                MatchPlayer otherPlayer = (MatchPlayer)obj;
                return Username == otherPlayer.Username;
            }
        }

        public override int GetHashCode()
        {
            return Username.GetHashCode();
        }
    }

}
