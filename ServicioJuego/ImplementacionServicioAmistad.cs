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
        public List<string> GetListUsernameFriends(int idPlayer)
        {
            AmistadDao dataAccess = new AmistadDao();

            try
            {
                return dataAccess.GetFriends(idPlayer);
            }
            catch (Exception ex)
            {
                throw new FaultException<ExcepcionServicio>(
        new ExcepcionServicio(ex.Message, ex),
        new FaultReason(ex.Message)
    );
            }
        }

        public bool ValidateFriendRequestSending(int idPlayerSender, string usernamePlayerRequested)
        {
            bool isFriendRequestValid = false;
            int idPlayerRequested = 0;
            bool hasRelation = false;
            ImplementacionServicio userDataAccess = new ImplementacionServicio();
            AmistadDao friendRequestDataAccess = new AmistadDao();

            try
            {
                idPlayerRequested = userDataAccess.GetIdPlayerByUsername(usernamePlayerRequested);
            }
            catch (Exception ex)
            {
                throw new FaultException<ExcepcionServicio>(
        new ExcepcionServicio(ex.Message, ex),
        new FaultReason(ex.Message)
    );
            }

            if (idPlayerRequested < 1)
            {
                return false;
            }

            if (idPlayerSender == idPlayerRequested)
            {
                return false;
            }

            try
            {
                hasRelation = friendRequestDataAccess.VerificarAmistad(idPlayerSender, idPlayerRequested);

            }
            catch (Exception ex)
            {
                throw new FaultException<ExcepcionServicio>(
         new ExcepcionServicio(ex.Message, ex),
         new FaultReason(ex.Message)
     );
            }

            if (!hasRelation)
            {
                isFriendRequestValid = true;
            }

            return isFriendRequestValid;
        }

        public int AddRequestFriendship(int idPlayerSender, string usernamePlayerRequested)
        {
            int rowsAffected = -1;
            ImplementacionServicio userDataAccess = new ImplementacionServicio();
            AmistadDao friendRequestDataAccess = new AmistadDao();
            try
            {
                int idPlayerRequested = userDataAccess.GetIdPlayerByUsername(usernamePlayerRequested);

                if (idPlayerRequested > 0)
                {
                    rowsAffected = friendRequestDataAccess.AgregarSolicitudAmistad(idPlayerSender, idPlayerRequested);
                }

                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new FaultException<ExcepcionServicio>(
           new ExcepcionServicio(ex.Message, ex),
           new FaultReason(ex.Message)
       );
            }
        }

        public List<string> GetUsernamePlayersRequesters(int idPlayer)
        {
            List<string> usernamePlayers = new List<string>();
            AmistadDao friendRequestDataAccess = new AmistadDao();
            ImplementacionServicio userDataAccess = new ImplementacionServicio();

            try
            {
                List<int> playersRequestersId = friendRequestDataAccess.GetPlayerIdOfFriendRequesters(idPlayer);

                if (playersRequestersId != null)
                {
                    foreach (int idRequester in playersRequestersId)
                    {
                        usernamePlayers.Add(userDataAccess.GetUsernameByIdPlayer(idRequester));
                    }
                }

                return usernamePlayers;
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
        public ExcepcionServicio(string message) : base(message) { }
        public ExcepcionServicio(string message, Exception inner) : base(message, inner) { }
    }
}
