using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using AccesoDatos;
using System.Threading.Tasks;
using AccesoDatos.DAO;
using AccesoDatos.Utilidades;
using AccesoDatos.Excepciones;
using ServicioJuego.Excepciones;

namespace ServicioJuego
{
    public partial class ImplementacionServicio :  IGestorDeSolicitudesDeAmistad
    {

        private static Dictionary<string, IGestorDeSolicitudesDeAmistadCallBack> amistadEnLinea = new Dictionary<string, IGestorDeSolicitudesDeAmistadCallBack>();

        public void AgregarADiccionarioAmistadesEnLinea(string nombreUsuarioDeActualJugador)
        {
            IGestorDeSolicitudesDeAmistadCallBack canalDeDevoluciónUsuarioActual = OperationContext.Current.GetCallbackChannel<IGestorDeSolicitudesDeAmistadCallBack>();

            if (!amistadEnLinea.ContainsKey(nombreUsuarioDeActualJugador))
            {
                amistadEnLinea.Add(nombreUsuarioDeActualJugador, canalDeDevoluciónUsuarioActual);
            }
            else
            {
                amistadEnLinea[nombreUsuarioDeActualJugador] = canalDeDevoluciónUsuarioActual;
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
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        EliminarDeDiccionarioAmistadesEnLinea(nombreUsuarioJugadorRemitente);
                    }
                    catch (TimeoutException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
            catch (ExcepcionAccesoDatos ex)
            {
                HuntersTrophyExcepcion respuestaExcepcion = new HuntersTrophyExcepcion
                {
                    Mensaje = ex.Message,
                    StackTrace = ex.StackTrace
                };
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new FaultException<HuntersTrophyExcepcion>(respuestaExcepcion, new FaultReason(respuestaExcepcion.Mensaje));
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
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
            catch (ExcepcionAccesoDatos ex)
            {
                HuntersTrophyExcepcion respuestaExcepcion = new HuntersTrophyExcepcion
                {
                    Mensaje = ex.Message,
                    StackTrace = ex.StackTrace
                };
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new FaultException<HuntersTrophyExcepcion>(respuestaExcepcion, new FaultReason(respuestaExcepcion.Mensaje));
            }
        }
        
        public void EliminarAmigo(int idJugadorActual, string nombreUsuarioDeActualJugador, string nombreUsuarioAmigoEliminado)
        {
            lock (objetoDeBloqueo)
            {
                ImplementacionServicio usuarioAccesoDatos = new ImplementacionServicio();
                AmistadDao SolicitudAmistadAccesoDatos = new AmistadDao();

                try
                {
                    int idJugadorAmigo = usuarioAccesoDatos.ObtenerIdJugadorPorNombreUsuario(nombreUsuarioAmigoEliminado);
                    int filasAfectadas = SolicitudAmistadAccesoDatos.BorrarAmistad(idJugadorActual, idJugadorAmigo);

                    if (filasAfectadas > 0)
                    {
                        InformarAmigoEliminado(nombreUsuarioDeActualJugador, nombreUsuarioAmigoEliminado);
                        InformarAmigoEliminado(nombreUsuarioAmigoEliminado, nombreUsuarioDeActualJugador);
                    }
                }
                catch (ExcepcionAccesoDatos ex)
                {
                    HuntersTrophyExcepcion respuestaExcepcion = new HuntersTrophyExcepcion
                    {
                        Mensaje = ex.Message,
                        StackTrace = ex.StackTrace
                    };
                    throw new FaultException<HuntersTrophyExcepcion>(respuestaExcepcion, new FaultReason(respuestaExcepcion.Mensaje));
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
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
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
