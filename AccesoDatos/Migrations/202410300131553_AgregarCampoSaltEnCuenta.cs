namespace AccesoDatos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregarCampoSaltEnCuenta : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cuentas", "Salt", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cuentas", "Salt");
        }
    }
}
