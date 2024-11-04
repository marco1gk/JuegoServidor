namespace AccesoDatos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class atributoNuevoo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Amistades", "EstadoAmistad", c => c.String());
            DropColumn("dbo.Amistades", "EstadoAmigstad");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Amistades", "EstadoAmigstad", c => c.String());
            DropColumn("dbo.Amistades", "EstadoAmistad");
        }
    }
}
