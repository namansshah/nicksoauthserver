using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using NicksOAuthServer.Models;

namespace NicksOAuthServer.Providers
{
    public class NicksApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        public static readonly string OwinClientKey = "nick:oauth:client";
        public static readonly string OwinUserKey = "nick:oauth:user";
        public static readonly string OwinSessionKey = "nick:oauth:session";

        public NicksApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }
            
            _publicClientId = publicClientId;
        }                

        //validate that the client id is allowed to use Resource Owner Password Credentials Grant, authenticate the user, and verify the organizations match
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            bool validated=false;
            OAuthClient oauthClient = context.OwinContext.Get<OAuthClient>(OwinClientKey);
            //Make sure the client is allowed to be used for the Resource Owner Password Credentials Grant
            if(oauthClient!=null && oauthClient.AllowedGrant == OAuthGrant.ResourceOwnerPasswordCredentials)
            {
                ApplicationUserManager userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
                //ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
                ApplicationUser user = await userManager.FindWithLockoutAsync(context.UserName, context.Password);

                if (user != null && user.OrganizationId == oauthClient.OrganizationId)                
                {
                    ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);                    
                    //ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,CookieAuthenticationDefaults.AuthenticationType);
                    Guid oauthSessionValue=Guid.NewGuid();
                    oAuthIdentity.AddClaim(new Claim(OAuthBearerAuthenticationWithRevocationProvider.OAuthSessionClaimKey, oauthSessionValue.ToString()));
                    
                    ApplicationDbContext dbContext = context.OwinContext.Get<ApplicationDbContext>();
                    var oauthSessions = dbContext.OAuthSessions.Where(oas => oas.UserId.ToString() == user.Id && oas.ClientId == oauthClient.Id);
                    foreach(OAuthSession oauthSession in oauthSessions){
                        dbContext.OAuthSessions.Remove(oauthSession);
                    }

                    dbContext.OAuthSessions.Add(new OAuthSession
                    {
                        ClientId = oauthClient.Id,                        
                        OrganizationId = user.OrganizationId,
                        UserId = user.Id,
                        Id = oauthSessionValue
                    });
                    dbContext.SaveChanges();

                    AuthenticationProperties properties = CreateProperties(user.UserName);
                    AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);                    
                    context.Validated(oAuthIdentity);
                    context.Request.Context.Authentication.SignIn(oAuthIdentity);
                    context.OwinContext.Set<ApplicationUser>(OwinUserKey, user);
                    validated = true;
                }
            }

            if (!validated)
            {
                context.SetError("Authentication Failed");
                context.Rejected();
            }            
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        //Validate the client id and client secret
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            bool validated = false;
            string clientId;
            string clientSecret;
            
            //Try to get the client id and secret from Basic Auth Header
            if(context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                ApplicationUserManager userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
                ApplicationDbContext dbContext = context.OwinContext.Get<ApplicationDbContext>();
                
                if(clientId != null){
                    OAuthClient oauthClient = await dbContext.OAuthClients.FirstOrDefaultAsync(oac => oac.ClientId.ToString() == clientId);
                    if (oauthClient != null && oauthClient.Enabled && userManager.PasswordHasher.VerifyHashedPassword(oauthClient.ClientSecretHash, clientSecret)==PasswordVerificationResult.Success)
                    {
                        context.OwinContext.Set<OAuthClient>(OwinClientKey, oauthClient);
                        context.Validated(clientId);
                        validated = true;
                    }
                }
            }

            if (!validated)
            {
                context.SetError("Authentication Failed");
                context.Rejected();
            }

            //return Task.FromResult<object>(null);
        }

        /*
        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }
        */

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            bool validated = false;
            if (context.Ticket != null && context.Ticket.Identity != null)
            {
                ApplicationUser user = context.OwinContext.Get<ApplicationUser>(OwinUserKey);
                OAuthClient oauthClient = context.OwinContext.Get<OAuthClient>(OwinClientKey);
                ApplicationDbContext dbContext = context.OwinContext.Get<ApplicationDbContext>();
                var oauthSessions = dbContext.OAuthSessions.Where(oas => oas.UserId.ToString() == user.Id && oas.ClientId == oauthClient.Id);
                foreach (OAuthSession oauthSession in oauthSessions)
                {
                    dbContext.OAuthSessions.Remove(oauthSession);
                }

                Guid oauthSessionValue = Guid.NewGuid();

                dbContext.OAuthSessions.Add(new OAuthSession
                {
                    ClientId = oauthClient.Id,
                    OrganizationId = user.OrganizationId,
                    UserId = user.Id,
                    Id = oauthSessionValue
                });
                dbContext.SaveChanges();

                context.Ticket.Identity.AddClaim(new Claim(OAuthBearerAuthenticationWithRevocationProvider.OAuthSessionClaimKey, oauthSessionValue.ToString()));

                context.Validated();
                context.Request.Context.Authentication.SignIn(context.Ticket.Identity);                
                validated = true;
            }

            if (!validated)
            {
                context.SetError("Authentication Failed");
                context.Rejected();
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}