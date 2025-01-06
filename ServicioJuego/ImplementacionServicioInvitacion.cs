using AccesoDatos.Utilidades;
using ServicioJuego.Correo;
using ServicioJuego.Correo.Plantillas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ServicioJuego
{
    public partial class ImplementacionServicio : IGestorInvitacion
    {
        public bool EnviarInvitacionCorreo(string codigoSalaEspera, string correo)
        {
            try
            {
                EnviadorCorreos enviadorCorreos = new EnviadorCorreos(new PlantillaInvitacionSalaEspera());
                bool enviado = enviadorCorreos.EnviarCorreo(correo, codigoSalaEspera);

                if (enviado)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (FormatException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
            }
            catch (SmtpException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
            }
            return false;
        }

    }
}




