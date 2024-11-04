namespace AccesoDatos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relacion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Amistades", "JugadorAmigo_JugadorId", c => c.Int());
            AlterColumn("dbo.Amistades", "AmigoId", c => c.Int());
            CreateIndex("dbo.Amistades", "JugadorAmigo_JugadorId");
            AddForeignKey("dbo.Amistades", "JugadorAmigo_JugadorId", "dbo.Jugadores", "JugadorId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Amistades", "JugadorAmigo_JugadorId", "dbo.Jugadores");
            DropIndex("dbo.Amistades", new[] { "JugadorAmigo_JugadorId" });
            AlterColumn("dbo.Amistades", "AmigoId", c => c.Int(nullable: false));
            DropColumn("dbo.Amistades", "JugadorAmigo_JugadorId");
        }
    }
}
