namespace AccesoDatos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class añadirEstadisticas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Estadisticas",
                c => new
                    {
                        IdEstadisticas = c.Int(nullable: false, identity: true),
                        IdJugador = c.Int(nullable: false),
                        NumeroVictorias = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdEstadisticas)
                .ForeignKey("dbo.Jugadores", t => t.IdJugador)
                .Index(t => t.IdJugador);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Estadisticas", "IdJugador", "dbo.Jugadores");
            DropIndex("dbo.Estadisticas", new[] { "IdJugador" });
            DropTable("dbo.Estadisticas");
        }
    }
}
