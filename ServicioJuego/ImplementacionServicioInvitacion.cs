using ServicioJuego.Correo;
using ServicioJuego.Correo.Plantillas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego
{
    public partial class ImplementacionServicio : IGestorInvitacion
    {
        public bool EnviarInvitacionCorreo(string codigoSalaEspera, string correo)
        {
            bool enviado;
            try
            {
                EnviadorCorreos enviadorCorreos = new EnviadorCorreos(new PlantillaInvitacionSalaEspera());
                 enviado = enviadorCorreos.EnviarCorreo(correo, codigoSalaEspera);

                if (enviado)
                {
                    Console.WriteLine("Correo enviado exitosamente a " + correo);
                }
                else
                {
                    Console.WriteLine("Error al enviar el correo a " + correo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción no controlada: {ex.Message}");
                return false;
            }
            return enviado;
        }


    }
}




