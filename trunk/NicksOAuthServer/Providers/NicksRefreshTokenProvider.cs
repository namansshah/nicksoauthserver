using Microsoft.Owin.Security.Infrastructure;
using NicksOAuthServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Helpers;
using System.IO;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin;
using Microsoft.AspNet.Identity;

namespace NicksOAuthServer.Providers
{
    public class NicksRefreshTokenProvider : IAuthenticationTokenProvider
    {
        public static string OAuthRefreshTokenSessionClaimKey = "OAuthRefreshSession";
        

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {            
            Guid refreshToken = Guid.NewGuid();
            
            ApplicationDbContext dbContext = context.OwinContext.Get<ApplicationDbContext>();
            ApplicationUser user = context.OwinContext.Get<ApplicationUser>(NicksApplicationOAuthProvider.OwinUserKey);
            OAuthClient oauthClient = context.OwinContext.Get<OAuthClient>(NicksApplicationOAuthProvider.OwinClientKey);
            OAuthSession oauthSession = dbContext.OAuthSessions.SingleOrDefault(oas => oas.UserId == user.Id && oas.ClientId == oauthClient.Id);
            oauthSession.RefreshToken = refreshToken;
            oauthSession.RefreshTokenEnabled = true;
            await dbContext.SaveChangesAsync();            
            context.SetToken(refreshToken.ToString());
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {            
            //PIN lockout not yet implemented. Might integrated with account lockout or its own thing
            //Uncomment this to accept a post parameter called "pin"
            //IFormCollection form = await context.Request.ReadFormAsync();
            //string submittedPin = form["PIN"];
            
            if(!String.IsNullOrEmpty(context.Token)){
                Guid refreshToken = Guid.Parse(context.Token);
                if(refreshToken != null){
                    ApplicationDbContext dbContext = context.OwinContext.Get<ApplicationDbContext>();

                    OAuthSession oauthSession = dbContext.OAuthSessions.SingleOrDefault(oas => oas.RefreshToken == refreshToken);
                    OAuthClient oauthClient = context.OwinContext.Get<OAuthClient>(NicksApplicationOAuthProvider.OwinClientKey);

                    if (oauthSession != null && oauthClient != null && oauthSession.ClientId == oauthClient.Id && oauthClient.OrganizationId == oauthSession.OrganizationId && oauthSession.IsRefreshTokenValid(refreshToken, Startup.RefreshTokenTimeSpan))
                    {
                        ApplicationUserManager userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
                        ApplicationUser user = await userManager.FindByIdAsync(oauthSession.UserId);
                        //Uncomment this and the closing brace to verify the PIN hash referenced above
                        //if (userManager.PasswordHasher.VerifyHashedPassword(user.PINHash,submittedPin) == PasswordVerificationResult.Success)
                        //{
                            context.OwinContext.Set<ApplicationUser>(NicksApplicationOAuthProvider.OwinUserKey, user);
                            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);
                            IDictionary<string, string> properties = new Dictionary<string, string>{ { "userName", user.UserName } };
                            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties(properties));
                            ticket.Properties.IssuedUtc = DateTimeOffset.UtcNow;
                            ticket.Properties.ExpiresUtc = DateTimeOffset.UtcNow.Add(Startup.OAuthOptions.AccessTokenExpireTimeSpan);
                            context.SetTicket(ticket);
                        //}
                    }
                }
            }
        }
        
        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}