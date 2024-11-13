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
            var mockCallback = new Mock<IGestorSalasEsperasCallBack>();
            var jugador = new JugadorSalaEspera { NombreUsuario = "Jugador1", CallbackChannel = mockCallback.Object };
            var servicio = new ImplementacionServicio();

            // Acción
            servicio.CrearSalaEspera(jugador);

            // Verificación
            Assert.NotNull(servicio.BuscarSalaEsperaDisponible());
            mockCallback.Verify(callback => callback.NotificarSalaEsperaCreada(It.IsAny<string>()), Times.Once);
        }

        // Prueba para el método JoinLobbyAsHost
        [Fact]
        public void UnirseLobbyComoAnfitrion_DeberiaUnirJugadorAlLobby()
        {
            // Configuración
            var mockCallback = new Mock<IGestorSalasEsperasCallBack>();
            var jugador = new JugadorSalaEspera { NombreUsuario = "Jugador1", CallbackChannel = mockCallback.Object };
            var servicio = new ImplementacionServicio();
            servicio.CrearSalaEspera(jugador);

            var codigoLobby = servicio.BuscarSalaEsperaDisponible();

            // Acción
            servicio.UnirSalaEsperaComoAnfitrion(codigoLobby);

            // Verificación
            mockCallback.Verify(callback => callback.NotificarSalaEsperaCreada(codigoLobby), Times.Once);
        }


        // Prueba para el método JoinLobby
        [Fact]
        public void UnirseLobby_DeberiaAgregarJugadorAlLobbyExistente()
        {
            // Configuración
            var mockCallback = new Mock<IGestorSalasEsperasCallBack>();
            var jugador1 = new JugadorSalaEspera { NombreUsuario = "Jugador1", CallbackChannel = mockCallback.Object };
            var servicio = new ImplementacionServicio();
            servicio.CrearSalaEspera(jugador1);

            var nuevoJugador = new JugadorSalaEspera { NombreUsuario = "Jugador2", CallbackChannel = mockCallback.Object };
            var codigoLobby = servicio.BuscarSalaEsperaDisponible();

            // Acción
            servicio.UnirseSalaEspera(codigoLobby, nuevoJugador);

            // Verificación
            mockCallback.Verify(callback => callback.NotificarJugadoresEnSalaEspera(codigoLobby, It.IsAny<List<JugadorSalaEspera>>()), Times.Once);
            mockCallback.Verify(callback => callback.NotificarJugadorSeUnioSalaEspera(It.IsAny<JugadorSalaEspera>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void UnirseLobby_DeberiaManejarLobbyLleno()
        {
            // Configuración
            var mockCallback = new Mock<IGestorSalasEsperasCallBack>();
            var servicio = new ImplementacionServicio();
            var codigoLobby = servicio.BuscarSalaEsperaDisponible();

            // Crear lobby lleno
            for (int i = 1; i <= 4; i++)
            {
                var jugador = new JugadorSalaEspera { NombreUsuario = $"Jugador{i}", CallbackChannel = mockCallback.Object };
                servicio.UnirseSalaEspera(codigoLobby, jugador);
            }

            var nuevoJugador = new JugadorSalaEspera { NombreUsuario = "Jugador5", CallbackChannel = mockCallback.Object };

            // Acción
            servicio.UnirseSalaEspera(codigoLobby, nuevoJugador);

            // Verificación
            mockCallback.Verify(callback => callback.NotificarSalaEsperaLlena(), Times.Once);
        }

        [Fact]
        public void UnirseLobby_DeberiaManejarLobbyNoExistente()
        {
            // Configuración
            var mockCallback = new Mock<IGestorSalasEsperasCallBack>();
            var nuevoJugador = new JugadorSalaEspera { NombreUsuario = "Jugador2", CallbackChannel = mockCallback.Object };
            var servicio = new ImplementacionServicio();

            // Acción
            servicio.UnirseSalaEspera("CodigoInvalido", nuevoJugador);

            // Verificación
            mockCallback.Verify(callback => callback.NotificarSalaEsperaNoExiste(), Times.Once);
        }

        // Prueba para el método ExitLobby
        [Fact]
        public void SalirLobby_DeberiaEliminarJugadorDelLobby()
        {
            // Configuración
            var mockCallback = new Mock<IGestorSalasEsperasCallBack>();
            var jugador = new JugadorSalaEspera { NombreUsuario = "Jugador1", CallbackChannel = mockCallback.Object };
            var servicio = new ImplementacionServicio();
            servicio.CrearSalaEspera(jugador);
            var codigoLobby = servicio.BuscarSalaEsperaDisponible();

            // Acción
            servicio.SalirSalaEspera(codigoLobby, "Jugador1");

            // Verificación
            Assert.Null(servicio.BuscarSalaEsperaDisponible());
            mockCallback.Verify(callback => callback.NotificarJugadorSalioSalaEspera("Jugador1"), Times.Once);
        }


        // Prueba para el método SendMessage
        [Fact]
        public void EnviarMensaje_DeberiaEnviarMensajeAJugadoresEnLobby()
        {
            // Configuración
            var mockCallback = new Mock<IGestorSalasEsperasCallBack>();
            var jugador1 = new JugadorSalaEspera { NombreUsuario = "Jugador1", CallbackChannel = mockCallback.Object };
            var servicio = new ImplementacionServicio();
            servicio.CrearSalaEspera(jugador1);
            var codigoLobby = servicio.BuscarSalaEsperaDisponible();

            var jugador2 = new JugadorSalaEspera { NombreUsuario = "Jugador2", CallbackChannel = mockCallback.Object };
            servicio.UnirseSalaEspera(codigoLobby, jugador2);

            // Acción
            servicio.MandarMensaje("Hola a todos");

            // Verificación
            mockCallback.Verify(callback => callback.RecibirMensaje("Jugador1", "Hola a todos"), Times.AtLeastOnce);
        }

        [Fact]
        public void EnviarMensaje_DeberiaManejarJugadorSinLobby()
        {
            // Configuración
            var mockCallback = new Mock<IGestorSalasEsperasCallBack>();
            var servicio = new ImplementacionServicio();

            // Acción
            servicio.MandarMensaje("Mensaje sin lobby");

            // Verificación
            // (Aquí podrías verificar que no se llamó a ReceiveMessage, pero en este caso, el mensaje solo se muestra en la consola)
        }
    }
}
