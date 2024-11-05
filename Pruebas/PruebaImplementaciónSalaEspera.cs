using System;
using System.Collections.Generic;
using System.ServiceModel;
using Moq;
using Xunit;
using AccesoDatos;
using AccesoDatos.DAO;
using AccesoDatos.Modelo;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ServicioJuego;


namespace Pruebas
{
    public class PruebaImplementacionServicioSalaEspera
    {
        // Prueba para el método CreateLobby
        [Fact]
        public void CrearLobby_DeberiaCrearNuevoLobby()
        {
            // Configuración
            var mockCallback = new Mock<ILobbyManagerCallback>();
            var jugador = new LobbyPlayer { Username = "Jugador1", CallbackChannel = mockCallback.Object };
            var servicio = new ImplementacionServicio();

            // Acción
            servicio.CreateLobby(jugador);

            // Verificación
            Assert.NotNull(servicio.BuscarLobbyDisponible());
            mockCallback.Verify(callback => callback.NotifyLobbyCreated(It.IsAny<string>()), Times.Once);
        }

        // Prueba para el método JoinLobbyAsHost
        [Fact]
        public void UnirseLobbyComoAnfitrion_DeberiaUnirJugadorAlLobby()
        {
            // Configuración
            var mockCallback = new Mock<ILobbyManagerCallback>();
            var jugador = new LobbyPlayer { Username = "Jugador1", CallbackChannel = mockCallback.Object };
            var servicio = new ImplementacionServicio();
            servicio.CreateLobby(jugador);

            var codigoLobby = servicio.BuscarLobbyDisponible();

            // Acción
            servicio.JoinLobbyAsHost(codigoLobby);

            // Verificación
            mockCallback.Verify(callback => callback.NotifyLobbyCreated(codigoLobby), Times.Once);
        }


        // Prueba para el método JoinLobby
        [Fact]
        public void UnirseLobby_DeberiaAgregarJugadorAlLobbyExistente()
        {
            // Configuración
            var mockCallback = new Mock<ILobbyManagerCallback>();
            var jugador1 = new LobbyPlayer { Username = "Jugador1", CallbackChannel = mockCallback.Object };
            var servicio = new ImplementacionServicio();
            servicio.CreateLobby(jugador1);

            var nuevoJugador = new LobbyPlayer { Username = "Jugador2", CallbackChannel = mockCallback.Object };
            var codigoLobby = servicio.BuscarLobbyDisponible();

            // Acción
            servicio.JoinLobby(codigoLobby, nuevoJugador);

            // Verificación
            mockCallback.Verify(callback => callback.NotifyPlayersInLobby(codigoLobby, It.IsAny<List<LobbyPlayer>>()), Times.Once);
            mockCallback.Verify(callback => callback.NotifyPlayerJoinToLobby(It.IsAny<LobbyPlayer>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void UnirseLobby_DeberiaManejarLobbyLleno()
        {
            // Configuración
            var mockCallback = new Mock<ILobbyManagerCallback>();
            var servicio = new ImplementacionServicio();
            var codigoLobby = servicio.BuscarLobbyDisponible();

            // Crear lobby lleno
            for (int i = 1; i <= 4; i++)
            {
                var jugador = new LobbyPlayer { Username = $"Jugador{i}", CallbackChannel = mockCallback.Object };
                servicio.JoinLobby(codigoLobby, jugador);
            }

            var nuevoJugador = new LobbyPlayer { Username = "Jugador5", CallbackChannel = mockCallback.Object };

            // Acción
            servicio.JoinLobby(codigoLobby, nuevoJugador);

            // Verificación
            mockCallback.Verify(callback => callback.NotifyLobbyIsFull(), Times.Once);
        }

        [Fact]
        public void UnirseLobby_DeberiaManejarLobbyNoExistente()
        {
            // Configuración
            var mockCallback = new Mock<ILobbyManagerCallback>();
            var nuevoJugador = new LobbyPlayer { Username = "Jugador2", CallbackChannel = mockCallback.Object };
            var servicio = new ImplementacionServicio();

            // Acción
            servicio.JoinLobby("CodigoInvalido", nuevoJugador);

            // Verificación
            mockCallback.Verify(callback => callback.NotifyLobbyDoesNotExist(), Times.Once);
        }

        // Prueba para el método ExitLobby
        [Fact]
        public void SalirLobby_DeberiaEliminarJugadorDelLobby()
        {
            // Configuración
            var mockCallback = new Mock<ILobbyManagerCallback>();
            var jugador = new LobbyPlayer { Username = "Jugador1", CallbackChannel = mockCallback.Object };
            var servicio = new ImplementacionServicio();
            servicio.CreateLobby(jugador);
            var codigoLobby = servicio.BuscarLobbyDisponible();

            // Acción
            servicio.ExitLobby(codigoLobby, "Jugador1");

            // Verificación
            Assert.Null(servicio.BuscarLobbyDisponible());
            mockCallback.Verify(callback => callback.NotifyPlayerLeftLobby("Jugador1"), Times.Once);
        }


        // Prueba para el método SendMessage
        [Fact]
        public void EnviarMensaje_DeberiaEnviarMensajeAJugadoresEnLobby()
        {
            // Configuración
            var mockCallback = new Mock<ILobbyManagerCallback>();
            var jugador1 = new LobbyPlayer { Username = "Jugador1", CallbackChannel = mockCallback.Object };
            var servicio = new ImplementacionServicio();
            servicio.CreateLobby(jugador1);
            var codigoLobby = servicio.BuscarLobbyDisponible();

            var jugador2 = new LobbyPlayer { Username = "Jugador2", CallbackChannel = mockCallback.Object };
            servicio.JoinLobby(codigoLobby, jugador2);

            // Acción
            servicio.SendMessage("Hola a todos");

            // Verificación
            mockCallback.Verify(callback => callback.ReceiveMessage("Jugador1", "Hola a todos"), Times.AtLeastOnce);
        }

        [Fact]
        public void EnviarMensaje_DeberiaManejarJugadorSinLobby()
        {
            // Configuración
            var mockCallback = new Mock<ILobbyManagerCallback>();
            var servicio = new ImplementacionServicio();

            // Acción
            servicio.SendMessage("Mensaje sin lobby");

            // Verificación
            // (Aquí podrías verificar que no se llamó a ReceiveMessage, pero en este caso, el mensaje solo se muestra en la consola)
        }
    }
}
