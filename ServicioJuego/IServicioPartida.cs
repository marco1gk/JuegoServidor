using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        [OperationContract(IsOneWay = true)]
        void LanzarDado(string idPartida, string nombreUsuario);

        [OperationContract(IsOneWay = true)]
        void RepartirCartas(string idPartida);

        [OperationContract(IsOneWay = true)]
        void TomarCartaDeMazo(string idPartida, string nombreUsuario, int idCarta);

        [OperationContract(IsOneWay = true)]
        void AgregarCartaACartasEnMano(string nombreUsuario, Carta carta, string idPartida);

        [OperationContract(IsOneWay = true)]
        void TomarFichaMesa(string idPartida, int idFicha);

        [OperationContract(IsOneWay = true)]
        void UtilizarCarta(string idPartida, int idCarta, string nombreJugador);

        [OperationContract(IsOneWay = true)]
        void AgregarCartaADescarte(Carta carta, List<JugadorPartida> jugadores);

        [OperationContract(IsOneWay = true)]
        void DevolverFichaAMesa(int idFicha, string idPartida);

        [OperationContract(IsOneWay = true)]
        void AgregarCartaAEscondite(string nombreUsuario, int idCarta, string idPartida);

        [OperationContract]
        int NumeroCartasEnMano(string nombreUsuario, string idPartida);

        [OperationContract(IsOneWay = true)]
        void RobarCartaAJugador(string nombreUsuario, string idPartida);

        [OperationContract(IsOneWay = true)]
        void RobarCarta(string nombreJugadorObjetivoRobo, string idPartida);
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

        [OperationContract]
        void NotificarResultadoDado(string nombreUsuario, int resultado);

        [OperationContract]
        void NotificarCartasEnMano(List<Carta> cartasRepartidas);

        [OperationContract]
        void NotificarCartasEnMazo(List<Carta> cartasEnMazo);

        [OperationContract]
        void NotificarCartaTomadaMazo(int idCarta);

        [OperationContract]
        void NotificarCartaAgregadaAMano(Carta carta);

        [OperationContract]
        void NotificarFichaTomadaMesa(string nombreUsuario, int idFicha);

        [OperationContract]
        void NotificarCartaUtilizada(int idCarta);

        [OperationContract]
        void NotificarCartaAgregadaADescarte(Carta carta);

        [OperationContract]
        void NotificarFichaDevuelta(int idFicha, string nombreJugadorTurnoActual);

        [OperationContract]
        void NotificarCartaAgregadaAEscondite(int idCarta);

        [OperationContract]
        void NotificarIntentoRoboCarta(string nombreUsuario);

        [OperationContract]
        void NotificarCartaRobada(Carta cartaRobada, string nombreJugadorObjetivoRobo, string nombreJugadorTurnoActual);

        
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

    [DataContract]
    public class Carta
    {
        [DataMember]
        public int IdCarta { get; set; }

        [DataMember]
        public string Tipo { get; set; }

        [DataMember]
        public string RutaImagen { get; set; }

        [DataMember]
        public double PosicionX { get; set; }

        [DataMember]
        public double PosicionY { get; set; }

        [DataMember]
        public bool Asignada { get; set; }

        public Carta(string tipo, int idCarta, string rutaImagen)
        {
            IdCarta = idCarta;
            Tipo = tipo;
            RutaImagen = rutaImagen;
            PosicionX = 0;
            PosicionY = 0;
            Asignada = false;
        }

        // Asegúrate de incluir también un constructor sin parámetros para la serialización:
        public Carta() { }

    }

}
