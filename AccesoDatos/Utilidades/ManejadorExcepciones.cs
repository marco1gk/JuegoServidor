using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace AccesoDatos.Utilidades
{
    public static class ManejadorExcepciones
    {
        private static readonly ILogger _logger = GestionLogger.GetLogger();

        public static void ManejarErrorExcepcion(Exception ex)
        {
            _logger.Error(ex.Source + " - " + ex.Message + "\n" + ex.StackTrace + "\n");
        }

        public static void ManejarFatalExcepcion(Exception ex)
        {
            _logger.Fatal(ex.Source + " - " + ex.Message + "\n" + ex.StackTrace + "\n");
        }
    }
}
