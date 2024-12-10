using AccesoDatos.Modelo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.ServiceModel;
using static ServicioJuego.JugadorPartida;

namespace ServicioJuego
{
    public partial class ImplementacionServicio : IServicioPartida
    {
        private static readonly Dictionary<string, Partida> partidas = new Dictionary<string, Partida>();
        private static readonly Dictionary<string, IServicioPartidaCallback> _callbacks = new Dictionary<string, IServicioPartidaCallback>();
        private static List<Carta> CartasEnMazo = new List<Carta>();
        private static List<Carta> CartasDescarte = new List<Carta>();
        private static readonly Dictionary<string, List<Carta>> CartasEnEscondite = new Dictionary<string, List<Carta>>();
        private static readonly Dictionary<string, List<Carta>> CartasEnMano = new Dictionary<string, List<Carta>>();

        private void InicializarMazo(string idPartida)
        {
            var cartasTemporales = new List<Carta>();

            int idCounter = 1;

            cartasTemporales.AddRange(Enumerable.Repeat(0, 3)
                                                 .Select(_ => new Carta("Carta1", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta1.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 5)
                                                 .Select(_ => new Carta("Carta2", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta2.png")));
            cartasTemporales.AddRange(Enumerable.Repeat(0, 7)
                                                 .Select(_ => new Carta("Carta3", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta3.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 9)
                                                 .Select(_ => new Carta("Carta4", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta4.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 11)
                                                 .Select(_ => new Carta("Carta5", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta5.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 13)
                                                 .Select(_ => new Carta("Carta6", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta6.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 3)
                                                 .Select(_ => new Carta("Carta7", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta7.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 2)
                                                 .Select(_ => new Carta("Carta8", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta8.png")));

    
            CartasEnMazo = cartasTemporales;
        }

        private void BarajarMazo(string idPartida)
        {
            Random random = new Random();
            for (int i = CartasEnMazo.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1); 
                var temp = CartasEnMazo[i];
                CartasEnMazo[i] = CartasEnMazo[j];
                CartasEnMazo[j] = temp;
            }
        }

        private List<Carta> RepartirCartas(int cantidad)
        {
            var cartasRepartidas = CartasEnMazo.Take(cantidad).ToList();
            CartasEnMazo = CartasEnMazo.Skip(cantidad).ToList();
            return cartasRepartidas;
        }

        private void AsignarCartasAJugadores(string idPartida)
        {
            if (partidas.ContainsKey(idPartida))
            {
                var partida = partidas[idPartida];
                int[] cartasPorJugador = { 3, 4, 5, 6 };

                for (int i = 0; i < partida.Jugadores.Count; i++)
                {
                    var jugador = partida.Jugadores[i];
                    int cantidadCartas = cartasPorJugador[i];

                    CartasEnMano[jugador.NombreUsuario] = new List<Carta>();

                    var cartasRepartidas = RepartirCartas(cantidadCartas);
                    foreach (var carta in cartasRepartidas)
                    {
                        CartasEnMano[jugador.NombreUsuario].Add(carta);
                        carta.Asignada = true;
                    }
                }
            }
            else
            {
                Console.WriteLine("La partida no se encontro");
            }
        }

        public void RepartirCartas(string idPartida)
        {
            InicializarMazo(idPartida);
            BarajarMazo(idPartida);
            AsignarCartasAJugadores(idPartida);
            if (partidas.ContainsKey(idPartida))
            {
                foreach(var jugador in partidas[idPartida].Jugadores)
                {
                    if (jugador.CallbackChannel != null)
                    {
                        try
                        {
                            jugador.CallbackChannel.NotificarCartasEnMano(CartasEnMano[jugador.NombreUsuario]);
                            jugador.CallbackChannel.NotificarCartasEnMazo(CartasEnMazo);
                        }
                        catch (CommunicationException ex)
                        {
                            Console.WriteLine($"Error al notificar al jugador {jugador.NombreUsuario}: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                    }
                }
            }
            else
            {
                Console.WriteLine("La partida no se encontro");
            }
        }
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

        public void LanzarDado(string idPartida, string nombreUsuario)
        {
            if (partidas.ContainsKey(idPartida))
            {
                var partida = partidas[idPartida];
                var jugadorActual = partida.Jugadores[partida.TurnoActual];

                if (jugadorActual.NombreUsuario == nombreUsuario)
                {
                    Random random = new Random();
                    int resultadoDado = random.Next(1, 7);
                    
                    foreach (var jugador in partida.Jugadores)
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            try
                            {
                                jugador.CallbackChannel.NotificarResultadoDado(nombreUsuario, resultadoDado);
                                Console.WriteLine($"Resultado del dado ({resultadoDado}) notificado a {jugador.NombreUsuario}.");
                            }
                            catch (CommunicationException ex)
                            {
                                Console.WriteLine($"Error al notificar resultado del dado a {jugador.NombreUsuario}: {ex.Message}");
                            }
                        }
                    }
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

        public void TomarCartaDeMazo(string idPartida, string nombreUsuario, int idCarta)
        {
            if (partidas.ContainsKey(idPartida))
            {
                var cartaTomadaMazo = CartasEnMazo.FirstOrDefault(c => c.IdCarta == idCarta);
                if(cartaTomadaMazo != null)
                {
                    CartasEnMazo.Remove(cartaTomadaMazo);
                    AgregarCartaACartasEnMano(nombreUsuario, cartaTomadaMazo, idPartida);
                    var jugadores = partidas[idPartida].Jugadores;

                    foreach(var jugador in jugadores)
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            try
                            {
                                jugador.CallbackChannel.NotificarCartaTomadaMazo(cartaTomadaMazo.IdCarta);
                            }
                            catch (CommunicationException ex)
                            {
                                Console.WriteLine($"Error al notificar resultado del dado a {jugador.NombreUsuario}: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("La carta no se encontro");
                }
            }
            else
            {
                Console.WriteLine("Partida no encontrada");
            }
        }

        public void AgregarCartaACartasEnMano(string nombreUsuario, Carta carta, string idPartida)
        {
            if (CartasEnMano.ContainsKey(nombreUsuario))
            {
                CartasEnMano[nombreUsuario].Add(carta);
                var jugador = partidas[idPartida].Jugadores.FirstOrDefault(j => j.NombreUsuario == nombreUsuario);
                if(jugador.CallbackChannel != null)
                {
                    jugador.CallbackChannel.NotificarCartaAgregadaAMano(carta);
                }
                else
                {
                    Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                }
            }
            else
            {
                Console.WriteLine("El jugador no esta en la partida.");
            }
        }

        public void TomarFichaMesa(string idPartida, int idFicha)
        {
            if (partidas.ContainsKey(idPartida))
            {
                var jugadores = partidas[idPartida].Jugadores;
                var jugadorTurnoActual = jugadores[partidas[idPartida].TurnoActual];
                
                foreach (var jugador in jugadores)
                {
                    if(jugador.CallbackChannel != null)
                    {
                        jugador.CallbackChannel.NotificarFichaTomadaMesa(jugadorTurnoActual.NombreUsuario, idFicha);
                    }
                    else
                    {
                        Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Partida no encontrada");
            }
        }

        public void UtilizarCarta(string idPartida, int idCarta, string nombreJugador)
        {
            if (partidas.ContainsKey(idPartida))
            {
                var cartaUtilizada = CartasEnMano[nombreJugador].FirstOrDefault(c => c.IdCarta == idCarta);
                var jugadores = partidas[idPartida].Jugadores;
                var jugadorActual = jugadores.FirstOrDefault(j => j.NombreUsuario == nombreJugador);
                if (cartaUtilizada != null)
                {
                    CartasEnMano[nombreJugador].Remove(cartaUtilizada);
                    AgregarCartaADescarte(cartaUtilizada, jugadores);

                    if(jugadorActual.CallbackChannel != null)
                    {
                        jugadorActual.CallbackChannel.NotificarCartaUtilizada(cartaUtilizada.IdCarta);
                    }
                    else
                    {
                        Console.WriteLine($"El jugador {jugadorActual.NombreUsuario} no tiene un callback válido.");
                    }
                }
                else
                {
                    Console.WriteLine("No tienes esta carta");
                }
            }
            else
            {
                Console.WriteLine("Partida no encontrada");
            }
        }

        public void AgregarCartaADescarte(Carta cartaUtilizada, List<JugadorPartida> jugadores)
        {
            CartasDescarte.Add(cartaUtilizada);
            
            foreach(var jugador in jugadores)
            {
                if(jugador.CallbackChannel != null)
                {
                    jugador.CallbackChannel.NotificarCartaAgregadaADescarte(cartaUtilizada);
                }
                else
                {
                    Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                }
            }
        }

        public void DevolverFichaAMesa(int idFicha, string idPartida)
        {
            if (partidas.ContainsKey(idPartida))
            {
                var jugadores = partidas[idPartida].Jugadores;
                var jugadorTurnoActual = jugadores[partidas[idPartida].TurnoActual];

                foreach (var jugador in jugadores)
                {
                    if(jugador.CallbackChannel != null)
                    {
                        jugador.CallbackChannel.NotificarFichaDevuelta(idFicha, jugadorTurnoActual.NombreUsuario);
                    }
                    else
                    {
                        Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Partida no encontrada");
            }
        }

        public void AgregarCartaAEscondite(string nombreUsuario, int idCarta, string idPartida)
        {
            var jugador = partidas[idPartida].Jugadores.FirstOrDefault(j => j.NombreUsuario == nombreUsuario);

            if(jugador != null)
            {
                var carta = CartasEnMano[jugador.NombreUsuario].FirstOrDefault(c => c.IdCarta == idCarta);
                if (!CartasEnEscondite.ContainsKey(nombreUsuario))
                {
                    CartasEnEscondite[jugador.NombreUsuario] = new List<Carta>();
                }
                CartasEnEscondite[jugador.NombreUsuario].Add(carta);

                if(jugador.CallbackChannel != null)
                {
                    jugador.CallbackChannel.NotificarCartaAgregadaAEscondite(carta.IdCarta);
                }
                else
                {
                    Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                }
            }
            else
            {
                Console.WriteLine("Jugador no encontrado.");
            }
        }

        public int NumeroCartasEnMano(string nombreUsuario, string idPartida)
        {
            var jugadorActual = partidas[idPartida].Jugadores.FirstOrDefault(j => j.NombreUsuario == nombreUsuario);
            int numeroCartasEnMano = 0;

            if( jugadorActual != null)
            {
                numeroCartasEnMano = CartasEnMano[nombreUsuario].Count;
            }
            return numeroCartasEnMano;
        }

        public void RobarCartaAJugador(string nombreUsuario, string idPartida)
        {
            var partida = partidas[idPartida];
            var jugadores = partida.Jugadores;
            var jugadorObjetivoRobo = jugadores.FirstOrDefault(c => c.NombreUsuario == nombreUsuario);

            if(jugadorObjetivoRobo != null)
            {
                var cartasBloquoRobo = CartasEnMano[nombreUsuario].Where(c => c.Tipo == "Carta7" || c.Tipo == "Carta8").ToList();

                if(cartasBloquoRobo.Count == 0)
                {
                    RobarCarta(jugadorObjetivoRobo.NombreUsuario, partida.IdPartida);
                }
                else
                {
                    foreach (var jugador in jugadores)
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            jugador.CallbackChannel.NotificarIntentoRoboCarta(nombreUsuario);
                        }
                        else
                        {
                            Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                        }
                    }
                }
                
            }
            else
            {
                Console.WriteLine("Jugador de objetivo de robo no encontrado");
            }
        }

        public void RobarCarta(string nombreJugadorObjetivoRobo, string idPartida)
        {
            var partida = partidas[idPartida];
            var jugadores = partida.Jugadores;
            var jugadorObjetivoRobo = jugadores.FirstOrDefault(j => j.NombreUsuario == nombreJugadorObjetivoRobo);
            var jugadorTurnoActual = jugadores[partida.TurnoActual];
            List<Carta> cartasJugadorObjetivoRobo;
            cartasJugadorObjetivoRobo = CartasEnMano[jugadorObjetivoRobo.NombreUsuario];
            var cartaRobada = cartasJugadorObjetivoRobo[new Random().Next(cartasJugadorObjetivoRobo.Count)];
            CartasEnMano[jugadorObjetivoRobo.NombreUsuario].Remove(cartaRobada);
            CartasEnMano[jugadorTurnoActual.NombreUsuario].Add(cartaRobada);

            foreach(var jugador in jugadores)
            {
                if(jugador.CallbackChannel != null)
                {
                    jugador.CallbackChannel.NotificarCartaRobada(cartaRobada, jugadorObjetivoRobo.NombreUsuario, jugadorTurnoActual.NombreUsuario);
                }
                else
                {
                    Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                }
            }
        }

    }
}


