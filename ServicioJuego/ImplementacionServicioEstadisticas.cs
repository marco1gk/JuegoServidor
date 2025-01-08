using AccesoDatos.DAO;
using AccesoDatos.Excepciones;
using AccesoDatos.Modelo;
using AccesoDatos.Utilidades;
using ServicioJuego.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego
{
    public partial class ImplementacionServicio : IEstadisticasGlobales
    {

        public List<Estadisticas> ObtenerEstadisticasGlobales()
        {
            EstadisticasDao accesoDatos = new EstadisticasDao();
            List<Estadisticas> estadisiticasGlobales = new List<Estadisticas>();

            try
            {
                List<Estadisticas> puntuaciones = accesoDatos.ObtenerEstadisticasGlobales();

                foreach (Estadisticas puntuacion in puntuaciones)
                {
                    Estadisticas estadisticasGlobales = new Estadisticas
                    {
                        IdEstadisticas = puntuacion.IdEstadisticas,
                        IdJugador = puntuacion.IdJugador,
                        NumeroVictorias = puntuacion.NumeroVictorias
                    };

                    estadisiticasGlobales.Add(estadisticasGlobales);
                }
            }
            catch (ExcepcionAccesoDatos ex)
            {
                HuntersTrophyExcepcion respuestaaExcepcion = new HuntersTrophyExcepcion
                {
                    Mensaje = ex.Message,
                    StackTrace = ex.StackTrace
                };
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new FaultException<HuntersTrophyExcepcion>(respuestaaExcepcion, new FaultReason(respuestaaExcepcion.Mensaje));
            }

            return estadisiticasGlobales;
        }


        public int ActualizarVictorias(int idJugador)
        {
            EstadisticasDao AccesoDatos = new EstadisticasDao();
            try
            {
                int respuesta = AccesoDatos.ActualizarVictoriasJugador(idJugador);

                return respuesta;
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

    }
}
