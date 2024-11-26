using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego
{
    [ServiceContract]
    public interface IGestorInvitacion
    {

        [OperationContract]
        bool EnviarInvitacionCorreo(string codigoSalaEspera, string correo);

    }
}
