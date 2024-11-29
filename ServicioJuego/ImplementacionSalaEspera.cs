using AccesoDatos.Modelo;
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
    public partial class ImplementacionServicio : ILobbyManager
    {
        public static readonly Dictionary<string, List<JugadorSalaEspera>> salasEspera = new Dictionary<string, List<JugadorSalaEspera>>();
        private static readonly object lockUsuarios = new object();
        private static readonly Dictionary<string, object> codigosGenerados = new Dictionary<string, object>();





        public void InvitarAmigoASala(string codigoSalaEspera, string nombreAmigo, string nombreInvitador)
        {
            Console.WriteLine("codigo " + codigoSalaEspera);
            Console.WriteLine("nombreAmigo " + nombreAmigo);
            Console.WriteLine("nombreInvitador " + nombreInvitador);
            IGestorUsuarioCallback amigoCallback = BuscarJugadorEnLinea(nombreAmigo);
            IGestorSalasEsperasCallBack actualUsuarioCanalCallback = OperationContext.Current.GetCallbackChannel<IGestorSalasEsperasCallBack>();

            if (salasEspera.ContainsKey(codigoSalaEspera))
            {


                if (amistadEnLinea == null) { Console.WriteLine("es nula otraa vez"); }
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
                        Console.WriteLine($"Error al invitar a {nombreAmigo}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"El amigo {nombreAmigo} no está en línea o no está disponible.");
                }
            }
            else
            {
                Console.WriteLine($"La sala con código {codigoSalaEspera} no existe.");
            }
        }

        private IGestorUsuarioCallback BuscarJugadorEnLinea(string nombreAmigo)
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

                Console.WriteLine(ex.ToString());
                RealizarSalidaLobby(codigoSalaEspera, jugador.NombreUsuario, false);
            }
            catch (TimeoutException ex)
            {

                Console.WriteLine(ex.ToString());

                RealizarSalidaLobby(codigoSalaEspera, jugador.NombreUsuario, false);
            }
            
        }

        public string BuscarSalaEsperaDisponible()
        {
            if (salasEspera.Any())
            {
                return salasEspera.Keys.First();
            }
            return null;
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

                    Console.WriteLine(ex.ToString());
                    RealizarSalidaLobby(codigoSalaEspera, anfitrion.NombreUsuario, false);
                }
            }
        }

        public void UnirseSalaEspera(string codigoSalaEspera, JugadorSalaEspera jugador)
        {
            IGestorSalasEsperasCallBack currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IGestorSalasEsperasCallBack>();
            jugador.CallbackChannel = currentUserCallbackChannel;
            int maximoJugadores = 4;

            try
            {
                if (salasEspera.ContainsKey(codigoSalaEspera))
                {
                    List<JugadorSalaEspera> jugadoresEnSala = salasEspera[codigoSalaEspera];
                    int numeroJugadoresEnSala = jugadoresEnSala.Count;

                    if (numeroJugadoresEnSala < maximoJugadores)
                    {
                        jugador.CallbackChannel.NotificarJugadoresEnSalaEspera(codigoSalaEspera, jugadoresEnSala);
                        NotificarJugadorIngresoSalaEspera(jugadoresEnSala, jugador, numeroJugadoresEnSala, codigoSalaEspera);
                        jugadoresEnSala.Add(jugador);
                        NotificarPuedeIniciarJuegoSiEsAnfitrión(jugadoresEnSala);
                        NotificarJugadoresSalaEspera(codigoSalaEspera, jugadoresEnSala);
                    }
                    else
                    {
                        jugador.CallbackChannel.NotificarSalaEsperaLlena();
                    }
                }
                else
                {
                    jugador.CallbackChannel.NotificarSalaEsperaNoExiste();
                }
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void NotificarPuedeIniciarJuegoSiEsAnfitrión(List<JugadorSalaEspera> jugadoresEnSalaEspera)
        {
            bool puedeIniciar = jugadoresEnSalaEspera.Count >= 2;
            JugadorSalaEspera anfitrion = jugadoresEnSalaEspera[0];

            foreach (var jugador in jugadoresEnSalaEspera)
            {
                try
                {
                    jugador.CallbackChannel.NotificarPuedeIniciarPartida(jugador.Equals(anfitrion) && puedeIniciar);
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public void IniciarPartida(string codigoSalaEspera)
        {
            if (salasEspera.ContainsKey(codigoSalaEspera))
            {
                List<JugadorSalaEspera> jugadoresEnSalaEspera = salasEspera[codigoSalaEspera];
                List<JugadorPartida> jugadoresPartida = new List<JugadorPartida>();

                foreach (var jugador in jugadoresEnSalaEspera)
                {
                    JugadorPartida jugadorPartida = new JugadorPartida { NombreUsuario = jugador.NombreUsuario, NumeroFotoPerfil = jugador.NumeroFotoPerfil };
                    jugadoresPartida.Add(jugadorPartida);
                }

                foreach (var jugador in jugadoresEnSalaEspera)
                {
                    try
                    {
                        jugador.CallbackChannel.NotificarIniciarPartida(jugadoresPartida.ToArray());
                    }
                    catch (CommunicationException ex)
                    {
                        Console.WriteLine(ex.ToString());
                        RealizarSalidaLobby(codigoSalaEspera, jugador.NombreUsuario, false);
                    }

                }

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

                    Console.WriteLine(ex.ToString());
                    RealizarSalidaLobby(codigoSalaEspera, jugador.NombreUsuario, false);
                }
            }
        }



        public void SalirSalaEspera(string codigoSalaEspera, string nombreUsuario)
        {
            RealizarSalidaLobby(codigoSalaEspera, nombreUsuario, false);
        }

        public void ExpulsarJugadorSalaEspera(string codigoSalaEspera, string nombreUsuario)
        {
            RealizarSalidaLobby(codigoSalaEspera, nombreUsuario, true);
        }

        private void GestionarJugadorExpulsado(JugadorSalaEspera jugadorAEliminar)
        {
            try
            {
                jugadorAEliminar.CallbackChannel.NotificarExpulsadoSalaEspera();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
            }
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
                    Console.WriteLine(ex.ToString());
                    RealizarSalidaLobby(codigoSalaEspera, nombreUsuario, esExplusaldo);
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

        public void MandarMensaje(string mensaje)
        {
            IGestorSalasEsperasCallBack canalUsuarioActualCallback = OperationContext.Current.GetCallbackChannel<IGestorSalasEsperasCallBack>();

            string codigoSalaEspera = salasEspera
                .Where(lobby => lobby.Value.Any(player => player.CallbackChannel == canalUsuarioActualCallback))
                .Select(lobby => lobby.Key)
                .FirstOrDefault();

            if (codigoSalaEspera == null)
            {
                Console.WriteLine("No se pudo encontrar el lobby del jugador que envía el mensaje.");
                return;
            }

            string nombreUsuarioReceptor = salasEspera[codigoSalaEspera]
                .Where(jugador => jugador.CallbackChannel == canalUsuarioActualCallback)
                .Select(jugador => jugador.NombreUsuario)
                .FirstOrDefault();

            if (nombreUsuarioReceptor == null)
            {
                Console.WriteLine("No se pudo encontrar al jugador que envía el mensaje.");
                return;
            }

            List<JugadorSalaEspera> jugadoresSalaEspera = salasEspera[codigoSalaEspera];
            foreach (var jugador in jugadoresSalaEspera)
            {
                try
                {

                    jugador.CallbackChannel.RecibirMensaje(nombreUsuarioReceptor, mensaje);
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine($"Error al enviar mensaje a {jugador.NombreUsuario}: {ex.Message}");
                }
            }
        }

        public void NotificarJugadoresSalaEspera(string codigoSalaEspera, List<JugadorSalaEspera> jugadoresSalaEspera)
        {
            foreach (var jugador in jugadoresSalaEspera)
            {
                try
                {
                    jugador.CallbackChannel.NotificarJugadoresEnSalaEspera(codigoSalaEspera, jugadoresSalaEspera);
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine($"Error notificando a {jugador.NombreUsuario}: {ex.Message}");
             
                }
            }
        }

        
    }
}