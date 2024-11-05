using AccesoDatos.Modelo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AccesoDatos
{
    public class ContextoBaseDatos : DbContext
    {
        public virtual DbSet<Cuenta> Cuentas { get; set; }
        public virtual DbSet<Jugador> Jugadores { get; set; }

        public DbSet<Amistad> Amistades { get; set; }

        public ContextoBaseDatos() : base ("name=ContactoBaseDatos") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Jugador>().ToTable("Jugadores").Property(jugador => jugador.NombreUsuario).HasColumnAnnotation(
                IndexAnnotation.AnnotationName,
            new IndexAnnotation(
                    new IndexAttribute("IX_JugadorNombreJugador") { IsUnique = true }
                    ));

            modelBuilder.Entity<Cuenta>().Property(cuenta => cuenta.Correo).HasColumnAnnotation(
               IndexAnnotation.AnnotationName,
           new IndexAnnotation(
                   new IndexAttribute("IX_CuentaCorreo") { IsUnique = true }
                   ));

            modelBuilder.Entity<Jugador>()
                .HasOptional(jugador => jugador.Cuenta)
                .WithRequired(cuenta => cuenta.Jugador)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Amistad>()
                .ToTable("Amistades")
                .HasRequired(amistad => amistad.Jugador)
                .WithMany()
                .HasForeignKey(amistad => amistad.JugadorId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }


    }
}

