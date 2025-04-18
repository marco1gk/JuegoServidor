﻿using AccesoDatos.Modelo;
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
        void RobarCartaAJugador(string nombreDefensor, string idPartida, bool cartaDuplicacionActiva);

        [OperationContract(IsOneWay = true)]
        void RobarCarta(string idPartida, string nombreDefensor);

        [OperationContract(IsOneWay = true)]
        void UtilizarCartaDefensiva(string idPartida, string nombreDefensor);

        [OperationContract(IsOneWay = true)]
        void RobarCartaEsconditeAJugador(string nombreDefensor, string idPartida, bool cartaDuplicacionActiva);

        [OperationContract(IsOneWay = true)]
        void RobarCartaEscondite(string idPartida, string nombreDefensor);

        [OperationContract(IsOneWay = true)]
        void TomarCartaDeDescarte(string idPartida, string nombreJugador, int idCarta);

        [OperationContract(IsOneWay = true)]
        void ObligarATirarDado(string idPartida);

        [OperationContract(IsOneWay = true)]
        void PreguntarGuardarCartaEnEscondite(string idPartida);

        [OperationContract(IsOneWay = true)]
        void EnviarDecision(string idPartida, bool decision);

        [OperationContract(IsOneWay = true)]
        void RevelarCartaMazo(string idPartida);

        [OperationContract(IsOneWay = true)]
        void OcultarCartaMazo(string idPartida);

        [OperationContract(IsOneWay = true)]
        void FinalizarJuego(string idPartida);

        [OperationContract(IsOneWay = true)]
        void DejarTirarDado(string idPartida);

        [OperationContract(IsOneWay = true)]
        void EstablecerModoSeleccionCarta(string idPartida, int idModoSeleccion, string nombreJugador);

        [OperationContract]
        void RegistrarJugadorInvitado(JugadorPartida invitado);

        [OperationContract(IsOneWay = true)]
        void ActualizarDecisionTurno(string idPartida);

    }

    [ServiceContract]
    public interface IServicioPartidaCallback
    {
        [OperationContract]
        void NotificarTurnoIniciado(string jugadorTurnoActual);

        [OperationContract]
        void NotificarJugadorDesconectado(string nombreUsuario);

        [OperationContract]
        void NotificarTurnoTerminado(string nombreUsuario);


        [OperationContract]
        void NotificarPartidaCreada(string idPartida);

        [OperationContract]
        void NotificarResultadoDado(string nombreJugador, int resultadoDado);

        [OperationContract]
        void NotificarCartasEnMano(List<Carta> cartasRepartidas);

        [OperationContract]
        void NotificarCartasEnMazo(List<Carta> cartasEnMazo);

        [OperationContract]
        void NotificarCartaTomadaMazo(int idCarta);

        [OperationContract]
        void NotificarCartaAgregadaAMano(Carta carta);

        [OperationContract]
        void NotificarFichaTomadaMesa(string jugadorTurnoActual, int idFicha);

        [OperationContract]
        void NotificarCartaUtilizada(int idCartaUtilizada);

        [OperationContract]
        void NotificarCartaAgregadaADescarte(Carta cartaUtilizada);

        [OperationContract]
        void NotificarFichaDevuelta(int idFicha, string nombreJugadorTurnoActual);

        [OperationContract]
        void NotificarCartaAgregadaAEscondite(int idCarta);

        [OperationContract]
        void NotificarCartaRobada(Carta carta, string jugadorObjetivoRobo, string jugadorTurnoActual);

        [OperationContract]
        void NotificarIntentoRoboCartaEscondite(string nombreJugadorAtacante);

        [OperationContract]
        void NotificarCartaEsconditeRobada(Carta carta, string jugadorObjetivoRobo, string jugadorTurnoActual);

        [OperationContract]
        void NotificarCartaTomadaDescarte(int idCarta);

        [OperationContract]
        void NotificarTiroDadoForzado(string jugadorTurnoActual);

        [OperationContract]
        void NotificarPreguntaJugadores(string jugadorTurnoActual, string tipoCartaRevelada);

        [OperationContract]
        void NotificarNumeroJugadoresGuardaronCarta(int numeroJugadoresGuardaronCarta);

        [OperationContract]
        void NotificarMazoRevelado();

        [OperationContract]
        void NotificarMazoOculto(Carta cartaParteTrasera);

        [OperationContract]
        void NotificarResultadosJuego(Dictionary<string, int> puntajes, string ganador, int puntajeGanador);

        [OperationContract]
        void NotificarPararTirarDado();

        [OperationContract]
        void NotificarModoSeleccionCarta(int idModoSeleccionCarta);

        [OperationContract]
        void NotificarActualizacionDecisionTurno();

        [OperationContract]
        void NotificarIntentoRobo(string nombreJugadorDefensor);

    }

    [DataContract]
    public class JugadorPartida
    {
        [DataMember]
        public string NombreUsuario { get; set; }

        [DataMember]
        public int NumeroFotoPerfil { get; set; }
        public IServicioPartidaCallback CallbackChannel { get; set; }

        [DataMember]
        public bool EsInvitado { get; set; }

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

        [DataMember]
        public RoboContexto RoboEnProgreso { get; set; }
    }

    [DataContract]
    public class Carta
    {
        [DataMember]
        public int IdCarta { get; set; }

        [DataMember]
        public string Tipo { get; set; }

        [DataMember]
        public int IdRutaImagen { get; set; }

        [DataMember]
        public double PosicionX { get; set; }

        [DataMember]
        public double PosicionY { get; set; }


        public Carta(string tipo, int idCarta, int idRutaImagen)
        {
            IdCarta = idCarta;
            Tipo = tipo;
            IdRutaImagen = idRutaImagen;
            PosicionX = 0;
            PosicionY = 0;
        }

        public Carta() { }

    }

    public class RoboContexto
    {
        public JugadorPartida Atacante { get; set; }
        public JugadorPartida Defensor { get; set; }
    }

}

