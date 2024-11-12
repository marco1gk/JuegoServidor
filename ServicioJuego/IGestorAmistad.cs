using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego
{
    [ServiceContract]
    public interface IGestorAmistad
    {
        [OperationContract]
        
        List<string> ObtenerListaNombresUsuariosAmigos(int idPlayer);

        [OperationContract]
       
        bool ValidarEnvioSolicitudAmistad(int idPlayerSender, string usernamePlayerRequested);

        [OperationContract]

        int AgregarSolicitudAmistad(int idPlayerSender, string usernamePlayerRequested);

        [OperationContract]
        List<string> ObtenerNombresUsuariosSolicitantes(int idPlayer);
    }
}
