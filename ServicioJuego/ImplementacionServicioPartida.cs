using AccesoDatos.Modelo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.ServiceModel;
//using static ServicioJuego.JugadorPartida;

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
        private static int jugadoresConCartaRevelada = 0;
        private static int jugadoresGuardaronCarta = 0;
        private static int jugadoresNoGuardaronCarta = 0;
        private Carta cartaParteTrasera = new Carta("ParteTraseraCarta", 55, 9);


        private void InicializarMazo(string idPartida)
        {
            // Lista temporal para crear todas las cartas como instancias separadas
            var cartasTemporales = new List<Carta>();

            // Contador para los IDs de las cartas
            int idCounter = 1;

            // Agregar cartas al mazo asignando IDs únicos
            cartasTemporales.AddRange(Enumerable.Repeat(0, 3)
                                                 .Select(_ => new Carta("Carta1", idCounter++, 1)));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 5)
                                                 .Select(_ => new Carta("Carta2", idCounter++, 2)));
            cartasTemporales.AddRange(Enumerable.Repeat(0, 7)
                                                 .Select(_ => new Carta("Carta3", idCounter++, 3)));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 9)
                                                 .Select(_ => new Carta("Carta4", idCounter++, 4)));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 11)
                                                 .Select(_ => new Carta("Carta5", idCounter++, 5)));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 13)
                                                 .Select(_ => new Carta("Carta6", idCounter++, 6)));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 3)
                                                 .Select(_ => new Carta("Carta7", idCounter++, 7)));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 2)
                                                 .Select(_ => new Carta("Carta8", idCounter++, 8)));
            
            cartasTemporales.Add(cartaParteTrasera);

            // Asignar el mazo completo
            CartasEnMazo = cartasTemporales;
        }

        /*private void InicializarMazo(string idPartida)
        {
            // Lista temporal para crear todas las cartas como instancias separadas
            var cartasTemporales = new List<Carta>();

            // Contador para los IDs de las cartas
            int idCounter = 1;

            // Agregar cartas al mazo asignando IDs únicos
            cartasTemporales.AddRange(Enumerable.Repeat(0, 3)
                                                 .Select(_ => new Carta("Carta1", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta1.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 3)
                                                 .Select(_ => new Carta("Carta2", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta2.png")));
            cartasTemporales.AddRange(Enumerable.Repeat(0, 3)
                                                 .Select(_ => new Carta("Carta3", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta3.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 3)
                                                 .Select(_ => new Carta("Carta4", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta4.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 3)
                                                 .Select(_ => new Carta("Carta5", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta5.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 1)
                                                 .Select(_ => new Carta("Carta6", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta6.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 1)
                                                 .Select(_ => new Carta("Carta7", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta7.png")));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 1)
                                                 .Select(_ => new Carta("Carta8", idCounter++, "/Recursos/ElementosPartida/ImagenesPartida/Cartas/Carta8.png")));

            cartasTemporales.Add(cartaParteTrasera);

            // Asignar el mazo completo
            CartasEnMazo = cartasTemporales;
        }*/

        private void BarajarMazo(string idPartida)
        {
            Random random = new Random();
            var cartasSinParteTrasera = CartasEnMazo.Take(CartasEnMazo.Count - 1).ToList();
            var cartaParteTrasera = CartasEnMazo.Last();

            for (int i = cartasSinParteTrasera.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = cartasSinParteTrasera[i];
                cartasSinParteTrasera[i] = cartasSinParteTrasera[j];
                cartasSinParteTrasera[j] = temp;
            }

            CartasEnMazo = cartasSinParteTrasera;
            CartasEnMazo.Add(cartaParteTrasera); // Mantener la carta trasera al final
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
        /*public void RegistrarJugador(string nombreUsuario)
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
        }*/

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
                Console.WriteLine($"El jugador {nombreUsuario} ya está registrado. Actualizando el callback.");
                _callbacks[nombreUsuario] = callback; // Actualiza el callback si ya existe
            }
        }
        /*public void CrearPartida(List<JugadorPartida> jugadores, string idPartida)
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
        }*/

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

            if (!partidas.ContainsKey(idPartida))
            {
                partidas.Add(idPartida, partida);
                Console.WriteLine("Partida creada correctamente.");
            }
            else
            {
                Console.WriteLine("La partida con este ID ya existe.");
                return;
            }

            foreach (var jugador in jugadores)
            {
                if (_callbacks.TryGetValue(jugador.NombreUsuario, out var callback))
                {
                    jugador.CallbackChannel = callback;
                    Console.WriteLine($"Notificando al jugador {jugador.NombreUsuario} sobre la creación de la partida.");
                    NotificarPartidaCreada(jugador, idPartida);
                }
                else
                {
                    Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un canal de callback registrado.");

                    if (jugador.EsInvitado)
                    {
                        Console.WriteLine($"El jugador {jugador.NombreUsuario} es invitado.");
                        string nombre = jugador.NombreUsuario;
                        Console.WriteLine($"Registrando al jugador invitado {nombre}.");
                        RegistrarJugadorInvitado(new JugadorPartida { NombreUsuario = nombre });
                    }
                    else
                    {
                        Console.WriteLine($"El jugador {jugador.NombreUsuario} no es invitado, pero no tiene callback.");
                    }
                }
            }

            Console.WriteLine("Proceso de creación de la partida finalizado.");
        }

        public void RegistrarJugadorInvitado(JugadorPartida invitado)
        {
            Console.WriteLine("esto es lo que recibe el registrar jugador como invitado" + invitado.NombreUsuario);
            try
            {
                var callback = OperationContext.Current.GetCallbackChannel<IServicioPartidaCallback>();
                if (callback != null)
                {
                    _callbacks[invitado.NombreUsuario] = callback;
                    Console.WriteLine($"Callback registrado para el invitado: {invitado.NombreUsuario}");
                }
                else
                {
                    Console.WriteLine($"Error: No se pudo obtener el canal de callback para el invitado {invitado.NombreUsuario}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar el jugador invitado: {ex.Message}");
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
                var jugadores = partida.Jugadores;
                var jugadorTurnoActual = jugadores[partida.TurnoActual];

                foreach(var jugador in jugadores)
                {
                    if (jugador.CallbackChannel != null)
                        jugador.CallbackChannel.NotificarTurnoIniciado(jugadorTurnoActual.NombreUsuario);
                    else
                        Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                }
            }
            else
            {
                Console.WriteLine("Partida no encontrada.");
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
                CartasEnMano[jugador.NombreUsuario].Remove(carta);

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

        public void RobarCartaAJugador(string nombreDefensor, string idPartida, bool cartaDuplicacionActiva)
        {
            var partida = partidas[idPartida];
            var jugadores = partida.Jugadores;
            var jugadorObjetivoRobo = jugadores.FirstOrDefault(c => c.NombreUsuario == nombreDefensor);
            var jugadorAtacante = jugadores[partida.TurnoActual]; // Quien inició el ataque originalmente.
            int cartasARobar = 1;
            if (cartaDuplicacionActiva)
            {
                cartasARobar = 2;
            }

            if (jugadorObjetivoRobo != null)
            {
                var cartasBloqueoRobo = CartasEnMano[nombreDefensor].Where(c => c.Tipo == "Carta7" || c.Tipo == "Carta8").ToList();

                if (cartasBloqueoRobo.Count == 0)
                {
                    for (int i = 0; i < cartasARobar; i++)
                    {
                        RobarCarta(partida.IdPartida, nombreDefensor);
                    }
                }
                else
                {
                    partida.RoboEnProgreso = new RoboContexto
                    {
                        Atacante = jugadorAtacante,
                        Defensor = jugadorObjetivoRobo
                    };

                    foreach (var jugador in jugadores)
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            jugador.CallbackChannel.NotificarIntentoRoboCarta(nombreDefensor);
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


        public void RobarCarta(string idPartida, string nombreDefensor)
        {
            var partida = partidas[idPartida];
            if (partida.RoboEnProgreso != null)
            {
                var atacante = partida.RoboEnProgreso.Atacante;
                var defensor = partida.RoboEnProgreso.Defensor;

                if (CartasEnMano[defensor.NombreUsuario].Count > 0)
                {
                    var cartaRobada = CartasEnMano[defensor.NombreUsuario][new Random().Next(CartasEnMano[defensor.NombreUsuario].Count)];
                    CartasEnMano[defensor.NombreUsuario].Remove(cartaRobada);
                    CartasEnMano[atacante.NombreUsuario].Add(cartaRobada);

                    foreach (var jugador in partida.Jugadores)
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            jugador.CallbackChannel.NotificarCartaRobada(cartaRobada, defensor.NombreUsuario, atacante.NombreUsuario);
                        }
                    }
                }

                // Finaliza el contexto del robo.
                partida.RoboEnProgreso = null;
            }
            else
            {
                var jugadorObjetivoRobo = partida.Jugadores.FirstOrDefault(j => j.NombreUsuario == nombreDefensor);
                var jugadorTurnoActual = partida.Jugadores[partida.TurnoActual];
                var cartaRobada = CartasEnMano[jugadorObjetivoRobo.NombreUsuario][new Random().Next(CartasEnMano[jugadorObjetivoRobo.NombreUsuario].Count)];
                CartasEnMano[jugadorObjetivoRobo.NombreUsuario].Remove(cartaRobada);
                CartasEnMano[jugadorTurnoActual.NombreUsuario].Add(cartaRobada);

                foreach (var jugador in partida.Jugadores)
                {
                    if (jugador.CallbackChannel != null)
                    {
                        jugador.CallbackChannel.NotificarCartaRobada(cartaRobada, jugadorObjetivoRobo.NombreUsuario, jugadorTurnoActual.NombreUsuario);
                    }
                }

            }
        }

        public void UtilizarCartaDefensiva(string idPartida, string nombreDefensor)
        {
            var partida = partidas[idPartida];
            if (partida.RoboEnProgreso != null)
            {
                var atacanteAnterior = partida.RoboEnProgreso.Atacante;
                partida.RoboEnProgreso.Atacante = partida.RoboEnProgreso.Defensor;
                partida.RoboEnProgreso.Defensor = atacanteAnterior;

                foreach (var jugador in partida.Jugadores)
                {
                    if (jugador.CallbackChannel != null)
                    {
                        jugador.CallbackChannel.NotificarIntentoRoboCarta(partida.RoboEnProgreso.Defensor.NombreUsuario);
                    }
                }
            }
        }

        public void RobarCartaEsconditeAJugador(string nombreDefensor, string idPartida, bool cartaDuplicacionActiva)
        {
            var partida = partidas[idPartida];
            var jugadores = partida.Jugadores;
            var jugadorObjetivoRobo = jugadores.FirstOrDefault(c => c.NombreUsuario == nombreDefensor);
            var jugadorAtacante = jugadores[partida.TurnoActual]; // Quien inició el ataque originalmente.
            int cartasARobar = 1;
            if (cartaDuplicacionActiva)
            {
                cartasARobar = 2;
            }

            if (jugadorObjetivoRobo != null)
            {
                var cartasBloqueoRobo = CartasEnMano[nombreDefensor].Where(c => c.Tipo == "Carta7" || c.Tipo == "Carta8").ToList();

                if (cartasBloqueoRobo.Count == 0)
                {
                    for(int i = 0; i < cartasARobar; i ++)
                    {
                        RobarCartaEscondite(partida.IdPartida, nombreDefensor);
                    }
                    
                }
                else
                {
                    partida.RoboEnProgreso = new RoboContexto
                    {
                        Atacante = jugadorAtacante,
                        Defensor = jugadorObjetivoRobo
                    };

                    foreach (var jugador in jugadores)
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            jugador.CallbackChannel.NotificarIntentoRoboCartaEscondite(nombreDefensor);
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


        public void RobarCartaEscondite(string idPartida, string nombreDefensor)
        {
            var partida = partidas[idPartida];
            if (partida.RoboEnProgreso != null)
            {
                var atacante = partida.RoboEnProgreso.Atacante;
                var defensor = partida.RoboEnProgreso.Defensor;

                if (CartasEnEscondite[defensor.NombreUsuario].Count > 0)
                {
                    var cartaRobada = CartasEnEscondite[defensor.NombreUsuario][new Random().Next(CartasEnEscondite[defensor.NombreUsuario].Count)];
                    CartasEnEscondite[defensor.NombreUsuario].Remove(cartaRobada);
                    CartasEnMano[atacante.NombreUsuario].Add(cartaRobada);

                    foreach (var jugador in partida.Jugadores)
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            jugador.CallbackChannel.NotificarCartaEsconditeRobada(cartaRobada, defensor.NombreUsuario, atacante.NombreUsuario);
                        }
                    }
                }

                // Finaliza el contexto del robo.
                partida.RoboEnProgreso = null;
            }
            else
            {
                var jugadorObjetivoRobo = partida.Jugadores.FirstOrDefault(j => j.NombreUsuario == nombreDefensor);
                var jugadorTurnoActual = partida.Jugadores[partida.TurnoActual];
                var cartaRobada = CartasEnEscondite[jugadorObjetivoRobo.NombreUsuario][new Random().Next(CartasEnEscondite[jugadorObjetivoRobo.NombreUsuario].Count)];
                CartasEnEscondite[jugadorObjetivoRobo.NombreUsuario].Remove(cartaRobada);
                CartasEnMano[jugadorTurnoActual.NombreUsuario].Add(cartaRobada);

                foreach (var jugador in partida.Jugadores)
                {
                    if (jugador.CallbackChannel != null)
                    {
                        jugador.CallbackChannel.NotificarCartaEsconditeRobada(cartaRobada, jugadorObjetivoRobo.NombreUsuario, jugadorTurnoActual.NombreUsuario);
                    }
                }

            }
        }

        public void TomarCartaDeDescarte(string idPartida, string nombreUsuario, int idCarta)
        {
            if (partidas.ContainsKey(idPartida))
            {
                var cartaTomadaDescarte = CartasDescarte.FirstOrDefault(c => c.IdCarta == idCarta);
                if (cartaTomadaDescarte != null)
                {
                    CartasDescarte.Remove(cartaTomadaDescarte);
                    AgregarCartaACartasEnMano(nombreUsuario, cartaTomadaDescarte, idPartida);
                    var jugadores = partidas[idPartida].Jugadores;

                    foreach (var jugador in jugadores)
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            try
                            {
                                jugador.CallbackChannel.NotificarCartaTomadaDescarte(cartaTomadaDescarte.IdCarta);
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

        public void ObligarATirarDado(string idPartida)
        {
            if (partidas.ContainsKey(idPartida))
            {
                var partida = partidas[idPartida];
                var jugadores = partida.Jugadores;
                var jugadorEnTurno = jugadores[partida.TurnoActual];
                // Notificar al jugador que debe tirar el dado
                foreach( var jugador in jugadores)
                {
                    if (jugador.CallbackChannel != null)
                    {
                        jugador.CallbackChannel.NotificarTiroDadoForzado(jugadorEnTurno.NombreUsuario);
                    }
                    else
                    {
                        Console.WriteLine($"El jugador {jugadorEnTurno.NombreUsuario} no tiene un canal de callback válido.");
                    }
                }
                
            }
            else
            {
                Console.WriteLine("Partida no encontrada.");
            }
        }

        public void PreguntarGuardarCartaEnEscondite(string idPartida)
        {
            if (partidas.ContainsKey(idPartida))
            {
                var jugadores = partidas[idPartida].Jugadores;
                var jugadorTurnoActual = jugadores[partidas[idPartida].TurnoActual];
                var cartaRevelada = CartasEnMazo.Last();
                
                foreach( var jugador in jugadores)
                {
                    if(jugador.NombreUsuario != jugadorTurnoActual.NombreUsuario)
                    {
                        Console.WriteLine($"La carta que se revelo fue de tipo: {cartaRevelada.Tipo}");
                        if (CartasEnMano[jugador.NombreUsuario].Any(c => c.Tipo == cartaRevelada.Tipo))
                        {
                            jugadoresConCartaRevelada++;
                            if(jugador.CallbackChannel != null)
                            {
                                jugador.CallbackChannel.NotificarPreguntaJugadores(jugadorTurnoActual.NombreUsuario);
                            }
                            else
                            {
                                Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un canal de callback válido.");
                            }
                        }
                        else
                        {
                            if(jugadorTurnoActual.CallbackChannel != null)
                            {
                                jugadorTurnoActual.CallbackChannel.NotificarNumeroJugadoresGuardaronCarta(0);
                            }
                            else
                            {
                                Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un canal de callback válido.");
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Partida no encontrada");
            }
        }

        public void EnviarDecision(string idPartida, bool decision)
        {
            Console.WriteLine($"Jugadores con carta revelada: {jugadoresConCartaRevelada}");
            if (partidas.ContainsKey(idPartida))
            {
                var partida = partidas[idPartida];
                var jugadores = partida.Jugadores;
                var jugadorTurnoActual = jugadores[partida.TurnoActual];
                if (decision)
                {
                    jugadoresGuardaronCarta++;
                }
                else
                {
                    jugadoresNoGuardaronCarta++;
                }
                Console.WriteLine($"Jugadores que guardaron carta: {jugadoresGuardaronCarta}");
                Console.WriteLine($"Jugadores que no guardaron carta: {jugadoresNoGuardaronCarta}");
                if (jugadoresGuardaronCarta + jugadoresNoGuardaronCarta == jugadoresConCartaRevelada)
                {
                    if(jugadorTurnoActual.CallbackChannel != null)
                    {
                        jugadorTurnoActual.CallbackChannel.NotificarNumeroJugadoresGuardaronCarta(jugadoresGuardaronCarta);
                    }
                    else
                    {
                        Console.WriteLine($"El jugador {jugadorTurnoActual.NombreUsuario} no tiene un canal de callback válido.");
                    }
                    jugadoresConCartaRevelada = 0;
                    jugadoresGuardaronCarta = 0;
                    jugadoresNoGuardaronCarta = 0;
                }
            }
            else
            {
                Console.WriteLine("Partida no encontrada");
            }
        }

        public void RevelarCartaMazo(string idPartida)
        {
            if (partidas.ContainsKey(idPartida))
            {
                CartasEnMazo.Remove(CartasEnMazo.Last());
                var partida = partidas[idPartida];
                var jugadores = partida.Jugadores;
                foreach(var jugador in jugadores)
                {
                    if(jugador.CallbackChannel != null)
                    {
                        jugador.CallbackChannel.NotificarMazoRevelado();
                    }
                    else
                    {
                        Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un canal de callback válido.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Partida no encontrada");
            }
        }

        public void OcultarCartaMazo(string idPartida)
        {
            if (partidas.ContainsKey(idPartida))
            {
                CartasEnMazo.Add(cartaParteTrasera);
                var partida = partidas[idPartida];
                var jugadores = partida.Jugadores;
                foreach (var jugador in jugadores)
                {
                    if (jugador.CallbackChannel != null)
                    {
                        jugador.CallbackChannel.NotificarMazoOculto(cartaParteTrasera);
                    }
                    else
                    {
                        Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un canal de callback válido.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Partida no encontrada");
            }
        }

        /*public void FinalizarJuego(string idPartida)
        {
            if (!partidas.ContainsKey(idPartida))
                return;

            var partida = partidas[idPartida];

            foreach (var jugador in partida.Jugadores)
            {
                if (CartasEnMano.ContainsKey(jugador.NombreUsuario))
                {
                    CartasDescarte.AddRange(CartasEnMano[jugador.NombreUsuario]);
                    CartasEnMano[jugador.NombreUsuario].Clear();
                }
            }

            var puntajes = CalcularPuntaje(partida.Jugadores);

            var ganador = puntajes.OrderByDescending(p => p.Value).FirstOrDefault();

            foreach (var jugador in partida.Jugadores)
            {
                if (jugador.CallbackChannel != null)
                {
                    jugador.CallbackChannel.NotificarResultadosJuego(puntajes, ganador.Key, ganador.Value);
                }
                else
                {
                    Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un canal de callback válido.");
                }
            }
            partidas.Remove(idPartida);
            CartasEnMano.Clear();
            CartasEnEscondite.Clear();
            CartasDescarte.Clear();
        }

        private Dictionary<string, int> CalcularPuntaje(List<JugadorPartida> jugadores)
        {
            Dictionary<string, int> puntajes = jugadores.ToDictionary(j => j.NombreUsuario, j => 0);

            var tablaPuntos = new Dictionary<string, int[]>

            {
                { "Carta1", new[] { 3, 0, 0 } },
                { "Carta2", new[] { 4, 2, 0 } },
                { "Carta3", new[] { 5, 3, 1 } },
                { "Carta4", new[] { 6, 2, 1 } },
                { "Carta5", new[] { 7, 0, 0 } }
            };

            foreach (var tipoCarta in tablaPuntos.Keys)
            {
                var conteoPorJugador = jugadores
                    .Select(j => new { JugadorPartida = j, Cantidad = CartasEnEscondite[j.NombreUsuario].Count(c => c.Tipo == tipoCarta) })
                    .OrderByDescending(x => x.Cantidad)
                    .ThenBy(x => x.JugadorPartida.NombreUsuario)
                    .ToList();

                for (int i = 0; i < conteoPorJugador.Count && i < 3; i++)
                {
                    var jugador = conteoPorJugador[i].JugadorPartida;
                    int cantidad = conteoPorJugador[i].Cantidad;

                    if (cantidad > 0)
                    {
                        puntajes[jugador.NombreUsuario] += tablaPuntos[tipoCarta][i];
                    }
                }
            }

            return puntajes;
        }*/

        public void FinalizarJuego(string idPartida)
        {
            if (!partidas.ContainsKey(idPartida))
                return;
            var partida = partidas[idPartida];
            foreach (var jugador in partida.Jugadores)
            {
                if (CartasEnMano.ContainsKey(jugador.NombreUsuario))
                {
                    CartasDescarte.AddRange(CartasEnMano[jugador.NombreUsuario]);
                    CartasEnMano[jugador.NombreUsuario].Clear();
                }
            }
            var puntajes = CalcularPuntaje(partida.Jugadores);
            var ganador = puntajes.OrderByDescending(p => p.Value).FirstOrDefault();
            foreach (var jugador in partida.Jugadores)
            {
                if (jugador.CallbackChannel != null)
                {
                    jugador.CallbackChannel.NotificarResultadosJuego(puntajes, ganador.Key, ganador.Value);
                }
                else
                {
                    Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un canal de callback válido.");
                }
            }
            partidas.Remove(idPartida);
            CartasEnMano.Clear();
            CartasEnEscondite.Clear();
            CartasDescarte.Clear();
        }
        private Dictionary<string, int> CalcularPuntaje(List<JugadorPartida> jugadores)
        {
            Dictionary<string, int> puntajes = jugadores.ToDictionary(j => j.NombreUsuario, j => 0);
            var tablaPuntos = new Dictionary<string, int[]>
            {
                { "Carta1", new[] { 3, 0, 0 } },
                { "Carta2", new[] { 4, 2, 0 } },
                { "Carta3", new[] { 5, 3, 1 } },
                { "Carta4", new[] { 6, 2, 1 } },
                { "Carta5", new[] { 7, 0, 0 } }
            };
            foreach (var tipoCarta in tablaPuntos.Keys)
            {
                var conteoPorJugador = jugadores
                    .Select(j => new { JugadorPartida = j, Cantidad = CartasEnEscondite[j.NombreUsuario].Count(c => c.Tipo == tipoCarta) })
                    .OrderByDescending(x => x.Cantidad)
                    .ThenBy(x => x.JugadorPartida.NombreUsuario)
                    .ToList();
                for (int i = 0; i < conteoPorJugador.Count && i < 3; i++)
                {
                    var jugador = conteoPorJugador[i].JugadorPartida;
                    int cantidad = conteoPorJugador[i].Cantidad;
                    if (cantidad > 0)
                    {
                        puntajes[jugador.NombreUsuario] += tablaPuntos[tipoCarta][i];
                    }
                }
            }
            return puntajes;
        }

        public void DejarTirarDado(string idPartida)
        {
            if (partidas.ContainsKey(idPartida))
            {
                var partida = partidas[idPartida];
                var jugadores = partida.Jugadores;

                foreach(var jugador in jugadores)
                {
                    jugador.CallbackChannel.NotificarPararTirarDado();
                }
            }
            else
            {
                Console.Write("Partida no encontrada");
            }
        }

        public void EstablecerModoSeleccionCartaJugadorEnTurno(string idPartida, int idModoSeleccion)
        {
            var partida = partidas[idPartida];
            var jugadores = partida.Jugadores;
            var jugadorTurnoActual = jugadores[partida.TurnoActual];

            foreach(var jugador in jugadores)
            {
                jugadorTurnoActual.CallbackChannel.NotificarModoSeleccionCarta(idModoSeleccion);
            }
        }

        public void EstablecerModoSeleccionCartaJugador(string idPartida, int idModoSeleccion)
        {
            var partida = partidas[idPartida];
            var jugadores = partida.Jugadores;
            var jugadorTurnoActual = jugadores[partida.TurnoActual];
            foreach (var jugador in jugadores)
            {
                if(jugador.NombreUsuario != jugadorTurnoActual.NombreUsuario)
                {
                    jugador.CallbackChannel.NotificarModoSeleccionCarta(idModoSeleccion);
                }
            }
        }

        public void EstablecerModoSeleccionarCartaJugadores(string idPartida, int idModoSeleccion, List<string> nombresJugadores)
        {
            var partida = partidas[idPartida];
            var jugadores = partida.Jugadores;
            var jugadorTurnoActual = jugadores[partida.TurnoActual];
            foreach (var jugador in jugadores)
            {
                foreach(var nombreJugador in nombresJugadores)
                {
                    if(jugador.NombreUsuario == nombreJugador)
                    {
                        if (jugador.NombreUsuario != jugadorTurnoActual.NombreUsuario)
                        {
                            jugador.CallbackChannel.NotificarModoSeleccionCarta(idModoSeleccion);
                        }
                    }
                }
                
            }
        }
    }
}


