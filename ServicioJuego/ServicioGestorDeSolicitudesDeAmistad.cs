using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using AccesoDatos;
using System.Threading.Tasks;
using AccesoDatos.DAO;

namespace ServicioJuego
{
    public partial class ImplementacionServicio :  IGestorDeSolicitudesDeAmistad
    {

        private static Dictionary<string, IGestorDeSolicitudesDeAmistadCallBack> onlineFriendship = new Dictionary<string, IGestorDeSolicitudesDeAmistadCallBack>();


        public void AddToOnlineFriendshipDictionary(string usernameCurrentPlayer)
        {
            IGestorDeSolicitudesDeAmistadCallBack currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IGestorDeSolicitudesDeAmistadCallBack>();

            if (!onlineFriendship.ContainsKey(usernameCurrentPlayer))
            {
                onlineFriendship.Add(usernameCurrentPlayer, currentUserCallbackChannel);
            }
            else
            {
                onlineFriendship[usernameCurrentPlayer] = currentUserCallbackChannel;
            }
        }



        public void SendFriendRequest(string usernamePlayerSender, string usernamePlayerRequested)
        {
            lock (lockObject)
            {
                if (onlineFriendship.ContainsKey(usernamePlayerRequested))
                {
                    try
                    {
                        onlineFriendship[usernamePlayerRequested].NotifyNewFriendRequest(usernamePlayerSender);
                    }
                    catch (CommunicationException ex)
                    {
                        Console.WriteLine(ex);
                        RemoveFromOnlineFriendshipDictionary(usernamePlayerSender);
                    }
                    catch (TimeoutException ex)
                    {
                        Console.WriteLine(ex);
                        RemoveFromOnlineFriendshipDictionary(usernamePlayerSender);
                    }
                }
            }
        }

        public void AcceptFriendRequest(int idPlayerRequested, string usernamePlayerRequested, string usernamePlayerSender)
        {
            ImplementacionServicio userDataAccess = new ImplementacionServicio();
            AmistadDao friendRequestDataAccess = new AmistadDao();

            try
            {
                int idPlayerSender = userDataAccess.GetIdPlayerByUsername(usernamePlayerSender);
                int rowsAffected = friendRequestDataAccess.UpdateFriendRequestToAccepted(idPlayerRequested, idPlayerSender);

                if (rowsAffected > 0)
                {
                    InformFriendRequestAccepted(usernamePlayerSender, usernamePlayerRequested);
                    InformFriendRequestAccepted(usernamePlayerRequested, usernamePlayerSender);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                
            }
        }

        private void InformFriendRequestAccepted(string usernameTarget, string usernameNewFriend)
        {
            if (onlineFriendship.ContainsKey(usernameTarget))
            {
                try
                {
                    onlineFriendship[usernameTarget].NotifyFriendRequestAccepted(usernameNewFriend);
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine(ex);
                    RemoveFromOnlineFriendshipDictionary(usernameTarget);
                }
            }
        }

        public void RejectFriendRequest(int idCurrentPlayer, string username)
        {
            ImplementacionServicio userDataAccess = new ImplementacionServicio();
            AmistadDao friendRequestDataAccess = new AmistadDao();

            try
            {
                int idPlayerAccepted = userDataAccess.GetIdPlayerByUsername(username);

                friendRequestDataAccess.DeleteFriendRequest(idCurrentPlayer, idPlayerAccepted);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void DeleteFriend(int idCurrentPlayer, string usernameCurrentPlayer, string usernameFriendDeleted)
        {
            lock (lockObject)
            {
                ImplementacionServicio userDataAccess = new ImplementacionServicio();
                AmistadDao friendRequestDataAccess = new AmistadDao();

                try
                {
                    int idPlayerFriend = userDataAccess.GetIdPlayerByUsername(usernameFriendDeleted);
                    int rowsAffected = friendRequestDataAccess.DeleteFriendship(idCurrentPlayer, idPlayerFriend);

                    if (rowsAffected > 0)
                    {
                        InformFriendDeleted(usernameCurrentPlayer, usernameFriendDeleted);
                        InformFriendDeleted(usernameFriendDeleted, usernameCurrentPlayer);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void InformFriendDeleted(string usernameTarget, string usernameDeletedFriend)
        {
            if (onlineFriendship.ContainsKey(usernameTarget))
            {
                try
                {
                    onlineFriendship[usernameTarget].NotifyDeletedFriend(usernameDeletedFriend);
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine(ex);
                    RemoveFromOnlineFriendshipDictionary(usernameTarget);
                }
            }
        }

        public void RemoveFromOnlineFriendshipDictionary(string username)
        {
            onlineFriendship.Remove(username);
        }


    }
}
