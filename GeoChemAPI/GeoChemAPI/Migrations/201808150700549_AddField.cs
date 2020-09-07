namespace GeoChemAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Env_datapoint", "Field", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Env_datapoint", "Field");
        }
    }
}
