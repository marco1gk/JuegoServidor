namespace AccesoDatos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AmistadRealizada : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Amistades",
                c => new
                    {
                        AmistadId = c.Int(nullable: false, identity: true),
                        JugadorId = c.Int(nullable: false),
                        AmigoId = c.Int(nullable: false),
                        ImagenAmigoId = c.Int(nullable: false),
                        EnLinea = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AmistadId)
                .ForeignKey("dbo.Jugadores", t => t.JugadorId)
                .Index(t => t.JugadorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Amistades", "JugadorId", "dbo.Jugadores");
            DropIndex("dbo.Amistades", new[] { "JugadorId" });
            DropTable("dbo.Amistades");
        }
    }
}
