using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Modelo
{
    public class Cuenta
    {

        [Key, ForeignKey("Jugador")]
        public int JugadorId { get; set; }

        [Required]
        [Index(IsUnique =true)]
        [MaxLength(100)]
        public string Correo { get; set; }

        [Required]
        public string ContraseniaHash   { get; set; }
        
        [Required]
        public string Salt { get; set; }

        public virtual Jugador Jugador { get; set; } 

    }
}
