using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Numerics;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Security.Principal;
using AccesoDatos.Modelo;
using AccesoDatos.Utilidades;
using AccesoDatos.Excepciones;



namespace AccesoDatos.DAO
{
    public class AmistadDao
    {
        private const string ESTADO_AMISTAD = "Amigo";
        private const string ESTADO_SOLICITUD = "Solicitud";

       
        public AmistadDao()
        {
  
        }

        public bool VerificarAmistad(int idJugadorMandaSolicitud, int idJugadorReciveSolicitud)
        {
            bool tieneRelacion = false;

            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var amistad = (from fs in contexto.Amistades
                                      where
                                          (fs.JugadorId == idJugadorMandaSolicitud && fs.AmigoId == idJugadorReciveSolicitud)
                                          || (fs.JugadorId == idJugadorReciveSolicitud && fs.AmigoId == idJugadorMandaSolicitud)
                                          && (fs.EstadoAmistad.Equals(ESTADO_AMISTAD) || fs.EstadoAmistad.Equals(ESTADO_SOLICITUD))
                                      select fs).ToList();

                    tieneRelacion = amistad.Any();
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

            return tieneRelacion;
        }

        public int AgregarSolicitudAmistad(int idJugadorMandaSolicitud, int idJugadorReciveSolicitud)
        {
            int filasAfectadas = -1;


            if (idJugadorMandaSolicitud > 0 && idJugadorReciveSolicitud > 0)
            {
                Amistad amistad = new Amistad();
                amistad.JugadorId = idJugadorMandaSolicitud;
                amistad.AmigoId = idJugadorReciveSolicitud;
                amistad.EstadoAmistad = ESTADO_SOLICITUD;

                try
                {
                    using (var contexto = new ContextoBaseDatos())
                    {
                        contexto.Amistades.Add(amistad);
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

            return filasAfectadas;
        }

        public bool EsAmigo(int idJugador, int idJugadorAmigo)
        {
            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var amistad = (from fs in contexto.Amistades
                                      where
                                          ((fs.JugadorId == idJugador && fs.AmigoId == idJugadorAmigo)
                                          || (fs.JugadorId == idJugadorAmigo && fs.AmigoId == idJugador))
                                          && (fs.EstadoAmistad.Equals(ESTADO_AMISTAD))
                                      select fs).ToList();
                    bool esAmigo = amistad.Any();
                    return esAmigo;
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

        public List<int> ObtenerIdJugadorSolicitantesAmistad(int idJugador)
        {
            List<int> idJugadores = new List<int>();

            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var solicitudesDeAmistadDelJugador = (
                        from fs in contexto.Amistades
                        where (fs.AmigoId == idJugador && fs.EstadoAmistad.Equals(ESTADO_SOLICITUD))
                        select fs.JugadorId
                    ).ToList();

                    if (solicitudesDeAmistadDelJugador.Any())
                    {
                        foreach (var solicitanteDeAmistad in solicitudesDeAmistadDelJugador)
                        {
                            idJugadores.Add((int)solicitanteDeAmistad);
                        }
                    }
                    return idJugadores;
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

        public int ActualizarSolicitudAmistad_Aceptada(int idJugadorActual, int idJugadorAceptado)
        {
            int filasAfectadas = -1;

            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var amistad = contexto.Amistades.FirstOrDefault(fs =>
                        (fs.JugadorId == idJugadorAceptado && fs.AmigoId == idJugadorActual && fs.EstadoAmistad == ESTADO_SOLICITUD)
                        || (fs.JugadorId == idJugadorActual && fs.AmigoId == idJugadorAceptado && fs.EstadoAmistad == ESTADO_SOLICITUD)
                    );

                    if (amistad != null)
                    {
                        amistad.EstadoAmistad = ESTADO_AMISTAD;
                        filasAfectadas = contexto.SaveChanges();
                    }
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

            return filasAfectadas;
        }

        public int BorrarSolicitudAmistad(int idJugadorActual, int idJugadorRechazado)
        {
            int filasAfectadas = -1;

            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var amistad = contexto.Amistades.FirstOrDefault(fs =>
                        (fs.JugadorId == idJugadorRechazado && fs.AmigoId == idJugadorActual && fs.EstadoAmistad == ESTADO_SOLICITUD)
                        || (fs.JugadorId == idJugadorActual && fs.AmigoId == idJugadorRechazado && fs.EstadoAmistad == ESTADO_SOLICITUD)
                    );

                    if (amistad != null)
                    {
                        contexto.Amistades.Remove(amistad);
                        filasAfectadas = contexto.SaveChanges();
                    }
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

            return filasAfectadas;
        }

        public int BorrarAmistad(int idJugadorActual, int idJugadorAmigo)
        {
            int filasAfectadas = -1;

            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var amistad = contexto.Amistades.FirstOrDefault(fs =>
                        (fs.JugadorId == idJugadorAmigo && fs.AmigoId == idJugadorActual && fs.EstadoAmistad == ESTADO_AMISTAD)
                        || (fs.JugadorId == idJugadorActual && fs.AmigoId == idJugadorAmigo && fs.EstadoAmistad == ESTADO_AMISTAD)
                    );

                    if (amistad != null)
                    {
                        contexto.Amistades.Remove(amistad);
                        filasAfectadas = contexto.SaveChanges();
                    }
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

            return filasAfectadas;
        }

        public List<string> ObtenerAmigos(int idJugador)
        {
            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var amigos = contexto.Amistades
                    .Where(f => (f.AmigoId == idJugador || f.JugadorId == idJugador) && f.EstadoAmistad == ESTADO_AMISTAD)
                    .SelectMany(f => new[] { f.JugadorId, f.AmigoId })
                    .Distinct()
                    .Where(id => id != idJugador)
                    .Join(contexto.Jugadores,
                          friendId => friendId,
                          player => player.JugadorId,
                          (friendId, player) => player.NombreUsuario)
                    .ToList();
                    return amigos;
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
    }
}
