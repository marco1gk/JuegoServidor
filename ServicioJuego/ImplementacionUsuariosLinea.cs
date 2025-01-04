using AccesoDatos.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using AccesoDatos;
using AccesoDatos.Utilidades;


namespace ServicioJuego
{
 
    public partial class ImplementacionServicio : IGestorUsuariosConectados
    {
        private static readonly object objetoDeBloqueo = new object();
        private static Dictionary<string, IGestorUsuarioCallback> usuariosEnLinea = new Dictionary<string, IGestorUsuarioCallback>();

        public void RegistrarUsuarioAUsuariosConectados(int jugadorId, string nombreUsuario)
        {
            Console.WriteLine("se conectó "+nombreUsuario);
            IGestorUsuarioCallback actualUsuarioCallbackCanal = OperationContext.Current.GetCallbackChannel<IGestorUsuarioCallback>();
            List<string> nombresUsuarioEnLinea = usuariosEnLinea.Keys.ToList();
            List<string> amigosEnLinea = new List<string>();

            if (!usuariosEnLinea.ContainsKey(nombreUsuario))
            {
                usuariosEnLinea.Add(nombreUsuario, actualUsuarioCallbackCanal);
            }
            else
            {
                usuariosEnLinea[nombreUsuario] = actualUsuarioCallbackCanal;
            }

            amigosEnLinea = nombresUsuarioEnLinea
                .Where(onlineUsername => EsAmigo(jugadorId, onlineUsername))
                .ToList();

            try
            {
                actualUsuarioCallbackCanal.NotificarAmigosEnLinea(amigosEnLinea);
            }
            catch (CommunicationException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                DesregistrarUsuarioDeUsuariosEnLinea(nombreUsuario);
            }
            catch (TimeoutException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                DesregistrarUsuarioDeUsuariosEnLinea(nombreUsuario);
            }

            NotificarInicioDeSesiónAAmigos(jugadorId, nombreUsuario);

            Task.Run(() => VerificarConexionUsuario(nombreUsuario));
        }

        private void VerificarConexionUsuario(string nombreUsuario)
        {
            while (true)
            {
                try
                {
                    if (usuariosEnLinea.ContainsKey(nombreUsuario))
                    {
                        usuariosEnLinea[nombreUsuario].Ping();
                    }
                }
                catch (CommunicationException)
                {
                    Console.WriteLine($"Usuario {nombreUsuario} desconectado.");
                    DesregistrarUsuarioDeUsuariosEnLinea(nombreUsuario);
                    break;
                }
                catch (TimeoutException)
                {
                    Console.WriteLine($"Usuario {nombreUsuario} desconectado.");
                    DesregistrarUsuarioDeUsuariosEnLinea(nombreUsuario);
                    break;
                }

                System.Threading.Thread.Sleep(10000); // Intervalo reducido a 10 segundos
            }
        }

        private void NotificarInicioDeSesiónAAmigos(int jugadorId, string nombreUsuario)
        {
            foreach (var usuario in usuariosEnLinea.ToList())
            {
                if (usuario.Key != nombreUsuario && EsAmigo(jugadorId, usuario.Key))
                {
                    try
                    {
                        usuario.Value.NotificarUsuarioConectado(nombreUsuario);
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        DesregistrarUsuarioDeUsuariosEnLinea(nombreUsuario);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        DesregistrarUsuarioDeUsuariosEnLinea(nombreUsuario);
                    }
                }
            }
        }

        private bool EsAmigo(int actualJugadorId, string nombreUsuarioEnLinea)
        {
            AmistadDao amistadDao = new AmistadDao();
            ImplementacionServicio usuarioA = new ImplementacionServicio();
            int idJugadorEnLinea = usuarioA.ObtenerIdJugadorPorNombreUsuario(nombreUsuarioEnLinea);

            bool esAmigo = amistadDao.EsAmigo(actualJugadorId, idJugadorEnLinea);

            return esAmigo;
        }

        public void DesregistrarUsuarioDeUsuariosEnLinea(string nombreUsuario)
        {
            if (usuariosEnLinea.ContainsKey(nombreUsuario))
            {
                usuariosEnLinea.Remove(nombreUsuario);

                foreach (var usuario in usuariosEnLinea.ToList())
                {
                    try
                    {
                        usuario.Value.NotificarUsuarioDesconectado(nombreUsuario);
                    }
                    catch (CommunicationException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        DesregistrarUsuarioDeUsuariosEnLinea(usuario.Key);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        DesregistrarUsuarioDeUsuariosEnLinea(usuario.Key);
                    }
                }
            }
        }


        public bool ReconectarUsuario(int idJugador, string nombreUsuario)
        {
            lock (objetoDeBloqueo)
            {
                if (usuariosEnLinea.ContainsKey(nombreUsuario))
                {
                    // Actualiza el canal de comunicación para la reconexión
                    usuariosEnLinea[nombreUsuario] = OperationContext.Current.GetCallbackChannel<IGestorUsuarioCallback>();

                    Console.WriteLine($"Usuario {nombreUsuario} se ha reconectado.");

                    // Sincroniza el estado del juego al usuario reconectado
                    Dictionary<string, string> estadoJuego = ObtenerEstadoActualDelJuego(); // Este método depende de tu lógica del juego
                    usuariosEnLinea[nombreUsuario].SincronizarEstado(estadoJuego);

                    return true;
                }
                else
                {
                    Console.WriteLine($"No se pudo reconectar al usuario {nombreUsuario}. No estaba registrado.");
                    return false;
                }
            }
        }

        private Dictionary<string, string> ObtenerEstadoActualDelJuego()
        {
            // Ejemplo de cómo podrías estructurar el estado del juego
            return new Dictionary<string, string>
    {
        { "jugador1", "posición: 10, puntuación: 150" },
        { "jugador2", "posición: 5, puntuación: 200" }
    };
        }


    }

}
