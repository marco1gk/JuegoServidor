using AccesoDatos.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using AccesoDatos.Utilidades;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Data.Common;
using AccesoDatos.Excepciones;

namespace AccesoDatos.DAO
{
    public class JugadorDao
    {
        public Jugador ObtenerJugador(int idJugador)
        {
            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var jugador = contexto.Jugadores
                        .Include(j => j.Cuenta)
                        .FirstOrDefault(j => j.JugadorId == idJugador);

                    if (jugador == null)
                    {
                        throw new ExcepcionAccesoDatos ($"Jugador con ID {idJugador} no existe.");
                    }

                    return jugador;
                }
            }
            catch (DbException ex)
            {
                throw new Exception("Ocurrió un error al acceder a la base de datos.", ex);
            }
        }

        public bool EditarNombreUsuario(int idJugador, string nuevoNombreUsuario)
        {
            using (var contexto = new ContextoBaseDatos())
            {
                var jugador = contexto.Jugadores
                    .FirstOrDefault(j => j.JugadorId == idJugador);

                if (jugador != null)
                {
                    jugador.NombreUsuario = nuevoNombreUsuario;

                    try
                    {
                        int filasAlteradas = contexto.SaveChanges();
                        return filasAlteradas > 0;
                    }
                    catch (DbUpdateException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        return false;
                    }
                    catch (EntityException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        return false;
                    }
                    catch (SqlException ex)
                    {

                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        ManejadorExcepciones.ManejarFatalExcepcion(ex);
                        return false;
                    }
                }
                return false;
            }
        }
    }
}

