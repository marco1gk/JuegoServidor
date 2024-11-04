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
        private const string STATUS_FRIEND = "Friend";
        private const string STATUS_REQUEST = "Request";

        public bool VerifyFriendship(int idPlayerSender, int idPlayerRequested)
        {
            bool hasRelation = false;

            try
            {
                using (var context = new ContextoBaseDatos())
                {
                    var fsiendship = (from fs in context.Amistades
                                      where
                                          (fs.JugadorId == idPlayerSender && fs.AmigoId == idPlayerRequested)
                                          || (fs.JugadorId == idPlayerRequested && fs.AmigoId == idPlayerSender)
                                          && (fs.EstadoAmistad.Equals(STATUS_FRIEND) || fs.EstadoAmistad.Equals(STATUS_REQUEST))
                                      select fs).ToList();

                    hasRelation = fsiendship.Any();
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

            return hasRelation;
        }

        public int AddRequestFriendship(int idPlayerSender, int idPlayerRequested)
        {
            int rowsAffected = -1;

            if (idPlayerSender > 0 && idPlayerRequested > 0)
            {
                Amistad fsiendShip = new Amistad();
                fsiendShip.JugadorId = idPlayerSender;
                fsiendShip.AmigoId = idPlayerRequested;
                fsiendShip.EstadoAmistad = STATUS_REQUEST;

                try
                {
                    using (var context = new ContextoBaseDatos())
                    {
                        context.Amistades.Add(fsiendShip);
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

        public bool IsFriend(int idPlayer, int idPlayerFriend)
        {
            try
            {
                using (var context = new ContextoBaseDatos())
                {
                    var fsiendship = (from fs in context.Amistades
                                      where
                                          ((fs.JugadorId == idPlayer && fs.AmigoId == idPlayerFriend)
                                          || (fs.JugadorId == idPlayerFriend && fs.AmigoId == idPlayer))
                                          && (fs.EstadoAmistad.Equals(STATUS_FRIEND))
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
                        where (fs.AmigoId == idPlayer && fs.EstadoAmistad.Equals(STATUS_REQUEST))
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
                        (fs.JugadorId == idPlayerAccepted && fs.AmigoId == idCurrentPlayer && fs.EstadoAmistad == STATUS_REQUEST)
                        || (fs.JugadorId == idCurrentPlayer && fs.AmigoId == idPlayerAccepted && fs.EstadoAmistad == STATUS_REQUEST)
                    );

                    if (friendship != null)
                    {
                        friendship.EstadoAmistad = STATUS_FRIEND;
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
                        (fs.JugadorId == idPlayerRejected && fs.AmigoId == idCurrentPlayer && fs.EstadoAmistad == STATUS_REQUEST)
                        || (fs.JugadorId == idCurrentPlayer && fs.AmigoId == idPlayerRejected && fs.EstadoAmistad == STATUS_REQUEST)
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
                        (fs.JugadorId == idPlayerFriend && fs.AmigoId == idCurrentPlayer && fs.EstadoAmistad == STATUS_FRIEND)
                        || (fs.JugadorId == idCurrentPlayer && fs.AmigoId == idPlayerFriend && fs.EstadoAmistad == STATUS_FRIEND)
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
                    .Where(f => (f.AmigoId == idPlayer || f.JugadorId == idPlayer) && f.EstadoAmistad == STATUS_FRIEND)
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
