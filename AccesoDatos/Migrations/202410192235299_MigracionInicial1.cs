namespace AccesoDatos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigracionInicial1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Jugadors", newName: "Jugadores");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Jugadores", newName: "Jugadors");
        }
    }
}
