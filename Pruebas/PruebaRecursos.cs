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
        //[Fact]
        //public void EnviarCodigoConfirmacion_DebeRetornarCodigo_CuandoElCorreoEsValido()
        //{
        //    var mockSmtpClient = new Mock<SmtpClient>();
        //    var servicioCorreo = new ServicioCorreo(mockSmtpClient.Object);

        //    string correo = "test@example.com";
        //    string codigo = servicioCorreo.EnviarCodigoConfirmacion(correo);

        //    Assert.NotNull(codigo);
        //    Assert.Equal(6, codigo.Length);
        //    mockSmtpClient.Verify(m => m.Send(It.IsAny<MailMessage>()), Times.Once);
        //}

        //[Fact]
        //public void EnviarCodigoConfirmacion_DebeRetornarNull_CuandoSeLanzaSmtpException()
        //{
        //    var mockSmtpClient = new Mock<SmtpClient>();
        //    mockSmtpClient
        //        .Setup(m => m.Send(It.IsAny<MailMessage>()))
        //        .Throws(new SmtpException("Error al enviar el correo"));

        //    var servicioCorreo = new ServicioCorreo(mockSmtpClient.Object);
        //    string correo = "test@example.com";
        //    string codigo = servicioCorreo.EnviarCodigoConfirmacion(correo);

        //    Assert.Null(codigo);
        //}

        //[Fact]
        //public void EnviarCodigoConfirmacion_DebeRetornarNull_CuandoSeLanzaExcepcionGeneral()
        //{
        //    var mockSmtpClient = new Mock<SmtpClient>();
        //    mockSmtpClient
        //        .Setup(m => m.Send(It.IsAny<MailMessage>()))
        //        .Throws(new Exception("Error general"));

        //    var servicioCorreo = new ServicioCorreo(mockSmtpClient.Object);
        //    string correo = "test@example.com";
        //    string codigo = servicioCorreo.EnviarCodigoConfirmacion(correo);

        //    Assert.Null(codigo);
        //}
    }
}




//[Fact]
//public void EnviarCodigoConfirmacion_DebeRetornarCodigo_CuandoCorreoEsValido()
//{
//    // Arrange
//    var recursos = new Recursos();
//    string correoValido = "axel.lu04@gmail.com";

//    // Act
//    string codigo = recursos.EnviarCodigoConfirmacion(correoValido);

//    // Assert
//    Assert.NotNull(codigo);
//    Assert.Equal(6, codigo.Length);
//    Assert.True(int.TryParse(codigo, out _), "El código no es un número válido.");
//}

//[Fact]
//public void EnviarCodigoConfirmacion_DebeRetornarNull_CuandoCorreoEsInvalido()
//{
//    // Arrange
//    var recursos = new Recursos();
//    string correoInvalido = "correo_invalido";

//    // Act
//    string codigo = recursos.EnviarCodigoConfirmacion(correoInvalido);

//    // Assert
//    Assert.Null(codigo);
//}

//[Fact]
//public void ValidarCodigo_DebeRetornarTrue_CuandoCodigosCoinciden()
//{
//    // Arrange
//    var recursos = new Recursos();
//    string codigoEnviado = "123456";
//    string codigoIngresado = "123456";

//    // Act
//    bool esValido = recursos.ValidarCodigo(codigoIngresado, codigoEnviado);

//    // Assert
//    Assert.True(esValido, "Los códigos deberían coincidir.");
//}

//[Fact]
//public void ValidarCodigo_DebeRetornarFalse_CuandoCodigosNoCoinciden()
//{
//    // Arrange
//    var recursos = new Recursos();
//    string codigoEnviado = "123456";
//    string codigoIngresado = "654321";

//    // Act
//    bool esValido = recursos.ValidarCodigo(codigoIngresado, codigoEnviado);

//    // Assert
//    Assert.False(esValido, "Los códigos no deberían coincidir.");
//}

//[Fact]
//public void GenerarSalt_DebeRetornarSalt_DeLongitudEspecificada()
//{
//    // Arrange
//    int tamanoSalt = 16;

//    // Act
//    string salt = Recursos.GenerarSalt(tamanoSalt);

//    // Assert
//    Assert.NotNull(salt);
//    Assert.Equal(tamanoSalt, Convert.FromBase64String(salt).Length);
//}

//[Fact]
//public void HashearContrasena_DebeRetornarHash_CuandoSeProporcionaSalt()
//{
//    // Arrange
//    string contrasena = "Password123";
//    string salt = Recursos.GenerarSalt();

//    // Act
//    string hash = Recursos.HashearContrasena(contrasena, salt);

//    // Assert
//    Assert.NotNull(hash);
//    Assert.NotEmpty(hash);
//}

//[Fact]
//public void VerificarContrasena_DebeRetornarTrue_CuandoContrasenaEsCorrecta()
//{
//    // Arrange
//    string contrasena = "Password123";
//    string salt = Recursos.GenerarSalt();
//    string hash = Recursos.HashearContrasena(contrasena, salt);

//    var cuenta = new Cuenta
//    {
//        ContraseniaHash = hash,
//        Salt = salt
//    };

//    // Act
//    bool esValida = Recursos.VerificarContrasena(contrasena, cuenta);

//    // Assert
//    Assert.True(esValida, "La contraseña debería ser válida.");
//}

//[Fact]
//public void VerificarContrasena_DebeRetornarFalse_CuandoContrasenaEsIncorrecta()
//{
//    // Arrange
//    string contrasenaCorrecta = "Password123";
//    string contrasenaIncorrecta = "WrongPassword";
//    string salt = Recursos.GenerarSalt();
//    string hash = Recursos.HashearContrasena(contrasenaCorrecta, salt);

//    var cuenta = new Cuenta
//    {
//        ContraseniaHash = hash,
//        Salt = salt
//    };

//    // Act
//    bool esValida = Recursos.VerificarContrasena(contrasenaIncorrecta, cuenta);

//    // Assert
//    Assert.False(esValida, "La contraseña no debería ser válida.");
//}