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
        private static readonly ILogger logger = GestionLogger.ObtenerLogger();

        public static void ManejarErrorExcepcion(Exception ex)
        {
            logger.Error(ex.Source + " - " + ex.Message + "\n" + ex.StackTrace + "\n");
        }

        public static void ManejarFatalExcepcion(Exception ex)
        {
            logger.Fatal(ex.Source + " - " + ex.Message + "\n" + ex.StackTrace + "\n");
        }
    }
}
