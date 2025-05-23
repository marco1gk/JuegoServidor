﻿using AccesoDatos.Modelo;
using AccesoDatos.Utilidades;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.ServiceModel;

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
        public void RegistrarJugador(string nombreUsuario)
        {
            var callback = OperationContext.Current.GetCallbackChannel<IServicioPartidaCallback>();

            if (!_callbacks.ContainsKey(nombreUsuario))
            {
                _callbacks[nombreUsuario] = callback;
                Console.WriteLine($"Callback registrado de: {nombreUsuario}");

                var channel = OperationContext.Current.Channel;
                channel.Closed += (sender, args) => DesconectarJugador(nombreUsuario);
                channel.Faulted += (sender, args) => DesconectarJugador(nombreUsuario);
            }
            else
            {
                Console.WriteLine($"El jugador {nombreUsuario} ya está registrado. Actualizando el callback.");
                _callbacks[nombreUsuario] = callback;
            }
        }


        private void DesconectarJugador(string nombreUsuario)
        {
            if (_callbacks.Remove(nombreUsuario))
            {
                Console.WriteLine($"Jugador {nombreUsuario} desconectado y eliminado de callbacks.");

                var partida = partidas.Values.FirstOrDefault(p => p.Jugadores.Any(j => j.NombreUsuario == nombreUsuario));
                if (partida != null)
                {
                    var jugador = partida.Jugadores.FirstOrDefault(j => j.NombreUsuario == nombreUsuario);
                    if (jugador != null)
                    {
                        partida.Jugadores.Remove(jugador);

                        foreach (var j in partida.Jugadores)
                        {
                            j.CallbackChannel?.NotificarJugadorDesconectado(nombreUsuario);
                        }

                        Console.WriteLine($"Jugador {nombreUsuario} eliminado de la partida {partida.IdPartida}.");

                        if (partida.TurnoActual >= partida.Jugadores.Count)
                        {
                            partida.TurnoActual = 0;
                        }
                        else if (partida.Jugadores.Count > 0)
                        {
                            partida.TurnoActual = (partida.TurnoActual + 1) % partida.Jugadores.Count;
                        }

                        EmpezarTurno(partida.IdPartida);
                    }
                }
            }
            else
            {
                Console.WriteLine($"No se encontró el callback para el jugador {nombreUsuario}.");
            }
        }


        public void EmpezarTurno(string idPartida)
        {
            if (partidas.ContainsKey(idPartida))
            {
                var partida = partidas[idPartida];
                var jugadores = partida.Jugadores;
                JugadorPartida jugadorTurnoActual = null;

                try
                {
                    jugadorTurnoActual = jugadores[partida.TurnoActual];
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    return; 
                }

                foreach (var jugador in jugadores.ToList())
                {
                    if (jugador.CallbackChannel != null)
                    {
                        try
                        {
                            jugador.CallbackChannel.NotificarTurnoIniciado(jugadorTurnoActual.NombreUsuario);
                        }
                        catch (EndpointNotFoundException ex)
                        {
                            ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        }
                        catch (CommunicationException ex)
                        {
                            ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        }
                        catch(TimeoutException ex)
                        {
                            ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
                catch (EndpointNotFoundException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (CommunicationException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (TimeoutException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
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

                    var jugadores = partida.Jugadores.ToList();

                    foreach (var jugador in jugadores)
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            try
                            {
                                jugador.CallbackChannel.NotificarResultadoDado(nombreUsuario, resultadoDado);
                                Console.WriteLine($"Resultado del dado ({resultadoDado}) notificado a {jugador.NombreUsuario}.");
                            }
                            catch (EndpointNotFoundException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                            }
                            catch (CommunicationException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                            }
                            catch (TimeoutException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
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

        public static void ImprimirCallbacks()
        {
            if (_callbacks.Count == 0)
            {
                Console.WriteLine("El diccionario _callbacks está vacío.");
                return;
            }

            Console.WriteLine("Contenido del diccionario _callbacks:");
            foreach (var entry in _callbacks)
            {
                Console.WriteLine($"Jugador: {entry.Key} - Callback registrado: {entry.Value != null}");
            }
        }

        private void InicializarMazo(string idPartida)
        {
            ImprimirCallbacks();
            var cartasTemporales = new List<Carta>();

            int idCounter = 1;

            cartasTemporales.AddRange(Enumerable.Repeat(0, 1)
                                                 .Select(_ => new Carta("Carta1", idCounter++, 1)));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 2)
                                                 .Select(_ => new Carta("Carta2", idCounter++, 2)));
            cartasTemporales.AddRange(Enumerable.Repeat(0, 6)
                                                 .Select(_ => new Carta("Carta3", idCounter++, 3)));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 3)
                                                 .Select(_ => new Carta("Carta4", idCounter++, 4)));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 2)
                                                 .Select(_ => new Carta("Carta5", idCounter++, 5)));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 1)
                                                 .Select(_ => new Carta("Carta6", idCounter++, 6)));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 1)
                                                 .Select(_ => new Carta("Carta7", idCounter++, 7)));

            cartasTemporales.AddRange(Enumerable.Repeat(0, 1)
                                                 .Select(_ => new Carta("Carta8", idCounter++, 8)));
            
            cartasTemporales.Add(cartaParteTrasera);

            CartasEnMazo = cartasTemporales;
        }

        
        private void BarajarMazo(string idPartida)
        {
            ImprimirCallbacks();
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
            CartasEnMazo.Add(cartaParteTrasera);
        }


        private List<Carta> RepartirCartas(int cantidad)
        {
            ImprimirCallbacks();
            var cartasRepartidas = CartasEnMazo.Take(cantidad).ToList();
            CartasEnMazo = CartasEnMazo.Skip(cantidad).ToList();
            return cartasRepartidas;
        }

        private void AsignarCartasAJugadores(string idPartida)
        {
            ImprimirCallbacks();
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
            ImprimirCallbacks();
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
                        catch (EndpointNotFoundException ex)
                        {
                            ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        }
                        catch (CommunicationException ex)
                        {
                            ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        }
                        catch (TimeoutException ex)
                        {
                            ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
        

        public void CrearPartida(List<JugadorPartida> jugadores, string idPartida)
        {
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
            catch (EndpointNotFoundException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
            }
            catch (CommunicationException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
            }
            catch (TimeoutException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
                catch (EndpointNotFoundException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (CommunicationException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (TimeoutException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
            }
            else
            {
                Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
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
                            catch (EndpointNotFoundException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                            }
                            catch (CommunicationException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                            }
                            catch (TimeoutException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
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

                if (jugador != null)
                {
                    try
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            jugador.CallbackChannel.NotificarCartaAgregadaAMano(carta);
                        }
                        else
                        {
                            Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                        }
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                }
                else
                {
                    Console.WriteLine($"No se encontró un jugador con el nombre de usuario {nombreUsuario} en la partida {idPartida}.");
                }
            }
            else
            {
                Console.WriteLine($"El jugador {nombreUsuario} no está en la partida o no tiene cartas en mano registradas.");
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
                    try
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            jugador.CallbackChannel.NotificarFichaTomadaMesa(jugadorTurnoActual.NombreUsuario, idFicha);
                        }
                        else
                        {
                            Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                        }
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
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

                    try
                    {
                        if (jugadorActual.CallbackChannel != null)
                        {
                            jugadorActual.CallbackChannel.NotificarCartaUtilizada(cartaUtilizada.IdCarta);
                        }
                        else
                        {
                            Console.WriteLine($"El jugador {jugadorActual.NombreUsuario} no tiene un callback válido.");
                        }
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
                try
                {
                    if (jugador.CallbackChannel != null)
                    {
                        jugador.CallbackChannel.NotificarCartaAgregadaADescarte(cartaUtilizada);
                    }
                    else
                    {
                        Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                    }
                }
                catch (EndpointNotFoundException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (CommunicationException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (TimeoutException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
                    try
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            jugador.CallbackChannel.NotificarFichaDevuelta(idFicha, jugadorTurnoActual.NombreUsuario);
                        }
                        else
                        {
                            Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                        }
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
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

                try
                {
                    if (jugador.CallbackChannel != null)
                    {
                        jugador.CallbackChannel.NotificarCartaAgregadaAEscondite(carta.IdCarta);
                    }
                    else
                    {
                        Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                    }
                }
                catch (EndpointNotFoundException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (CommunicationException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (TimeoutException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
            var jugadorAtacante = jugadores[partida.TurnoActual];
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
                        try
                        {
                            if (jugador.CallbackChannel != null)
                            {
                                jugador.CallbackChannel.NotificarIntentoRobo(nombreDefensor);
                            }
                            else
                            {
                                Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                            }
                        }
                        catch (EndpointNotFoundException ex)
                        {
                            ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        }
                        catch (CommunicationException ex)
                        {
                            ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        }
                        catch (TimeoutException ex)
                        {
                            ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
                ProcesarRoboEnProgreso(partida);
            }
            else
            {
                ProcesarNuevoRobo(partida, nombreDefensor);
            }
        }

        private void ProcesarRoboEnProgreso(Partida partida)
        {
            var atacante = partida.RoboEnProgreso.Atacante;
            var defensor = partida.RoboEnProgreso.Defensor;

            if (CartasEnMano[defensor.NombreUsuario].Count > 0)
            {
                var cartaRobada = RobarCartaDeJugador(defensor.NombreUsuario, atacante.NombreUsuario);
                NotificarJugadores(partida, cartaRobada, defensor.NombreUsuario, atacante.NombreUsuario);
            }

            partida.RoboEnProgreso = null;
        }

        private void ProcesarNuevoRobo(Partida partida, string nombreDefensor)
        {
            var jugadorObjetivoRobo = partida.Jugadores.FirstOrDefault(j => j.NombreUsuario == nombreDefensor);
            if (jugadorObjetivoRobo == null)
            {
                Console.WriteLine($"El jugador {nombreDefensor} no existe en la partida.");
                return;
            }

            var jugadorTurnoActual = partida.Jugadores[partida.TurnoActual];
            var cartaRobada = RobarCartaDeJugador(jugadorObjetivoRobo.NombreUsuario, jugadorTurnoActual.NombreUsuario);
            NotificarJugadores(partida, cartaRobada, jugadorObjetivoRobo.NombreUsuario, jugadorTurnoActual.NombreUsuario);
        }

        private Carta RobarCartaDeJugador(string nombreDefensor, string nombreAtacante)
        {
            var cartasDefensor = CartasEnMano[nombreDefensor];
            var cartaRobada = cartasDefensor[new Random().Next(cartasDefensor.Count)];
            cartasDefensor.Remove(cartaRobada);

            CartasEnMano[nombreAtacante].Add(cartaRobada);
            return cartaRobada;
        }

        private void NotificarJugadores(Partida partida, Carta cartaRobada, string nombreDefensor, string nombreAtacante)
        {
            foreach (var jugador in partida.Jugadores)
            {
                try
                {
                    if (jugador.CallbackChannel != null)
                    {
                        jugador.CallbackChannel.NotificarCartaRobada(cartaRobada, nombreDefensor, nombreAtacante);
                    }
                    else
                    {
                        Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                    }
                }
                catch (EndpointNotFoundException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (CommunicationException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (TimeoutException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
                    try
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            jugador.CallbackChannel.NotificarIntentoRobo(partida.RoboEnProgreso.Defensor.NombreUsuario);
                        }
                        else
                        {
                            Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                        }
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    
                }
            }
        }

        public void RobarCartaEsconditeAJugador(string nombreDefensor, string idPartida, bool cartaDuplicacionActiva)
        {
            var partida = partidas[idPartida];
            var jugadores = partida.Jugadores;
            var jugadorObjetivoRobo = jugadores.FirstOrDefault(c => c.NombreUsuario == nombreDefensor);
            var jugadorAtacante = jugadores[partida.TurnoActual];

            if (jugadorObjetivoRobo == null)
            {
                Console.WriteLine("Jugador objetivo de robo no encontrado.");
                return;
            }

            int cartasARobar = cartaDuplicacionActiva ? 2 : 1;
            var cartasBloqueoRobo = ObtenerCartasBloqueo(nombreDefensor);

            if (cartasBloqueoRobo.Count == 0)
            {
                RobarCartas(partida.IdPartida, nombreDefensor, cartasARobar);
            }
            else
            {
                IniciarRoboEnProgreso(partida, jugadorAtacante, jugadorObjetivoRobo);
                NotificarIntentoRobo(jugadores, nombreDefensor);
            }
        }

        private List<Carta> ObtenerCartasBloqueo(string nombreDefensor)
        {
            return CartasEnMano[nombreDefensor]
                .Where(c => c.Tipo == "Carta7" || c.Tipo == "Carta8")
                .ToList();
        }

        private void RobarCartas(string idPartida, string nombreDefensor, int cantidad)
        {
            for (int i = 0; i < cantidad; i++)
            {
                RobarCartaEscondite(idPartida, nombreDefensor);
            }
        }

        private void IniciarRoboEnProgreso(Partida partida, JugadorPartida atacante, JugadorPartida defensor)
        {
            partida.RoboEnProgreso = new RoboContexto
            {
                Atacante = atacante,
                Defensor = defensor
            };
        }

        private void NotificarIntentoRobo(List<JugadorPartida> jugadores, string nombreDefensor)
        {
            foreach (var jugador in jugadores)
            {
                try
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
                catch (EndpointNotFoundException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (CommunicationException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (TimeoutException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
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
                        try
                        {
                            if (jugador.CallbackChannel != null)
                            {
                                jugador.CallbackChannel.NotificarCartaEsconditeRobada(cartaRobada, defensor.NombreUsuario, atacante.NombreUsuario);
                            }
                            else
                            {
                                Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                            }
                        }
                        catch (EndpointNotFoundException ex)
                        {
                            ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        }
                        catch (CommunicationException ex)
                        {
                            ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        }
                        catch (TimeoutException ex)
                        {
                            ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        }
                        
                    }
                }
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
                    try
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            jugador.CallbackChannel.NotificarCartaEsconditeRobada(cartaRobada, jugadorObjetivoRobo.NombreUsuario, jugadorTurnoActual.NombreUsuario);
                        }
                        else
                        {
                            Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un callback válido.");
                        }
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
                            catch (EndpointNotFoundException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                            }
                            catch (CommunicationException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                            }
                            catch (TimeoutException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
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

                foreach( var jugador in jugadores)
                {
                    try
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
                    catch (EndpointNotFoundException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
                            try
                            {
                                if (jugador.CallbackChannel != null)
                                {
                                    jugador.CallbackChannel.NotificarPreguntaJugadores(jugadorTurnoActual.NombreUsuario, cartaRevelada.Tipo);
                                }
                                else
                                {
                                    Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un canal de callback válido.");
                                }
                            }
                            catch (EndpointNotFoundException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                            }
                            catch (CommunicationException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                            }
                            catch (TimeoutException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                            }
                            
                        }
                        else
                        {
                            try
                            {
                                if (jugadorTurnoActual.CallbackChannel != null)
                                {
                                    jugadorTurnoActual.CallbackChannel.NotificarNumeroJugadoresGuardaronCarta(0);
                                }
                                else
                                {
                                    Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un canal de callback válido.");
                                }
                            }
                            catch (EndpointNotFoundException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                            }
                            catch (CommunicationException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                            }
                            catch (TimeoutException ex)
                            {
                                ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
                    try
                    {
                        if (jugadorTurnoActual.CallbackChannel != null)
                        {
                            jugadorTurnoActual.CallbackChannel.NotificarNumeroJugadoresGuardaronCarta(jugadoresGuardaronCarta);
                        }
                        else
                        {
                            Console.WriteLine($"El jugador {jugadorTurnoActual.NombreUsuario} no tiene un canal de callback válido.");
                        }
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
                    try
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            jugador.CallbackChannel.NotificarMazoRevelado();
                        }
                        else
                        {
                            Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un canal de callback válido.");
                        }
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
                    try
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
                    catch (EndpointNotFoundException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                }
            }
            else
            {
                Console.WriteLine("Partida no encontrada");
            }
        }
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
                try
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
                catch (EndpointNotFoundException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (CommunicationException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (TimeoutException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
                    try
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            jugador.CallbackChannel.NotificarPararTirarDado();
                        }
                        else
                        {
                            Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un canal de callback válido.");
                        }
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    
                }
            }
            else
            {
                Console.Write("Partida no encontrada");
            }
        }

        public void ActualizarDecisionTurno(string idPartida)
        {
            if (partidas.ContainsKey(idPartida))
            {
                var partida = partidas[idPartida];
                var jugadores = partida.Jugadores;

                foreach( var jugador in jugadores)
                {
                    try
                    {
                        if (jugador.CallbackChannel != null)
                        {
                            jugador.CallbackChannel.NotificarActualizacionDecisionTurno();
                        }
                        else
                        {
                            Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un canal de callback válido.");
                        }
                    }
                    catch (EndpointNotFoundException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    }
                }
            }
            else
            {
                Console.Write("Partida no encontrada");
            }
        }

        public void EstablecerModoSeleccionCarta(string idPartida, int idModoSeleccion, string nombreJugador)
        {
            if (partidas.ContainsKey(idPartida))
            {
                var partida = partidas[idPartida];
                var jugadores = partida.Jugadores;
                var jugador = jugadores.FirstOrDefault(j => j.NombreUsuario == nombreJugador);

                try
                {
                    if (jugador.CallbackChannel != null)
                    {
                        jugador.CallbackChannel.NotificarModoSeleccionCarta(idModoSeleccion);
                    }
                    else
                    {
                        Console.WriteLine($"El jugador {jugador.NombreUsuario} no tiene un canal de callback válido.");
                    }
                }
                catch (EndpointNotFoundException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (CommunicationException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch (TimeoutException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
            }
            else
            {
                Console.Write("Partida no encontrada");
            }
            
        }
    }
}


