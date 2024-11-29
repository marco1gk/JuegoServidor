using AccesoDatos.Modelo;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net.Mail;
using AccesoDatos.Utilidades;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using AccesoDatos.Excepciones;

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
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    Console.WriteLine("Error al agregar" + ex);
                    return false;
                }
                catch (EntityException ex)
                {
                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    return false;
                }
                catch (SqlException ex)
                {

                    ManejadorExcepciones.ManejarErrorExcepcion(ex);
                    return false;
                }
                catch (Exception ex)
                {
                    ManejadorExcepciones.ManejarFatalExcepcion(ex);
                    return false;
                }
            }

        }


        public Cuenta ValidarInicioSesion(string correo, string contraseniaHash)
        {
            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var cuenta = contexto.Cuentas
                        .Include(c => c.Jugador)
                        .FirstOrDefault(c => c.Correo == correo && c.ContraseniaHash == contraseniaHash);

                    if (cuenta == null)
                    {
                        throw new ExcepcionAccesoDatos($"No se encontró una cuenta con el correo: {correo} o la contraseña es incorrecta.");
                    }

                    return cuenta;
                }
            }
            catch (SqlException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new ExcepcionAccesoDatos("Error en la base de datos: " + ex.Message);
            }
            catch (EntityException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new ExcepcionAccesoDatos("Error con Entity Framework: " + ex.Message);
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(ex);
                throw new ExcepcionAccesoDatos("Error inesperado al validar el inicio de sesión: " + ex.Message);
            }
        }


        public Cuenta ObtenerCuentaPorNombreUsuario(string nombreUsuario)
        {
            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    var cuenta = contexto.Cuentas
                        .Include(c => c.Jugador)
                        .FirstOrDefault(c => c.Correo == nombreUsuario);

                    if (cuenta == null)
                    {
                        throw new ExcepcionAccesoDatos($"No se encontró una cuenta con el correo: {nombreUsuario}");
                    }

                    return cuenta;
                }
            }
            catch (SqlException ex)
            {

                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new ExcepcionAccesoDatos("Error en la base de datos: " + ex.Message);
            }
            catch (EntityException ex)
            {

                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                throw new ExcepcionAccesoDatos("Error en Entity Framework: " + ex.Message);
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(ex);
                throw new ExcepcionAccesoDatos("Error inesperado al obtener la cuenta: " + ex.Message);
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
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        Console.WriteLine("Error al editar la contraseña: " + ex);
                        return false;
                    }
                    catch (SqlException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        Console.WriteLine("Error de base de datos: " + ex.Message);
                        return false;
                    }
                    catch (EntityException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        Console.WriteLine("Error de Entity Framework: " + ex.Message);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        ManejadorExcepciones.ManejarFatalExcepcion(ex);
                        Console.WriteLine("Error inesperado: " + ex.Message);
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
            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    return contexto.Cuentas.Any(cuenta => cuenta.Correo == correo);
                }
            }
            catch (SqlException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                Console.WriteLine("Error de base de datos: " + ex.Message);
                return false;
            }
            catch (EntityException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                Console.WriteLine("Error de Entity Framework: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(ex);
                Console.WriteLine("Error inesperado: " + ex.Message);
                return false;
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
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        Console.WriteLine("Error al actualizar el correo: " + ex);
                        return false;
                    }
                    catch (SqlException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        Console.WriteLine("Error de base de datos: " + ex.Message);
                        return false;
                    }
                    catch (EntityException ex)
                    {
                        ManejadorExcepciones.ManejarErrorExcepcion(ex);
                        Console.WriteLine("Error de Entity Framework: " + ex.Message);
                        return false;
                    }
                    catch (Exception ex)
                    {
                        ManejadorExcepciones.ManejarFatalExcepcion(ex);
                        Console.WriteLine("Error inesperado: " + ex.Message);
                        return false;
                    }
                }
                return false;
            }
        }

        public bool ExisteCorreo(string correo)
        {
            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    return contexto.Cuentas.Any(c => c.Correo == correo);
                }
            }
            catch (SqlException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                Console.WriteLine("Error de base de datos: " + ex.Message);
                return false;
            }
            catch (EntityException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                Console.WriteLine("Error de Entity Framework: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(ex);
                Console.WriteLine("Error inesperado: " + ex.Message);
                return false;
            }

        }

       
        public bool ExisteNombreUsuario(string nombreUsuario)
        {
            try
            {
                using (var contexto = new ContextoBaseDatos())
                {
                    return contexto.Jugadores.Any(j => j.NombreUsuario == nombreUsuario);
                }
            }
            catch (SqlException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                Console.WriteLine("Error de base de datos: " + ex.Message);
                return false; 
            }
            catch (EntityException ex)
            {
                ManejadorExcepciones.ManejarErrorExcepcion(ex);
                Console.WriteLine("Error de Entity Framework: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                ManejadorExcepciones.ManejarFatalExcepcion(ex);
                Console.WriteLine("Error inesperado: " + ex.Message);
                return false;
            }
        }

    }


}
