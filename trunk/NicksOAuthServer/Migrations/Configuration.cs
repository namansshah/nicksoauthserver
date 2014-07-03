namespace NicksOAuthServer.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using NicksOAuthServer.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<NicksOAuthServer.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(NicksOAuthServer.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            Organization hyvee = new Organization
            {
                Name = "Hy-Vee"
            };

            context.Organizations.AddOrUpdate(Org => Org.Name, hyvee);

            Organization henhouse = new Organization
            {
                Name = "Hen House"
            };
            context.Organizations.AddOrUpdate(Org => Org.Name, henhouse);

            Organization pricechopper = new Organization
            {
                Name = "Price Chopper"
            };
            context.Organizations.AddOrUpdate(Org => Org.Name, pricechopper);

            context.SaveChanges();

            context.OAuthClients.AddOrUpdate(c => c.Name, new OAuthClient
            {
                Name = "Hy-Vee Mobile API",
                AllowedGrant = Providers.OAuthGrant.ResourceOwnerPasswordCredentials,
                ClientId=Guid.NewGuid(),
                CreatedOn=DateTimeOffset.Now,
                Description="Mobile API for Hy-Vee",
                Enabled=true,
                ClientSecretHash = userManager.PasswordHasher.HashPassword("Start123!"),
                OrganizationId=hyvee.Id
            });
            
            context.OAuthClients.AddOrUpdate(c => c.Name, new OAuthClient
            {
                Name = "Hen House Mobile API",
                AllowedGrant = Providers.OAuthGrant.ResourceOwnerPasswordCredentials,
                ClientId = Guid.NewGuid(),
                CreatedOn = DateTimeOffset.Now,
                Description = "Mobile API for Hen House",
                Enabled = true,
                ClientSecretHash = userManager.PasswordHasher.HashPassword("Start123!"),
                OrganizationId=henhouse.Id
            });

            context.OAuthClients.AddOrUpdate(c => c.Name, new OAuthClient
            {
                Name = "Price Chopper Mobile API",
                AllowedGrant = Providers.OAuthGrant.ResourceOwnerPasswordCredentials,
                ClientId = Guid.NewGuid(),
                CreatedOn = DateTimeOffset.Now,
                Description = "Mobile API for Price Chopper",
                Enabled = true,
                ClientSecretHash = userManager.PasswordHasher.HashPassword("Start123!"),
                OrganizationId=pricechopper.Id
            });

            context.OAuthClients.AddOrUpdate(c => c.Name, new OAuthClient
            {
                Name = "Hy-vee Mobile Client Credentials",
                AllowedGrant = Providers.OAuthGrant.ClientCredentials,
                ClientId = Guid.NewGuid(),
                CreatedOn = DateTimeOffset.Now,
                Description = "Client Credentials Grant for Hy-vee",
                Enabled = true,
                ClientSecretHash = userManager.PasswordHasher.HashPassword("Start123!"),
                OrganizationId = hyvee.Id
            });

            context.SaveChanges();            
            
            ApplicationUser foundUser = userManager.FindByName("firsthyveeuser");
            if (foundUser == null)
            {
                ApplicationUser user = new ApplicationUser() { UserName = "firsthyveeuser", Email = "nick.coblentz@gmail.com", FirstName = "First", LastName = "Hy-Vee User", OrganizationId = hyvee.Id, LockoutEnabled = true, PINHash = userManager.PasswordHasher.HashPassword("1234")};
                IdentityResult result = userManager.Create(user, "Start123!");
            }

            foundUser = userManager.FindByName("firstpricechopperuser");
            if (foundUser == null)
            {
                ApplicationUser user = new ApplicationUser() { UserName = "firstpricechopperuser", Email = "nick.coblentz@gmail.com", FirstName = "First", LastName = "Price Chopper User", OrganizationId = pricechopper.Id, LockoutEnabled = true, PINHash = userManager.PasswordHasher.HashPassword("1234") };
                IdentityResult result = userManager.Create(user, "Start123!");
            }

            foundUser = userManager.FindByName("firsthenhouseuser");
            if (foundUser == null)
            {
                ApplicationUser user = new ApplicationUser() { UserName = "firsthenhouseuser", Email = "nick.coblentz@gmail.com", FirstName = "First", LastName = "Hen House User", OrganizationId = henhouse.Id, LockoutEnabled = true, PINHash = userManager.PasswordHasher.HashPassword("1234") };
                IdentityResult result = userManager.Create(user, "Start123!");
            }

            
        }
    }
}
