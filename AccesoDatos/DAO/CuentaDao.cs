using AccesoDatos.Modelo;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos.DAO
{
    public class CuentaDao//editar cuenta
    {
        public bool AgregarJugadorConCuenta(Jugador jugador, Cuenta cuenta )
        {
            using (var contexto = new ContextoBaseDatos())
            {
                contexto.Cuentas.Add( cuenta );
                contexto.Jugadores.Add(jugador);
                cuenta.Jugador = jugador;
                cuenta.JugadorId = jugador.JugadorId;
                try
                {
                    int filasAlteradas = contexto.SaveChanges();
                    return filasAlteradas == 2;

                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine("Error al agregar"+ex  );
                    return false;
                }
            } 
           
        }


        public bool ExistenciaCorreo(string correo)
        {
            using (var contexto = new ContextoBaseDatos())
            {
                return contexto.Cuentas.Any(cuenta => cuenta.Correo == correo);
            }
        }
    }
}
