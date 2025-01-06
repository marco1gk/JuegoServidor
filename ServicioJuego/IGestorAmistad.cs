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
        
        List<string> ObtenerListaNombresUsuariosAmigos(int idJugador);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]

        bool ValidarEnvioSolicitudAmistad(int idJugadorEnviador, string nombreUsuarioSolicitado);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]

        int AgregarSolicitudAmistad(int idJugadorEnvia, string nombreUsuarioSolicitado);

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        List<string> ObtenerNombresUsuariosSolicitantes(int idJugador);
    }
}
