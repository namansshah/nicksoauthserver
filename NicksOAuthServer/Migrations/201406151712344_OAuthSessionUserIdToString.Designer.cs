// <auto-generated />
namespace NicksOAuthServer.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.1.0-30225")]
    public sealed partial class OAuthSessionUserIdToString : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(OAuthSessionUserIdToString));
        
        string IMigrationMetadata.Id
        {
            get { return "201406151712344_OAuthSessionUserIdToString"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}
