using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using NicksOAuthServer.Models;
using System;

namespace NicksOAuthServer
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
            
        }

        public async Task<ApplicationUser> FindWithLockoutAsync(string userName, string password)
        {
            ApplicationUser returnUser = null;

            ApplicationUser foundUser = await FindAsync(userName, password);
            if (foundUser != null)
            {
                /* How to unlock an account
                DateTimeOffset lockoutEnd = DateTimeOffset.Now;
                lockoutEnd.AddHours(-1);
                await SetLockoutEndDateAsync(foundUser.Id, lockoutEnd.ToUniversalTime());
                 */
                if (!await IsLockedOutAsync(foundUser.Id))
                {
                    returnUser = foundUser;
                    await ResetAccessFailedCountAsync(foundUser.Id);
                }
            }
            else
            {
                ApplicationUser failedUser = await FindByNameAsync(userName);
                if (failedUser != null)
                {
                    await AccessFailedAsync(failedUser.Id);
                }
            }
            return returnUser;

        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true                
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true                
            };

            manager.UserLockoutEnabledByDefault = true;
            manager.MaxFailedAccessAttemptsBeforeLockout = 3;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromDays(365*10);
            

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
}
