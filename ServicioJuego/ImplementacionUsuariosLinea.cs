using AccesoDatos.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using AccesoDatos;


namespace ServicioJuego
{
   // [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class ImplementacionServicio : IGestorUsuariosConectados 
    {
        private static readonly object lockObject = new object();
        private static Dictionary<string, IGestorUsuarioCallback> onlineUsers = new Dictionary<string, IGestorUsuarioCallback>();

        public void RegisterUserToOnlineUsers(int idPlayer, string username)
        {
            IGestorUsuarioCallback currentUserCallbackChannel = OperationContext.Current.GetCallbackChannel<IGestorUsuarioCallback>();
            List<string> onlineUsernames = onlineUsers.Keys.ToList();
            List<string> onlineFriends = new List<string>();

            if (!onlineUsers.ContainsKey(username))
            {
                onlineUsers.Add(username, currentUserCallbackChannel);
            }
            else
            {
                onlineUsers[username] = currentUserCallbackChannel;
            }

            onlineFriends = onlineUsernames
                .Where(onlineUsername => IsFriend(idPlayer, onlineUsername))
                .ToList();

            try
            {
                currentUserCallbackChannel.NotifyOnlineFriends(onlineFriends);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex);
            }

            NotifyUserLoggedInToFriends(idPlayer, username);
        }

        private void NotifyUserLoggedInToFriends(int idPlayer, string username)
        {
            foreach (var user in onlineUsers.ToList())
            {
                if (user.Key != username && IsFriend(idPlayer, user.Key))
                {
                    try
                    {
                        user.Value.NotifyUserLoggedIn(username);
                    }
                    catch (CommunicationException ex)
                    {
                        Console.WriteLine(ex);
                        UnregisterUserToOnlineUsers(username);
                    }
                    catch (TimeoutException ex)
                    {
                        Console.WriteLine(ex);
                        UnregisterUserToOnlineUsers(username);
                    }
                }
            }
        }

        private bool IsFriend(int currentIdPlayer, string onlineUsername)
        {
            AmistadDao friendRequestDataAccess = new AmistadDao();
            ImplementacionServicio userDataAccess = new ImplementacionServicio();
            int idOnlinePlayer = userDataAccess.GetIdPlayerByUsername(onlineUsername);

            bool isFriend = friendRequestDataAccess.EsAmigo(currentIdPlayer, idOnlinePlayer);

            return isFriend;
        }

        public void UnregisterUserToOnlineUsers(string username)
        {
            if (onlineUsers.ContainsKey(username))
            {
                onlineUsers.Remove(username);
                onlineFriendship.Remove(username);

                foreach (var user in onlineUsers.ToList())
                {
                    try
                    {
                        user.Value.NotifyUserLoggedOut(username);
                    }
                    catch (CommunicationException ex)
                    {
                        Console.WriteLine(ex);
                        UnregisterUserToOnlineUsers(username);
                    }
                    catch (TimeoutException ex)
                    {
                        Console.WriteLine(ex);
                        UnregisterUserToOnlineUsers(username);
                    }
                }
            }
        }


    }
}
