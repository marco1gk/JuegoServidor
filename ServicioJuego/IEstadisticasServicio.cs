using AccesoDatos.Modelo;
using ServicioJuego.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego
{
    [ServiceContract]
    public interface IEstadisticasGlobales
    {
        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        List<Estadistica> ObtenerEstadisticasGlobales();

        [OperationContract]
        [FaultContract(typeof(HuntersTrophyExcepcion))]
        int ActualizarVictorias(int idJugador);
    }
    [DataContract]
    public class EstadisticasDataContract
    {
        [DataMember]
        public int IdJugador { get; set; }

        [DataMember]
        public int NumeroVictorias { get; set; }

        [DataMember]
        public int IdEstadisticas { get; set; }
    }


}
