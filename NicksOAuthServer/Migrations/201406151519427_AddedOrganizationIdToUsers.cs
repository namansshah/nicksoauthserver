namespace NicksOAuthServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOrganizationIdToUsers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "OrganizationId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "OrganizationId");
        }
    }
}
