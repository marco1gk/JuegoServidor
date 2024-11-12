using AccesoDatos.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego
{
    public partial class ImplementacionServicio : IGestorAmistad
    {
        public List<string> ObtenerListaNombresUsuariosAmigos(int idJugador)
        {
            AmistadDao AccesoDatos = new AmistadDao();

            try
            {
                return AccesoDatos.ObtenerAmigos(idJugador);
            }
            catch (Exception ex)
            {
                throw new FaultException<ExcepcionServicio>(
        new ExcepcionServicio(ex.Message, ex),
        new FaultReason(ex.Message)
    );
            }
        }

        public bool ValidarEnvioSolicitudAmistad(int idPlayerEnvia, string nombreJugadorSolicitado)
        {
            bool SolicitudAmistadValida = false;
            int idJugadorSolicitado = 0;
            bool tieneRelacion = false;
            ImplementacionServicio usuarioAccesoDatos = new ImplementacionServicio();
            AmistadDao AccesoDatosSolicitudAmistad = new AmistadDao();

            try
            {
                idJugadorSolicitado = usuarioAccesoDatos.ObtenerIdJugadorPorNombreUsuario(nombreJugadorSolicitado);
            }
            catch (Exception ex)
            {
                throw new FaultException<ExcepcionServicio>(
        new ExcepcionServicio(ex.Message, ex),
        new FaultReason(ex.Message)
    );
            }

            if (idJugadorSolicitado < 1)
            {
                return false;
            }

            if (idPlayerEnvia == idJugadorSolicitado)
            {
                return false;
            }

            try
            {
                tieneRelacion = AccesoDatosSolicitudAmistad.VerificarAmistad(idPlayerEnvia, idJugadorSolicitado);

            }
            catch (Exception ex)
            {
                throw new FaultException<ExcepcionServicio>(
         new ExcepcionServicio(ex.Message, ex),
         new FaultReason(ex.Message)
     );
            }

            if (!tieneRelacion)
            {
                SolicitudAmistadValida = true;
            }

            return SolicitudAmistadValida;
        }

        public int AgregarSolicitudAmistad(int idJugadorEnvia, string nombreJugadorSolicitado)
        {
            int columasAfectadas = -1;
            ImplementacionServicio usuarioAccesoDatos = new ImplementacionServicio();
            AmistadDao SolicitudAmistadAccesoDatos = new AmistadDao();
            try
            {
                int idPlayerRequested = usuarioAccesoDatos.ObtenerIdJugadorPorNombreUsuario(nombreJugadorSolicitado);

                if (idPlayerRequested > 0)
                {
                    columasAfectadas = SolicitudAmistadAccesoDatos.AgregarSolicitudAmistad(idJugadorEnvia, idPlayerRequested);
                }

                return columasAfectadas;
            }
            catch (Exception ex)
            {
                throw new FaultException<ExcepcionServicio>(
           new ExcepcionServicio(ex.Message, ex),
           new FaultReason(ex.Message)
       );
            }
        }

        public List<string> ObtenerNombresUsuariosSolicitantes(int idJugador)
        {
            List<string> nombreJugadores = new List<string>();
            AmistadDao solicitudAmistadAccesoDatos = new AmistadDao();
            ImplementacionServicio usuarioAccesoDatos = new ImplementacionServicio();

            try
            {
                List<int> playersRequestersId = solicitudAmistadAccesoDatos.ObtenerIdJugadorSolicitantesAmistad(idJugador);

                if (playersRequestersId != null)
                {
                    foreach (int idRequester in playersRequestersId)
                    {
                        nombreJugadores.Add(usuarioAccesoDatos.ObtenerNombreUsuarioPorIdJugador(idRequester));
                    }
                }

                return nombreJugadores;
            }
            catch (Exception ex)
            {
                throw new FaultException<ExcepcionServicio>(
                     new ExcepcionServicio(ex.Message, ex),
                     new FaultReason(ex.Message)
                 );
            }
        }
    }
    public class ExcepcionServicio : Exception
    {
        public ExcepcionServicio(string mensaje) : base(mensaje) { }
        public ExcepcionServicio(string message, Exception inner) : base(message, inner) { }
    }
}
