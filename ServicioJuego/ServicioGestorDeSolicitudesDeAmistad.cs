using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using AccesoDatos;
using System.Threading.Tasks;
using AccesoDatos.DAO;

namespace ServicioJuego
{
    public partial class ImplementacionServicio :  IGestorDeSolicitudesDeAmistad
    {

        private static Dictionary<string, IGestorDeSolicitudesDeAmistadCallBack> amistadEnLinea = new Dictionary<string, IGestorDeSolicitudesDeAmistadCallBack>();


        public void AgregarADiccionarioAmistadesEnLinea(string nombreUsuarioJugadorActual)
        {
            IGestorDeSolicitudesDeAmistadCallBack canalDeDevoluciónUsuarioActual = OperationContext.Current.GetCallbackChannel<IGestorDeSolicitudesDeAmistadCallBack>();

            if (!amistadEnLinea.ContainsKey(nombreUsuarioJugadorActual))
            {
                amistadEnLinea.Add(nombreUsuarioJugadorActual, canalDeDevoluciónUsuarioActual);
            }
            else
            {
                amistadEnLinea[nombreUsuarioJugadorActual] = canalDeDevoluciónUsuarioActual;
            }
        }



        public void EnviarSolicitudAmistad(string nombreUsuarioJugadorRemitente, string nombreUsuarioJugadorSolicitado)
        {
            lock (objetoDeBloqueo)
            {
                if (amistadEnLinea.ContainsKey(nombreUsuarioJugadorSolicitado))
                {
                    try
                    {
                        amistadEnLinea[nombreUsuarioJugadorSolicitado].NotificarNuevaSolicitudAmistad(nombreUsuarioJugadorRemitente);
                    }
                    catch (CommunicationException ex)
                    {
                        Console.WriteLine(ex);
                        EliminarDeDiccionarioAmistadesEnLinea(nombreUsuarioJugadorRemitente);
                    }
                    catch (TimeoutException ex)
                    {
                        Console.WriteLine(ex);
                        EliminarDeDiccionarioAmistadesEnLinea(nombreUsuarioJugadorRemitente);
                    }
                }
            }
        }

        public void AceptarSolicitudAmistad(int idJugadorSolicitado, string nombreUsuarioJugadorSolicitado, string nombreUsuarioJugadorRemitente)
        {
            ImplementacionServicio usuarioAccesoDatos = new ImplementacionServicio();
            AmistadDao solicitudAmistadAccesoDatos = new AmistadDao();

            try
            {
                int idJugadorRemitente = usuarioAccesoDatos.ObtenerIdJugadorPorNombreUsuario(nombreUsuarioJugadorRemitente);
                int filasAfectadas = solicitudAmistadAccesoDatos.ActualizarSolicitudAmistad_Aceptada(idJugadorSolicitado, idJugadorRemitente);

                if (filasAfectadas > 0)
                {
                    InformarSolicitusAmistadAceptada(nombreUsuarioJugadorRemitente, nombreUsuarioJugadorSolicitado);
                    InformarSolicitusAmistadAceptada(nombreUsuarioJugadorSolicitado, nombreUsuarioJugadorRemitente);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                
            }
        }

        private void InformarSolicitusAmistadAceptada(string nombreUsuarioObjetivo, string nombreUsuarioNuevoAmigo)
        {
            if (amistadEnLinea.ContainsKey(nombreUsuarioObjetivo))
            {
                try
                {
                    amistadEnLinea[nombreUsuarioObjetivo].NotificarSolicitudAmistadAceptada(nombreUsuarioNuevoAmigo);
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine(ex);
                    EliminarDeDiccionarioAmistadesEnLinea(nombreUsuarioObjetivo);
                }
            }
        }

        public void RechazarSolicitudAmistad(int idJugadorActual, string nombreUsuario)
        {
            ImplementacionServicio usuarioAccesoDatos = new ImplementacionServicio();
            AmistadDao SolicitudAmistadAccesoDatos = new AmistadDao();

            try
            {
                int idJugadorAceptado = usuarioAccesoDatos.ObtenerIdJugadorPorNombreUsuario(nombreUsuario);

                SolicitudAmistadAccesoDatos.BorrarSolicitudAmistad(idJugadorActual, idJugadorAceptado);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        public void EliminarAmigo(int idJugadorActual, string nombreJugadorActual, string nombreUsuarioAmigoEliminado)
        {
            lock (objetoDeBloqueo)
            {
                ImplementacionServicio usuarioAccesoDatos = new ImplementacionServicio();
                AmistadDao SolicitudAmistadAccesoDatos = new AmistadDao();

                try
                {
                    int idPlayerFriend = usuarioAccesoDatos.ObtenerIdJugadorPorNombreUsuario(nombreUsuarioAmigoEliminado);
                    int rowsAffected = SolicitudAmistadAccesoDatos.BorrarAmistad(idJugadorActual, idPlayerFriend);

                    if (rowsAffected > 0)
                    {
                        InformarAmigoEliminado(nombreJugadorActual, nombreUsuarioAmigoEliminado);
                        InformarAmigoEliminado(nombreUsuarioAmigoEliminado, nombreJugadorActual);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void InformarAmigoEliminado(string nombreUsuarioObjetivo, string nombreUsuarioAmigoEliminado)
        {
            if (amistadEnLinea.ContainsKey(nombreUsuarioObjetivo))
            {
                try
                {
                    amistadEnLinea[nombreUsuarioObjetivo].NotificarAmigoEliminado(nombreUsuarioAmigoEliminado);
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine(ex);
                    EliminarDeDiccionarioAmistadesEnLinea(nombreUsuarioObjetivo);
                }
            }
        }

        public void EliminarDeDiccionarioAmistadesEnLinea(string nombreUsuario)
        {
            amistadEnLinea.Remove(nombreUsuario);
        }


    }
}
