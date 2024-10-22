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
            mail.From = new MailAddress("axel.lu04@gmail.com"); // Cambia por tu correo
            mail.To.Add(correo);
            mail.Subject = "Código de Verificación";
            mail.Body = "Tu código de verificación es: " + codigo;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587); // Cambia por tu servidor SMTP
            smtpClient.Credentials = new System.Net.NetworkCredential("axel.lu04@gmail.com", "159753 cr"); // Cambia por tus credenciaes
            smtpClient.EnableSsl = true;

            smtpClient.Send(mail);

            return codigo; // Devuelve el código para almacenarlo
        }

        public bool ValidarCodigo(string codigoIngresado, string codigoEnviado)
        {
            if (codigoIngresado == codigoEnviado)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
