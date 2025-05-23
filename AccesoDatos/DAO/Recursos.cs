﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AccesoDatos.Modelo;
using AccesoDatos.Utilidades;

namespace AccesoDatos.DAO
{
    public class Recursos
    {
        public string EnviarCodigoConfirmacion(string correo)
        {
            string codigo = new Random().Next(100000, 999999).ToString();

            MailMessage mail = new MailMessage
            {
                From = new MailAddress("hunterstrophy01@gmail.com")
            };
            mail.To.Add(correo);
            mail.Subject = "Código de Verificación";
            mail.Body = "Tu código de verificación es: " + codigo;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new System.Net.NetworkCredential("hunterstrophy01@gmail.com", "azcx qzqh kzdq ifve"),
                EnableSsl = true
            };

            try
            {
                smtpClient.Send(mail);
            }
            catch (SmtpException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                return null;
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                return null;
            }

            return codigo;
        }

        public bool ValidarCodigo(string codigoIngresado, string codigoEnviado)
        {
            return codigoIngresado == codigoEnviado;
        }

        public static string GenerarSalt(int tamano = 16)
        {
            var saltBytes = new byte[tamano];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashearContrasena(string contrasena, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var contrasenaConSalt = contrasena + salt;
                byte[] contrasenaBytes = Encoding.UTF8.GetBytes(contrasenaConSalt);
                byte[] hashBytes = sha256.ComputeHash(contrasenaBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerificarContrasena(string contrasena, Cuenta cuenta)
        {
            var hashContrasena = HashearContrasena(contrasena, cuenta.Salt);
            if (hashContrasena == cuenta.ContraseniaHash)
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
