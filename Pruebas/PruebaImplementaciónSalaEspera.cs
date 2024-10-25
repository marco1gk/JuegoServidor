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
    public class PruebaImplementacionServicio
    {
        private readonly ImplementacionServicio servicio;
        private readonly Mock<ILobbyManagerCallback> callbackMock;

        public PruebaImplementacionServicio()
        {
            servicio = new ImplementacionServicio();
            callbackMock = new Mock<ILobbyManagerCallback>();
         //   OperationContext.Current = new OperationContext(new Mock<IContextChannel>().Object);
        }

        [Fact]
        public void CrearLobby_ConJugador_DevuelveCodigoLobby()
        {
            // Arrange
            var jugador = new LobbyPlayer
            {
                Username = "Jugador1",
                CallbackChannel = callbackMock.Object
            };

            // Act
            servicio.CreateLobby(jugador);

            // Assert
            var lobbyCode = servicio.BuscarLobbyDisponible();
            Assert.NotNull(lobbyCode);
        }

        [Fact]
        public void UnirseALobby_Existente_DevuelveNotificacion()
        {
            // Arrange
            var jugador1 = new LobbyPlayer
            {
                Username = "Jugador1",
                CallbackChannel = callbackMock.Object
            };

            var jugador2 = new LobbyPlayer
            {
                Username = "Jugador2",
                CallbackChannel = callbackMock.Object
            };

            servicio.CreateLobby(jugador1);
            string lobbyCode = servicio.BuscarLobbyDisponible();

            // Act
            servicio.JoinLobby(lobbyCode, jugador2);

            // Assert
            callbackMock.Verify(c => c.NotifyPlayersInLobby(lobbyCode, It.IsAny<List<LobbyPlayer>>()), Times.Once);
        }

        [Fact]
        public void SalirDelLobby_EliminarJugador_DevuelveNotificacion()
        {
            // Arrange
            var jugador1 = new LobbyPlayer
            {
                Username = "Jugador1",
                CallbackChannel = callbackMock.Object
            };

            var jugador2 = new LobbyPlayer
            {
                Username = "Jugador2",
                CallbackChannel = callbackMock.Object
            };

            servicio.CreateLobby(jugador1);
            string lobbyCode = servicio.BuscarLobbyDisponible();
            servicio.JoinLobby(lobbyCode, jugador2);

            // Act
            servicio.ExitLobby(lobbyCode, "Jugador1");

            // Assert
            callbackMock.Verify(c => c.NotifyPlayerLeftLobby("Jugador1"), Times.Once);
        }

        [Fact]
        public void UnirseALobby_LobbyLlena_DevuelveNotificacionDeError()
        {
            // Arrange
            var jugador1 = new LobbyPlayer
            {
                Username = "Jugador1",
                CallbackChannel = callbackMock.Object
            };

            var jugador2 = new LobbyPlayer
            {
                Username = "Jugador2",
                CallbackChannel = callbackMock.Object
            };

            var jugador3 = new LobbyPlayer
            {
                Username = "Jugador3",
                CallbackChannel = callbackMock.Object
            };

            var jugador4 = new LobbyPlayer
            {
                Username = "Jugador4",
                CallbackChannel = callbackMock.Object
            };

            servicio.CreateLobby(jugador1);
            string lobbyCode = servicio.BuscarLobbyDisponible();
            servicio.JoinLobby(lobbyCode, jugador2);
            servicio.JoinLobby(lobbyCode, jugador3);
            servicio.JoinLobby(lobbyCode, jugador4);

            // Act
            var jugador5 = new LobbyPlayer
            {
                Username = "Jugador5",
                CallbackChannel = callbackMock.Object
            };
            servicio.JoinLobby(lobbyCode, jugador5);

            // Assert
            callbackMock.Verify(c => c.NotifyLobbyIsFull(), Times.Once);
        }
    }
}
