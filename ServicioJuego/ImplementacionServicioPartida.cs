using System;
using System.Collections.Generic;
using System.ServiceModel;
using static ServicioJuego.JugadorPartida;

namespace ServicioJuego
{
    public partial class ImplementacionServicio : IServicioPartida
    {
        private static readonly Dictionary<string, Partida> partidas = new Dictionary<string, Partida>();
        private static readonly Dictionary<string, IServicioPartidaCallback> _callbacks = new Dictionary<string, IServicioPartidaCallback>();

        public void RegistrarJugador(string nombreUsuario)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IServicioPartidaCallback>();

            if (!_callbacks.ContainsKey(nombreUsuario))
            {
                _callbacks[nombreUsuario] = callback;
                Console.WriteLine($"Callback registrado de: {nombreUsuario}");
            }
            else
            {
                Console.WriteLine($"El jugador {nombreUsuario} ya está registrado.");
            }
        }

        public void CrearPartida(List<JugadorPartida> jugadores, string idPartida)
        {
            Console.WriteLine("Jugadores recibidos en el servidor: ");
            foreach (var jugador in jugadores)
            {
                Console.WriteLine($"Jugador: {jugador.NombreUsuario}");
            }

            // Crear la instancia de la partida
            var partida = new Partida
            {
                IdPartida = idPartida,
                Jugadores = jugadores,
                TurnoActual = 0
            };

            partidas.Add(idPartida, partida);
            Console.WriteLine("Partida creada correctamente.");

            foreach (var jugador in jugadores)
            {
                if (_callbacks.TryGetValue(jugador.NombreUsuario, out var callback))
                {
                    jugador.CallbackChannel = callback;

                    NotificarPartidaCreada(jugador, idPartida);
                }
                else
                {
                    Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un canal de callback registrado.");
                }
            }
        }

        private void NotificarPartidaCreada(JugadorPartida jugador, string idPartida)
        {
            if (jugador.CallbackChannel != null)
            {
                try
                {
                    jugador.CallbackChannel.NotificarPartidaCreada(idPartida);
                    Console.WriteLine($"Notificación de partida creada enviada a {jugador.NombreUsuario}");
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine($"Error al notificar la creación de partida al jugador {jugador.NombreUsuario}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
            }
        }

        public void EmpezarTurno(string idPartida)
        {
            if (partidas.ContainsKey(idPartida))
            {
                var partida = partidas[idPartida];
                var jugadorActual = partida.Jugadores[partida.TurnoActual];

                NotificarTurnoIniciado(jugadorActual);
            }
            else
            {
                Console.WriteLine("Partida no encontrada.");
            }
        }

        private void NotificarTurnoIniciado(JugadorPartida jugador)
        {
            if (jugador.CallbackChannel != null)
            {
                try
                {
                    jugador.CallbackChannel.NotificarTurnoIniciado(jugador.NombreUsuario);
                    Console.WriteLine($"Es el turno de {jugador.NombreUsuario}.");
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine($"Error al iniciar el turno de {jugador.NombreUsuario}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
            }
        }

        public void FinalizarTurno(string idPartida, string nombreUsuario)
        {
            Console.WriteLine(idPartida);
            if (partidas.ContainsKey(idPartida))
            {
                var partida = partidas[idPartida];
                var jugadorActual = partida.Jugadores[partida.TurnoActual];

                if (jugadorActual.NombreUsuario == nombreUsuario)
                {
                    NotificarTurnoTerminado(jugadorActual);

                    partida.TurnoActual = (partida.TurnoActual + 1) % partida.Jugadores.Count;
                    EmpezarTurno(idPartida);
                }
                else
                {
                    Console.WriteLine($"No es el turno de {nombreUsuario}. Actualmente le toca a {jugadorActual.NombreUsuario}.");
                }
            }
            else
            {
                Console.WriteLine("Partida no encontrada.");
            }
        }

        private void NotificarTurnoTerminado(JugadorPartida jugador)
        {
            if (jugador.CallbackChannel != null)
            {
                try
                {
                    jugador.CallbackChannel.NotificarTurnoTerminado(jugador.NombreUsuario);
                    Console.WriteLine($"El turno de {jugador.NombreUsuario} ha terminado.");
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine($"Error al finalizar el turno de {jugador.NombreUsuario}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
            }
        }
    }
}

