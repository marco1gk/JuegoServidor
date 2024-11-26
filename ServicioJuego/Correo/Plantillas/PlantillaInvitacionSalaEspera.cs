using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego.Correo.Plantillas
{
    public class PlantillaInvitacionSalaEspera : IPlantillaCorreo
    {
        public string RealizarCorreo(string mensaje)
        {
            string contenidoCorreo = "Unete al juego! El código del lobby es: " + mensaje;
            return contenidoCorreo;
        }

    }
}
