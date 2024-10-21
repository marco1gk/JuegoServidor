using AccesoDatos.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace AccesoDatos.DAO
{
    public class JugadorDao
    {
        public Jugador ObtenerJugador(int idJugador)
        {
            using (var contexto = new ContextoBaseDatos())
            {
                return contexto.Jugadores
                    .Include(j => j.Cuenta)
                    .FirstOrDefault(j => j.JugadorId == idJugador);
            }
        }
    }
}

