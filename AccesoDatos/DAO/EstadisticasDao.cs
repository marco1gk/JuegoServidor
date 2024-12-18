using AccesoDatos.Excepciones;
using AccesoDatos.Modelo;
using AccesoDatos.Utilidades;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.DAO
{
    public class EstadisticasDao
    {
        public List<Estadisticas> ObtenerEstadisticasGlobales()
        {
            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    List<Estadisticas> estadisticas = (from estadisticasGlobales in contexto.Estadisticas
                                                       orderby estadisticasGlobales.NumeroVictorias descending
                                                       select estadisticasGlobales).ToList();

                    return estadisticas;
                }
            }
            catch (EntityException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new ExcepcionAccesoDatos(ex.Message);
            }
            catch (SqlException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new ExcepcionAccesoDatos(ex.Message);
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(ex);
                throw new ExcepcionAccesoDatos(ex.Message);
            }
        }

        public int ActualizarVictoriasJugador(int idJugador)
        {
            int filasAfectadas = -1;

            if (idJugador > 0)
            {
                try
                {
                    using (var contexto = new ContextoBaseDatos())
                    {
                        var estadisticasGlobalesJugador = contexto.Estadisticas
                            .FirstOrDefault(e => e.IdJugador == idJugador);

                        if (estadisticasGlobalesJugador != null)
                        {
                            estadisticasGlobalesJugador.NumeroVictorias += 1;
                     
                        }
                        else
                        {
                            estadisticasGlobalesJugador = new Estadisticas
                            {
                                IdJugador = idJugador,
                                NumeroVictorias = 1 
                            };
                            contexto.Estadisticas.Add(estadisticasGlobalesJugador);
                           
                        }

                        filasAfectadas = contexto.SaveChanges();
                    }
                }
                catch (EntityException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    throw new ExcepcionAccesoDatos(ex.Message);
                }
                catch (SqlException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    throw new ExcepcionAccesoDatos(ex.Message);
                }
                catch (Exception ex)
                {
                    ManejadorExcepciones.ManejarFatalExcepcion(ex);
                    throw new ExcepcionAccesoDatos(ex.Message);
                }
            }
            else
            {
                Console.WriteLine($"El IdJugador no es válido: {idJugador}");
            }

            return filasAfectadas;
        }


    }
}
