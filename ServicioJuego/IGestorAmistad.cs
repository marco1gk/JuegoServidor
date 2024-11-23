using ServicioJuego.Excepciones;
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
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        
        List<string> ObtenerListaNombresUsuariosAmigos(int idPlayer);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]

        bool ValidarEnvioSolicitudAmistad(int idPlayerSender, string usernamePlayerRequested);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]

        int AgregarSolicitudAmistad(int idPlayerSender, string usernamePlayerRequested);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        List<string> ObtenerNombresUsuariosSolicitantes(int idPlayer);
    }
}
