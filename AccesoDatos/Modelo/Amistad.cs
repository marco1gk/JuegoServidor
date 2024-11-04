﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Modelo
{
    public class Amistad
    {

        [Key]
        public int AmistadId { get; set; }
         public Nullable<int> JugadorId { get; set; }
    
        public Nullable<int> AmigoId { get; set; }

        public int ImagenAmigoId { get; set; }

        public bool EnLinea { get; set; }

        public string EstadoAmistad { get; set; }


        public virtual Jugador Jugador { get; set; }
        public virtual Jugador JugadorAmigo { get; set; }
    }
}


//public partial class FriendShips
//{

//    public int idFriendship { get; set; }

//    public string statusFriendship { get; set; }

//    public Nullable<int> idPlayer { get; set; }

//    public Nullable<int> idPlayerFriend { get; set; }



//    public virtual Players Players { get; set; }

//    public virtual Players Players1 { get; set; }

//}

//}