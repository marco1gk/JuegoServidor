using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Serilog;



namespace AccesoDatos.Utilidades
{
    public static class GestionLogger
    {
        private static ILogger _logger;

        private static void ConfiguararLogger(string rutaDelArchivoDeRegistro)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(rutaDelArchivoDeRegistro)
                .CreateLogger();
        }   

        private static string ConstruirRutaDelArchivoDeRegistro()
        {
            const string formatoFecha = "dd-MM-yyyy";
            const string idArchivoNombre = "Log";
            const string caracterSeparador = "_";
            const string extensionArchivo = ".txt";
            const string rutaRelativaDelArchivoRegistro = "../../Logs\\";

            DateTime fechaActual = DateTime.Today;
            string fecha = fechaActual.ToString(formatoFecha);

            string nombreDelArchivoDeRegistro = idArchivoNombre + caracterSeparador + fecha + extensionArchivo;
            string rutaAbsolutaDelArchivoDeRegistro = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rutaRelativaDelArchivoRegistro);
            string rutaDeRegistro = rutaAbsolutaDelArchivoDeRegistro + nombreDelArchivoDeRegistro;

            return rutaDeRegistro;
        }

        public static ILogger ObtenerLogger()
        {
            if (_logger == null)
            {
                string rutaDeRegistro = ConstruirRutaDelArchivoDeRegistro();
                ConfiguararLogger(rutaDeRegistro);
            }
            _logger = Log.Logger;
            return _logger;
        }

        public static void CerrarYVaciar()
        {
            (_logger as IDisposable)?.Dispose();
            Log.CloseAndFlush();
            _logger = null;
        }

    }
}
