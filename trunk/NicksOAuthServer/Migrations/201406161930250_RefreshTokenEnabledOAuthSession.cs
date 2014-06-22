namespace NicksOAuthServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefreshTokenEnabledOAuthSession : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OAuthSessions", "RefreshTokenEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OAuthSessions", "RefreshTokenEnabled");
        }
    }
}
