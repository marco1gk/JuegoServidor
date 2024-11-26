using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego.Correo.Plantillas
{
    public interface IPlantillaCorreo
    {

        string RealizarCorreo(string mensaje);
    }
}
