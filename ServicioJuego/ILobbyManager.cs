using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego
{
    [ServiceContract(CallbackContract = typeof(IGestorSalasEsperasCallBack))]
    public interface ILobbyManager
    {
        [OperationContract(IsOneWay = true)]
        void CrearSalaEspera(JugadorSalaEspera jugador);

        [OperationContract(IsOneWay = true)]
        void UnirseSalaEspera(string codigoSalaEspera, JugadorSalaEspera jugador);

        [OperationContract(IsOneWay = true)]
        void UnirSalaEsperaComoAnfitrion(string codigoSalaEspera);

        [OperationContract]
        void SalirSalaEspera(string codigoSalaEspera, string nombreUsuario);

        [OperationContract]
        void MandarMensaje(string mensaje);

        [OperationContract]
        string BuscarSalaEsperaDisponible();

        [OperationContract(IsOneWay = true)]
        void IniciarPartida(string codigoSalaEspera);

    }

    [ServiceContract]
    public interface IGestorSalasEsperasCallBack
    {
        [OperationContract]
        void NotificarSalaEsperaCreada(string codigoSalaEspera);

        [OperationContract]
        void NotificarJugadoresEnSalaEspera(string codigoSalaEspera, List<JugadorSalaEspera> jugador);

        [OperationContract]
        void NotificarJugadorSeUnioSalaEspera(JugadorSalaEspera jugador, int numeroJugadoresSalaEspera);

        [OperationContract]
        void NotificarJugadorSalioSalaEspera(string nombreUsuario);

        [OperationContract]
        void NotificarAnfritionJugadorSalioSalaEspera();

        [OperationContract]
        void NotificarIniciarPartida(JugadorSalaEspera[] jugadores);

        [OperationContract]
        void NotificarSalaEsperaLlena();

        [OperationContract]
        void NotificarSalaEsperaNoExiste();

        [OperationContract]
        void NotificarExpulsadoSalaEspera();
        [OperationContract]
        void RecibirMensaje(string nombreUsuario, string mensaje);
        [OperationContract]
        void NotificarPuedeIniciarPartida(bool puedeIniciar);
    }

    [DataContract]
    public class JugadorSalaEspera
    {
        [DataMember]
        public string NombreUsuario { get; set; }

        [DataMember]
        public int NumeroFotoPerfil { get; set; } 

        public IGestorSalasEsperasCallBack CallbackChannel { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                JugadorSalaEspera otherPlayer = (JugadorSalaEspera)obj;
                return NombreUsuario == otherPlayer.NombreUsuario;
            }
        }

        public override int GetHashCode()
        {
            return NombreUsuario.GetHashCode();
        }
    }
}
