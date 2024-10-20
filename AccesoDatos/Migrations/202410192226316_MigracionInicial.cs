namespace AccesoDatos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigracionInicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cuentas",
                c => new
                    {
                        JugadorId = c.Int(nullable: false),
                        Correo = c.String(nullable: false, maxLength: 100),
                        ContraseniaHash = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.JugadorId)
                .ForeignKey("dbo.Jugadors", t => t.JugadorId, cascadeDelete: true)
                .Index(t => t.JugadorId)
                .Index(t => t.Correo, unique: true, name: "IX_CuentaCorreo");
            
            CreateTable(
                "dbo.Jugadors",
                c => new
                    {
                        JugadorId = c.Int(nullable: false, identity: true),
                        NombreUsuario = c.String(nullable: false, maxLength: 50),
                        NumeroFotoPerfil = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.JugadorId)
                .Index(t => t.NombreUsuario, unique: true, name: "IX_JugadorNombreJugador");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cuentas", "JugadorId", "dbo.Jugadors");
            DropIndex("dbo.Jugadors", "IX_JugadorNombreJugador");
            DropIndex("dbo.Cuentas", "IX_CuentaCorreo");
            DropIndex("dbo.Cuentas", new[] { "JugadorId" });
            DropTable("dbo.Jugadors");
            DropTable("dbo.Cuentas");
        }
    }
}
