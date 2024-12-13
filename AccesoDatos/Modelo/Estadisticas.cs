using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Modelo
{
    public class Estadisticas
    {
        [Key]
        public int IdEstadisticas { get; set; }
        
        [ForeignKey("Jugador")]
        public int IdJugador { get; set; }

        public int NumeroVictorias { get; set; }

        public virtual Jugador Jugador { get; set; }  
    }
}
