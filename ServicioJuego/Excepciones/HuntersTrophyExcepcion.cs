using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego.Excepciones
{
    [DataContract]
    public class HuntersTrophyExcepcion
    {

        [DataMember]
        public string Mensaje {  get; set; }

        [DataMember]
        public string StackTrace{ get; set; }

    }
}
