using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego
{
    [ServiceContract(CallbackContract = typeof(IServicioPartidaCallback))]
    public interface IServicioPartida
    {
        [OperationContract(IsOneWay = true)]
        void EmpezarTurno(string idPartida);

        [OperationContract(IsOneWay = true)]
        void FinalizarTurno(string idPartida, string nombreUsuario);

        [OperationContract(IsOneWay = true)]
        void CrearPartida(List<JugadorPartida> jugadores, string idPartida);

        [OperationContract(IsOneWay = true)]
        void RegistrarJugador(string nombreUsuario);
    }

    [ServiceContract]
    public interface IServicioPartidaCallback
    {
        [OperationContract]
        void NotificarTurnoIniciado(string nombreUsuario);

        [OperationContract]
        void NotificarTurnoTerminado(string nombreUsuario);

        [OperationContract]
        void NotificarResultadoAccion(string accion, bool sucedio);

        [OperationContract]
        void NotificarPartidaCreada(string idPartida);
    }

    [DataContract]
    public class JugadorPartida
    {
        [DataMember]
        public string NombreUsuario { get; set; }

        [DataMember]
        public int NumeroFotoPerfil { get; set; }
        public IServicioPartidaCallback CallbackChannel { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            else
            {
                JugadorPartida otherPlayer = (JugadorPartida)obj;
                return NombreUsuario == otherPlayer.NombreUsuario;
            }
        }

        public override int GetHashCode()
        {
            return NombreUsuario.GetHashCode();
        }

        [DataContract]
        public class Partida
        {
            [DataMember]
            public string IdPartida { get; set; }
            [DataMember]
            public List<JugadorPartida> Jugadores { get; set; }
            [DataMember]
            public int TurnoActual { get; set; }
        }

    }

}
