using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.Excepciones
{
    public class ExcepcionAccesoDatos : Exception
    {
        public ExcepcionAccesoDatos(string mensaje) : base(mensaje) { }

    }
}
