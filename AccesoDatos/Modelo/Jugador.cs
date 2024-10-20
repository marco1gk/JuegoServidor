using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Modelo
{
    public class Jugador
    {
        [Key]
        public int JugadorId { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(50)]
        public string NombreUsuario { get; set; }
        public int NumeroFotoPerfil { get; set; } = 1;
        public Cuenta Cuenta { get; set; }  



    }
}
