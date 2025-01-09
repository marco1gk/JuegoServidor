using AccesoDatos.Modelo;
using AccesoDatos.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServicioJuego
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.PerSession)]
    public partial class ImplementacionServicio : ISalaEsperaServicio
    {
        private static readonly Dictionary<string, List<JugadorSalaEspera>> salasEspera = new Dictionary<string, List<JugadorSalaEspera>>();
        private static readonly object lockUsuarios = new object();
        private static readonly Dictionary<string, object> codigosGenerados = new Dictionary<string, object>();


        public void ExpulsarJugadorSalaEspera(string codigoSalaEspera, string nombreUsuario)
        {
            RealizarSalidaLobby(codigoSalaEspera, nombreUsuario, true);
        }
        private static void GestionarJugadorExpulsado(JugadorSalaEspera jugadorAEliminar)
        {
            try
            {
                jugadorAEliminar.CallbackChannel.NotificarExpulsadoSalaEspera();
            }
            catch (CommunicationException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(ex);
            }
        }

        public void SalirSalaEspera(string codigoSalaEspera, string nombreUsuario)
        {
            RealizarSalidaLobby(codigoSalaEspera, nombreUsuario, false);
        }


        private void RealizarSalidaLobby(string codigoSalaEspera, string nombreUsuario, bool expulsado)
        {
            if (salasEspera.ContainsKey(codigoSalaEspera))
            {
                List<JugadorSalaEspera> jugadores = salasEspera[codigoSalaEspera];
                JugadorSalaEspera jugadorAEliminar = null;

                int anfitrionIndice = 0;
                int indiceJugadorEliminado = anfitrionIndice;

                foreach (JugadorSalaEspera jugador in jugadores)
                {
                    if (jugador.NombreUsuario.Equals(nombreUsuario))
                    {
                        jugadorAEliminar = jugador;
                        break;
                    }
                    else
                    {
                        indiceJugadorEliminado++;
                    }
                }

                if (expulsado)
                {
                    GestionarJugadorExpulsado(jugadorAEliminar);
                }

                jugadores.Remove(jugadorAEliminar);
                salasEspera[codigoSalaEspera] = jugadores;

                NotificarJugadorSalioSalaEspera(jugadores, nombreUsuario, indiceJugadorEliminado, codigoSalaEspera, expulsado);

                if (indiceJugadorEliminado == anfitrionIndice)
                {
                    salasEspera.Remove(codigoSalaEspera);
                }
            }
        }

        private void NotificarJugadorSalioSalaEspera(List<JugadorSalaEspera> jugadores, string nombreUsuario, int indiceJugadorEliminado, string codigoSalaEspera, bool esExplusaldo)
        {
            int hostIndex = 0;

            foreach (var callbackChannel in jugadores.Select(p => p.CallbackChannel).ToList())
            {
                try
                {
                    if (indiceJugadorEliminado != hostIndex)
                    {
                        callbackChannel.NotificarJugadorSalioSalaEspera(nombreUsuario);
                    }
                    else
                    {
                        callbackChannel.NotificarAnfritionJugadorSalioSalaEspera();
                    }
                }
                catch (CommunicationException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    RealizarSalidaLobby(codigoSalaEspera, nombreUsuario, esExplusaldo);
                }
                catch (Exception ex)
                {
                    RealizarSalidaLobby(codigoSalaEspera, nombreUsuario, esExplusaldo);
                    ManejadorExcepciones.ManejarFatalExcepcion(ex);
                }
            }
        }

        public void InvitarAmigoASala(string codigoSalaEspera, string nombreAmigo, string nombreInvitador)
        {
            IGestorUsuarioCallback amigoCallback = BuscarJugadorEnLinea(nombreAmigo);
        
            if (salasEspera.ContainsKey(codigoSalaEspera))
            {
                if (amigoCallback != null)
                {
                    try
                    {
                        Task.Run(() =>
                        {
                            amigoCallback.NotificarInvitacionSala(nombreInvitador, codigoSalaEspera);
                        });

                        Console.WriteLine($"Invitación enviada a {nombreAmigo} para unirse a la sala {codigoSalaEspera}.");
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        Console.WriteLine($"Error al invitar a {nombreAmigo}: {ex.Message}");
                    }
                }
            }
        }

        private static IGestorUsuarioCallback BuscarJugadorEnLinea(string nombreAmigo)
        {
            if (usuariosEnLinea.ContainsKey(nombreAmigo))
            {
                return usuariosEnLinea[nombreAmigo];
            }
            return null;
        }
        public List<string> ObtenerCodigosGenerados()
        {
            return codigosGenerados.Keys.ToList();
        }


        public void MandarMensaje(string mensaje)
        {
            IGestorSalasEsperasCallBack canalUsuarioActualCallback = OperationContext.Current.GetCallbackChannel<IGestorSalasEsperasCallBack>();

            string codigoSalaEspera = salasEspera
                .Where(lobby => lobby.Value.Any(player => player.CallbackChannel == canalUsuarioActualCallback))
                .Select(lobby => lobby.Key)
                .FirstOrDefault();

            if (codigoSalaEspera == null)
            {
                return;
            }

            JugadorSalaEspera jugadorActual = salasEspera[codigoSalaEspera]
                .FirstOrDefault(player => player.CallbackChannel == canalUsuarioActualCallback);

            if (jugadorActual == null)
            {
                return;
            }

            string nombreUsuario = jugadorActual.NombreUsuario;

            List<JugadorSalaEspera> jugadoresSalaEspera = salasEspera[codigoSalaEspera];
            foreach (var jugador in jugadoresSalaEspera)
            {
                try
                {
                    jugador.CallbackChannel.RecibirMensaje(nombreUsuario, mensaje);
                }
                catch (CommunicationException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    Console.WriteLine($"Error al enviar mensaje a {jugador.NombreUsuario}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    ManejadorExcepciones.ManejarFatalExcepcion(ex);
                }
            }

            Console.WriteLine($"[{nombreUsuario}] envió un mensaje en la sala {codigoSalaEspera}: {mensaje}");
        }


        public void CrearSalaEspera(JugadorSalaEspera jugador)
        {
            IGestorSalasEsperasCallBack actualUsuarioCanalCallback = OperationContext.Current.GetCallbackChannel<IGestorSalasEsperasCallBack>();
            jugador.CallbackChannel = actualUsuarioCanalCallback;

            List<JugadorSalaEspera> jugadores = new List<JugadorSalaEspera> { jugador };
            string codigoSalaEspera = GenerarCodigoSalaEspera();
            codigosGenerados.Add(codigoSalaEspera, null);

            salasEspera.Add(codigoSalaEspera, jugadores);

            try
            {
                actualUsuarioCanalCallback.NotificarSalaEsperaCreada(codigoSalaEspera);
            }
            catch (CommunicationException ex)

            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                RealizarSalidaLobby(codigoSalaEspera, jugador.NombreUsuario, false);
            }
            catch (TimeoutException ex)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(ex);
                RealizarSalidaLobby(codigoSalaEspera, jugador.NombreUsuario, false);
            }
            
        }

        public void UnirSalaEsperaComoAnfitrion(string codigoSalaEspera)
        {
            if (salasEspera.ContainsKey(codigoSalaEspera))
            {
                IGestorSalasEsperasCallBack usuarioActualCanalCallback = OperationContext.Current.GetCallbackChannel<IGestorSalasEsperasCallBack>();
                List<JugadorSalaEspera> jugadores = salasEspera[codigoSalaEspera];
                JugadorSalaEspera anfitrion = jugadores[0];

                anfitrion.CallbackChannel = usuarioActualCanalCallback;

                try
                {
                    usuarioActualCanalCallback.NotificarSalaEsperaCreada(codigoSalaEspera);
                }
                catch (CommunicationException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    RealizarSalidaLobby(codigoSalaEspera, anfitrion.NombreUsuario, false);
                }
            }
        }
        public void UnirseSalaEspera(string codigoSalaEspera, JugadorSalaEspera jugador)
        {
            IGestorSalasEsperasCallBack currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IGestorSalasEsperasCallBack>();
            jugador.CallbackChannel = currentUserCallbackChannel;

            try
            {
                if (!salasEspera.ContainsKey(codigoSalaEspera))
                {
                    currentUserCallbackChannel.NotificarSalaEsperaNoExiste();
                    return;
                }

                var jugadoresEnSala = salasEspera[codigoSalaEspera];
                if (jugadoresEnSala.Count >= 4)
                {
                    currentUserCallbackChannel.NotificarSalaEsperaLlena();
                    return;
                }

                ProcesarJugador(codigoSalaEspera, jugador, jugadoresEnSala);
            }
            catch (CommunicationException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(ex);
            }
        }

        private void ProcesarJugador(string codigoSalaEspera, JugadorSalaEspera jugador, List<JugadorSalaEspera> jugadoresEnSala)
        {
            var jugadorExistente = jugadoresEnSala.FirstOrDefault(j => j.NombreUsuario == jugador.NombreUsuario);

            if (jugadorExistente != null)
            {
                ActualizarCanalSiEsNecesario(jugadorExistente, jugador.CallbackChannel);
            }
            else
            {
                jugadoresEnSala.Add(jugador);
                Console.WriteLine($"Jugador {jugador.NombreUsuario} se unió a la sala {codigoSalaEspera}.");
            }

            NotificarJugadoresSalaEspera(codigoSalaEspera, jugadoresEnSala);
        }

        private void ActualizarCanalSiEsNecesario(JugadorSalaEspera jugadorExistente, IGestorSalasEsperasCallBack nuevoCallbackChannel)
        {
            if (jugadorExistente.CallbackChannel != nuevoCallbackChannel)
            {
                jugadorExistente.CallbackChannel = nuevoCallbackChannel;
                Console.WriteLine($"Canal de callback actualizado para el jugador {jugadorExistente.NombreUsuario}.");
            }
        }


        public void IniciarPartida(string codigoSalaEspera)
        {
            Console.WriteLine($"Intentando iniciar partida para la sala con código: {codigoSalaEspera}");

            if (!salasEspera.ContainsKey(codigoSalaEspera))
            {
                Console.WriteLine($"La sala con código {codigoSalaEspera} no existe.");
                return;
            }

            var jugadoresEnSalaEspera = salasEspera[codigoSalaEspera];
            var jugadoresPartida = CrearJugadoresPartida(jugadoresEnSalaEspera);

            var jugadoresConError = NotificarJugadores(jugadoresEnSalaEspera, jugadoresPartida);

            if (jugadoresConError.Any())
            {
                ManejarJugadoresConError(jugadoresEnSalaEspera, jugadoresConError);
            }
        }

        private List<JugadorPartida> CrearJugadoresPartida(List<JugadorSalaEspera> jugadoresEnSalaEspera)
        {
            return jugadoresEnSalaEspera.Select(j => new JugadorPartida
            {
                NombreUsuario = j.NombreUsuario,
                NumeroFotoPerfil = j.NumeroFotoPerfil
            }).ToList();
        }

        private List<JugadorSalaEspera> NotificarJugadores(List<JugadorSalaEspera> jugadoresEnSalaEspera, List<JugadorPartida> jugadoresPartida)
        {
            var jugadoresConError = new List<JugadorSalaEspera>();

            foreach (var jugador in jugadoresEnSalaEspera)
            {
                if (!IntentarNotificarJugador(jugador, jugadoresPartida))
                {
                    jugadoresConError.Add(jugador);
                }
            }

            return jugadoresConError;
        }

        private bool IntentarNotificarJugador(JugadorSalaEspera jugador, List<JugadorPartida> jugadoresPartida)
        {
            int intentosMaximos = 3;

            for (int intentos = 0; intentos < intentosMaximos; intentos++)
            {
                try
                {
                    VerificarCanalJugador(jugador);
                    jugador.CallbackChannel.NotificarIniciarPartida(jugadoresPartida.ToArray());
                    Console.WriteLine($"Jugador {jugador.NombreUsuario} ha sido notificado correctamente.");
                    return true;
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine($"Error al notificar al jugador {jugador.NombreUsuario}: {ex.Message}");
                    RestaurarCanalJugador(jugador);
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine($"Timeout al notificar al jugador {jugador.NombreUsuario}: {ex.Message}");
                }
            }

            Console.WriteLine($"El jugador {jugador.NombreUsuario} ha sido marcado como inactivo.");
            return false;
        }

        private void VerificarCanalJugador(JugadorSalaEspera jugador)
        {
            var canalCliente = jugador.CallbackChannel as IClientChannel;
            if (canalCliente == null) return;

            if (canalCliente.State == CommunicationState.Faulted || canalCliente.State == CommunicationState.Closed)
            {
                Console.WriteLine($"Restaurando canal para {jugador.NombreUsuario} debido a estado {canalCliente.State}.");
                if (canalCliente.State != CommunicationState.Closed && canalCliente.State != CommunicationState.Closing)
                {
                    canalCliente.Close();
                }
                jugador.CallbackChannel = OperationContext.Current.GetCallbackChannel<IGestorSalasEsperasCallBack>();
            }
        }

        private void RestaurarCanalJugador(JugadorSalaEspera jugador)
        {
            var canalCliente = jugador.CallbackChannel as IClientChannel;
            if (canalCliente != null && canalCliente.State == CommunicationState.Faulted)
            {
                if (canalCliente.State != CommunicationState.Closed && canalCliente.State != CommunicationState.Closing)
                {
                    canalCliente.Close();
                }
                jugador.CallbackChannel = OperationContext.Current.GetCallbackChannel<IGestorSalasEsperasCallBack>();
                Console.WriteLine($"Nuevo canal creado para {jugador.NombreUsuario}.");
            }
        }

        private void ManejarJugadoresConError(List<JugadorSalaEspera> jugadoresEnSalaEspera, List<JugadorSalaEspera> jugadoresConError)
        {
            lock (lockUsuarios)
            {
                foreach (var jugador in jugadoresConError)
                {
                    Console.WriteLine($"Se marcó al jugador {jugador.NombreUsuario} como inactivo.");
                }
                jugadoresConError.ForEach(j => jugadoresEnSalaEspera.Remove(j));
                Console.WriteLine($"{jugadoresConError.Count} jugadores fueron removidos o marcados como inactivos.");
            }
        }


        private void NotificarJugadorIngresoSalaEspera(List<JugadorSalaEspera> jugadoresSalaEspera, JugadorSalaEspera jugadorIngresando, int numeroJugadoresSalaEspera, string codigoSalaEspera)
        {
            foreach (var jugador in jugadoresSalaEspera.ToList())
            {
                try
                {
                    jugador.CallbackChannel.NotificarJugadorSeUnioSalaEspera(jugadorIngresando, numeroJugadoresSalaEspera);
                }
                catch (CommunicationException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    RealizarSalidaLobby(codigoSalaEspera, jugador.NombreUsuario, false);
                }
                catch (Exception ex)
                {
                    ManejadorExcepciones.ManejarFatalExcepcion(ex);
                }
            }
        }
       
        private string GenerarCodigoSalaEspera()
        {
            int longitud = 6;
            string caracteres = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();

            char[] codigo = new char[longitud];

            for (int i = 0; i < longitud; i++)
            {
                codigo[i] = caracteres[random.Next(caracteres.Length)];
            }

            string codigoSalaEspera = new string(codigo);

            return salasEspera.ContainsKey(codigoSalaEspera) ? GenerarCodigoSalaEspera() : codigoSalaEspera;
        }


        private static void NotificarJugadoresSalaEspera(string codigoSalaEspera, List<JugadorSalaEspera> jugadores)
        {
            foreach (var jugador in jugadores)
            {
                try
                {
                    jugador.CallbackChannel.NotificarJugadoresEnSalaEspera(codigoSalaEspera, jugadores);
                }
                catch (CommunicationException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                }
                catch(Exception ex)
                {
                    ManejadorExcepciones.ManejarFatalExcepcion(ex);
                }
            }
        }


        
    }
}