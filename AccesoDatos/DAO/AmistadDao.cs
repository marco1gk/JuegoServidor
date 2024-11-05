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


namespace AccesoDatos.DAO
{
    public class AmistadDao
    {
        private const string ESTADO_AMISTAD = "Friend";
        private const string ESTADO_SOLICITUD = "Request";

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
                throw new Exception(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return tieneRelacion;
        }

        public int AgregarSolicitudAmistad(int idJugadorMandaSolicitud, int idJugadorReciveSolicitud)
        {
            int rowsAffected = -1;

            if (idJugadorMandaSolicitud > 0 && idJugadorReciveSolicitud > 0)
            {
                Amistad amistad = new Amistad();
                amistad.JugadorId = idJugadorMandaSolicitud;
                amistad.AmigoId = idJugadorReciveSolicitud;
                amistad.EstadoAmistad = ESTADO_SOLICITUD;

                try
                {
                    using (var context = new ContextoBaseDatos())
                    {
                        context.Amistades.Add(amistad);
                        rowsAffected = context.SaveChanges();
                    }
                }
                catch (EntityException ex)
                {
                    throw new Exception(ex.Message);
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return rowsAffected;
        }

        public bool EsAmigo(int idPlayer, int idPlayerFriend)
        {
            try
            {
                using (var context = new ContextoBaseDatos())
                {
                    var fsiendship = (from fs in context.Amistades
                                      where
                                          ((fs.JugadorId == idPlayer && fs.AmigoId == idPlayerFriend)
                                          || (fs.JugadorId == idPlayerFriend && fs.AmigoId == idPlayer))
                                          && (fs.EstadoAmistad.Equals(ESTADO_AMISTAD))
                                      select fs).ToList();
                    bool isFriend = fsiendship.Any();
                    return isFriend;
                }
            }
            catch (EntityException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<int> GetPlayerIdOfFriendRequesters(int idPlayer)
        {
            List<int> playersId = new List<int>();

            try
            {
                using (var context = new ContextoBaseDatos())
                {
                    var playerFriendRequests = (
                        from fs in context.Amistades
                        where (fs.AmigoId == idPlayer && fs.EstadoAmistad.Equals(ESTADO_SOLICITUD))
                        select fs.JugadorId
                    ).ToList();

                    if (playerFriendRequests.Any())
                    {
                        foreach (var friendRequester in playerFriendRequests)
                        {
                            playersId.Add((int)friendRequester);
                        }
                    }
                    return playersId;
                }
            }
            catch (EntityException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int UpdateFriendRequestToAccepted(int idCurrentPlayer, int idPlayerAccepted)
        {
            int rowsAffected = -1;

            try
            {
                using (var context = new ContextoBaseDatos())
                {
                    var friendship = context.Amistades.FirstOrDefault(fs =>
                        (fs.JugadorId == idPlayerAccepted && fs.AmigoId == idCurrentPlayer && fs.EstadoAmistad == ESTADO_SOLICITUD)
                        || (fs.JugadorId == idCurrentPlayer && fs.AmigoId == idPlayerAccepted && fs.EstadoAmistad == ESTADO_SOLICITUD)
                    );

                    if (friendship != null)
                    {
                        friendship.EstadoAmistad = ESTADO_AMISTAD;
                        rowsAffected = context.SaveChanges();
                    }
                }
            }
            catch (EntityException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rowsAffected;
        }

        public int DeleteFriendRequest(int idCurrentPlayer, int idPlayerRejected)
        {
            int rowsAffected = -1;

            try
            {
                using (var context = new ContextoBaseDatos())
                {
                    var friendship = context.Amistades.FirstOrDefault(fs =>
                        (fs.JugadorId == idPlayerRejected && fs.AmigoId == idCurrentPlayer && fs.EstadoAmistad == ESTADO_SOLICITUD)
                        || (fs.JugadorId == idCurrentPlayer && fs.AmigoId == idPlayerRejected && fs.EstadoAmistad == ESTADO_SOLICITUD)
                    );

                    if (friendship != null)
                    {
                        context.Amistades.Remove(friendship);
                        rowsAffected = context.SaveChanges();
                    }
                }
            }
            catch (EntityException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rowsAffected;
        }

        public int DeleteFriendship(int idCurrentPlayer, int idPlayerFriend)
        {
            int rowsAffected = -1;

            try
            {
                using (var context = new ContextoBaseDatos())
                {
                    var friendship = context.Amistades.FirstOrDefault(fs =>
                        (fs.JugadorId == idPlayerFriend && fs.AmigoId == idCurrentPlayer && fs.EstadoAmistad == ESTADO_AMISTAD)
                        || (fs.JugadorId == idCurrentPlayer && fs.AmigoId == idPlayerFriend && fs.EstadoAmistad == ESTADO_AMISTAD)
                    );

                    if (friendship != null)
                    {
                        context.Amistades.Remove(friendship);
                        rowsAffected = context.SaveChanges();
                    }
                }
            }
            catch (EntityException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return rowsAffected;
        }

        public List<string> GetFriends(int idPlayer)
        {
            try
            {
                using (var context = new ContextoBaseDatos())
                {
                    var friends = context.Amistades
                    .Where(f => (f.AmigoId == idPlayer || f.JugadorId == idPlayer) && f.EstadoAmistad == ESTADO_AMISTAD)
                    .SelectMany(f => new[] { f.JugadorId, f.AmigoId })
                    .Distinct()
                    .Where(id => id != idPlayer)
                    .Join(context.Jugadores,
                          friendId => friendId,
                          player => player.JugadorId,
                          (friendId, player) => player.NombreUsuario)
                    .ToList();
                    return friends;
                }
            }
            catch (EntityException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
