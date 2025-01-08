using AccesoDatos.DAO;
using AccesoDatos.Modelo;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pruebas
{
    public class PruebaRecursos
    {
       
        [Fact]
        public void EnviarCodigoConfirmacion_DebeRetornarCodigo_CuandoCorreoEsValido()
        {
            var recursos = new Recursos();
            string correoValido = "axel.lu04@gmail.com";

            string codigo = recursos.EnviarCodigoConfirmacion(correoValido);

            Assert.NotNull(codigo);
            Assert.Equal(6, codigo.Length);
            Assert.True(int.TryParse(codigo, out _), "El código no es un número válido.");
        }

        [Fact]
        public void EnviarCodigoConfirmacion_DebeRetornarNull_CuandoCorreoEsInvalido()
        {
            // Arrange
            var recursos = new Recursos();
            string correoInvalido = "correo_invalido";

            // Act
            string codigo = recursos.EnviarCodigoConfirmacion(correoInvalido);

            // Assert
            Assert.Null(codigo);
        }

        [Fact]
        public void ValidarCodigo_DebeRetornarTrue_CuandoCodigosCoinciden()
        {
            var recursos = new Recursos();
            string codigoEnviado = "123456";
            string codigoIngresado = "123456";

            bool esValido = recursos.ValidarCodigo(codigoIngresado, codigoEnviado);

            Assert.True(esValido, "Los códigos deberían coincidir.");
        }

        [Fact]
        public void ValidarCodigo_DebeRetornarFalse_CuandoCodigosNoCoinciden()
        {
            var recursos = new Recursos();
            string codigoEnviado = "123456";
            string codigoIngresado = "654321";

            bool esValido = recursos.ValidarCodigo(codigoIngresado, codigoEnviado);

            Assert.False(esValido, "Los códigos no deberían coincidir.");
        }

        [Fact]
        public void GenerarSalt_DebeRetornarSalt_DeLongitudEspecificada()
        {
            int tamanoSalt = 16;

            string salt = Recursos.GenerarSalt(tamanoSalt);

            Assert.NotNull(salt);
            Assert.Equal(tamanoSalt, Convert.FromBase64String(salt).Length);
        }

        [Fact]
        public void HashearContrasena_DebeRetornarHash_CuandoSeProporcionaSalt()
        {
            string contrasena = "Password123";
            string salt = Recursos.GenerarSalt();

            string hash = Recursos.HashearContrasena(contrasena, salt);

            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
        }

        [Fact]
        public void VerificarContrasena_DebeRetornarTrue_CuandoContrasenaEsCorrecta()
        {
            string contrasena = "Password123";
            string salt = Recursos.GenerarSalt();
            string hash = Recursos.HashearContrasena(contrasena, salt);

            var cuenta = new Cuenta
            {
                ContraseniaHash = hash,
                Salt = salt
            };

            bool esValida = Recursos.VerificarContrasena(contrasena, cuenta);

            Assert.True(esValida, "La contraseña debería ser válida.");
        }

        [Fact]
        public void VerificarContrasena_DebeRetornarFalse_CuandoContrasenaEsIncorrecta()
        {
            string contrasenaCorrecta = "Password123";
            string contrasenaIncorrecta = "WrongPassword";
            string salt = Recursos.GenerarSalt();
            string hash = Recursos.HashearContrasena(contrasenaCorrecta, salt);

            var cuenta = new Cuenta
            {
                ContraseniaHash = hash,
                Salt = salt
            };

            bool esValida = Recursos.VerificarContrasena(contrasenaIncorrecta, cuenta);

            Assert.False(esValida, "La contraseña no debería ser válida.");
        }
    }
}




