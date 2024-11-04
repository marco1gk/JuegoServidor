namespace AccesoDatos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class atributoNuevo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Amistades", "EstadoAmigstad", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Amistades", "EstadoAmigstad");
        }
    }
}
