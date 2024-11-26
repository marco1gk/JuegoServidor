using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AccesoDatos.Utilidades;
using ServicioJuego.Correo.Plantillas;

namespace ServicioJuego.Correo
{
    public class EnviadorCorreos
    {
        private readonly IPlantillaCorreo _plantillaCorreo;

        public EnviadorCorreos(IPlantillaCorreo plantillaCorreo)
        {
            _plantillaCorreo = plantillaCorreo;
        }

        public bool EnviarCorreo(string receptor, string mensaje)
        {
            string contenidoCorreo = _plantillaCorreo.RealizarCorreo(mensaje);
            string emisorCorreo = "hunterstrophy01@gmail.com";
            string nombreJuego = "Invitacion";
            string asunto = "Hunters Trophy";
            string direccionServicio = "smtp.gmail.com";
            string contraseñaAplicacion = "azcx qzqh kzdq ifve";  
            int puerto = 587;

            MailMessage mensajeCorreo = new MailMessage();
            mensajeCorreo.From = new MailAddress(emisorCorreo, nombreJuego);
            mensajeCorreo.To.Add(receptor);
            mensajeCorreo.Subject = asunto;
            mensajeCorreo.Body = contenidoCorreo;
            mensajeCorreo.IsBodyHtml = true;

            SmtpClient smtpCliente = new SmtpClient(direccionServicio, puerto);
            smtpCliente.EnableSsl = true; 
            smtpCliente.Credentials = new NetworkCredential(emisorCorreo, contraseñaAplicacion);

            try
            {
               
                smtpCliente.Send(mensajeCorreo);
                return true;
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("Error SMTP: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error general: " + ex.Message);
                return false;
            }
        }






    }
}
