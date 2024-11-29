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
                    Console.WriteLine("Correo enviado exitosamente a " + correo);
                    return true;
                }
                else
                {
                    Console.WriteLine("Error al enviar el correo a " + correo);
                    return false;
                }
            }
            catch (FormatException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                Console.WriteLine($"Formato de correo inválido: {ex.Message}");
            }
            catch (SmtpException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                Console.WriteLine($"Error de SMTP: {ex.Message}");
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                Console.WriteLine($"Excepción no controlada: {ex.Message}");
            }
            return false;
        }



    }
}




