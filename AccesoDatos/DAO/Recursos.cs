using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.DAO
{
    public class Recursos
    {
        public string EnviarCodigoConfirmacion(string correo)
        {
            // Generar un código de verificación (puedes usar una lógica más compleja)
            string codigo = new Random().Next(100000, 999999).ToString();

            // Configuración del correo
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("vaomarco052@gmail.com"); // Tu correo
            mail.To.Add(correo);
            mail.Subject = "Código de Verificación";
            mail.Body = "Tu código de verificación es: " + codigo;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = new System.Net.NetworkCredential("vaomarco052@gmail.com", "varillasMarco10@"); // Tu contraseña
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mail);
                Console.WriteLine("Correo enviado exitosamente."); // Mensaje de éxito
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("Error SMTP: " + ex.Message);
                return null; // Devuelve null si hay un error
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar el correo: " + ex.Message);
                return null; // Devuelve null si hay un error
            }

            return codigo; // Devuelve el código para almacenarlo
        }

        public bool ValidarCodigo(string codigoIngresado, string codigoEnviado)
        {
            return codigoIngresado == codigoEnviado;
        }
    }
}
