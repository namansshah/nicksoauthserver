namespace NicksOAuthServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOrganizationIdToOAuthClients : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OAuthClients", "OrganizationId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OAuthClients", "OrganizationId");
        }
    }
}
