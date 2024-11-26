using AccesoDatos.Modelo;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net.Mail;

namespace AccesoDatos.DAO
{
    public class CuentaDao
    {
        public bool AgregarJugadorConCuenta(Jugador jugador, Cuenta cuenta)
        {
            using (var contexto = new ContextoBaseDatos())
            {
                contexto.Cuentas.Add(cuenta);
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
                    Console.WriteLine("Error al agregar" + ex);
                    return false;
                }
            }

        }


        public Cuenta ValidarInicioSesion(string correo, string contraseniaHash)
        {
            using (var contexto = new ContextoBaseDatos())
            {
                return contexto.Cuentas
                    .Include(c => c.Jugador)
                    .FirstOrDefault(c => c.Correo == correo && c.ContraseniaHash == contraseniaHash);
            }
        }

        public Cuenta ObtenerCuentaPorNombreUsuario(string nombreUsuario)
        {
            using (var contexto = new ContextoBaseDatos())
            {
                return contexto.Cuentas
                    .Include(c => c.Jugador)
                    .FirstOrDefault(c => c.Correo == nombreUsuario);
            }
        }

        public bool EditarContraseñaPorCorreo(string correo, string nuevaContrasenia)
        {
            using (var contexto = new ContextoBaseDatos())
            {
                var cuentaExistente = contexto.Cuentas.SingleOrDefault(c => c.Correo == correo);

                if (cuentaExistente != null)
                {
                    string nuevoSalt = Recursos.GenerarSalt();
                    string nuevaContraseniaHash = Recursos.HashearContrasena(nuevaContrasenia, nuevoSalt);
                    cuentaExistente.ContraseniaHash = nuevaContraseniaHash;
                    cuentaExistente.Salt = nuevoSalt;

                    try
                    {
                        int filasAlteradas = contexto.SaveChanges();
                        return filasAlteradas > 0;
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine("Error al editar la contraseña: " + ex);
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("No se encontró la cuenta con el correo proporcionado.");
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

        public bool EditarCorreo(int idCuenta, string nuevoCorreo)
        {
            using (var contexto = new ContextoBaseDatos())
            {
                var cuenta = contexto.Cuentas
                    .FirstOrDefault(j => j.JugadorId == idCuenta);

                if (cuenta != null)
                {
                    cuenta.Correo = nuevoCorreo;

                    try
                    {
                        int filasAlteradas = contexto.SaveChanges();
                        return filasAlteradas > 0;
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine("Error al actualizar el correo: " + ex);
                        return false;
                    }
                }
                return false;
            }
        }

        public bool ExisteCorreo(string correo)
        {
            using (var context = new ContextoBaseDatos())
            {
                return context.Cuentas.Any(c => c.Correo == correo);
            }
        }

        public bool ExisteNombreUsuario(string nombreUsuario)
        {
            using (var context = new ContextoBaseDatos())
            {
                return context.Jugadores.Any(j => j.NombreUsuario == nombreUsuario);
            }
        }


    }


}
