using AccesoDatos.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

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

        public bool EditarNombreUsuario(int idJugador, string nuevoNombreUsuario)
        {
            using (var contexto = new ContextoBaseDatos())
            {
                var jugador = contexto.Jugadores
                    .FirstOrDefault(j => j.JugadorId == idJugador);

                if (jugador != null)
                {
                    jugador.NombreUsuario = nuevoNombreUsuario;

                    try
                    {
                        int filasAlteradas = contexto.SaveChanges();
                        return filasAlteradas > 0;
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine("Error al actualizar el nombre de usuario: " + ex);
                        return false;
                    }
                }
                return false;
            }
        }
    }
}

