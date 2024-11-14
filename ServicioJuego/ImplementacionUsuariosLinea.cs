using AccesoDatos.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using AccesoDatos;


namespace ServicioJuego
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class ImplementacionServicio : IGestorUsuariosConectados
    {
        private static readonly object objetoDeBloqueo = new object();
        private static Dictionary<string, IGestorUsuarioCallback> usuariosEnLinea = new Dictionary<string, IGestorUsuarioCallback>();

        public void RegistrarUsuarioAUsuariosConectados(int jugadorId, string nombreUsuario)
        {
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
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
            }

            NotificarInicioDeSesiónAAmigos(jugadorId, nombreUsuario);

            // Iniciar hilo de verificación de conexión (ping)
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

                // Esperar 30 segundos antes de verificar nuevamente
                System.Threading.Thread.Sleep(30000);
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
                        Console.WriteLine(ex);
                        DesregistrarUsuarioDeUsuariosEnLinea(nombreUsuario);
                    }
                    catch (TimeoutException ex)
                    {
                        Console.WriteLine(ex);
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
                        Console.WriteLine(ex);
                        DesregistrarUsuarioDeUsuariosEnLinea(usuario.Key);
                    }
                    catch (TimeoutException ex)
                    {
                        Console.WriteLine(ex);
                        DesregistrarUsuarioDeUsuariosEnLinea(usuario.Key);
                    }
                }
            }
        }
    }

}
