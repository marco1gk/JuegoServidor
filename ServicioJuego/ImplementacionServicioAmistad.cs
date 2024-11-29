using AccesoDatos.DAO;
using AccesoDatos.Excepciones;
using ServicioJuego.Excepciones;
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
            catch (ExcepcionAccesoDatos ex)
            {
                HuntersTrophyExcepcion respuestaExcepcion = new HuntersTrophyExcepcion
                {
                    Mensaje = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<HuntersTrophyExcepcion>(respuestaExcepcion, new FaultReason(respuestaExcepcion.Mensaje));
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
            catch (ExcepcionAccesoDatos ex)
            {
                HuntersTrophyExcepcion respuestaExcepcion = new HuntersTrophyExcepcion
                {
                    Mensaje = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw new FaultException<HuntersTrophyExcepcion>(respuestaExcepcion, new FaultReason(respuestaExcepcion.Mensaje));
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
                int idJugadorSolicitado = usuarioAccesoDatos.ObtenerIdJugadorPorNombreUsuario(nombreJugadorSolicitado);

                if (idJugadorSolicitado > 0)
                {
                    columasAfectadas = SolicitudAmistadAccesoDatos.AgregarSolicitudAmistad(idJugadorEnvia, idJugadorSolicitado);
                }

                return columasAfectadas;
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

        public List<string> ObtenerNombresUsuariosSolicitantes(int idJugador)
        {
            List<string> nombreJugadores = new List<string>();
            AmistadDao solicitudAmistadAccesoDatos = new AmistadDao();
            ImplementacionServicio usuarioAccesoDatos = new ImplementacionServicio();

            try
            {
                List<int> idJugadoresSolicitantes = solicitudAmistadAccesoDatos.ObtenerIdJugadorSolicitantesAmistad(idJugador);

                if (idJugadoresSolicitantes != null)
                {
                    foreach (int idSolicitante in idJugadoresSolicitantes)
                    {
                        nombreJugadores.Add(usuarioAccesoDatos.ObtenerNombreUsuarioPorIdJugador(idSolicitante));
                    }
                }

                return nombreJugadores;
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

}
