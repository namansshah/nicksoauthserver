namespace NicksOAuthServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OAuthSessionUserIdToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OAuthSessions", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OAuthSessions", "UserId", c => c.Guid(nullable: false));
        }
    }
}
