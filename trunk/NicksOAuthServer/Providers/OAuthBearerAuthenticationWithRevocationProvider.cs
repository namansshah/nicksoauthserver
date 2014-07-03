using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;
using NicksOAuthServer.Models;

namespace NicksOAuthServer.Providers
{
    public class OAuthBearerAuthenticationWithRevocationProvider : OAuthBearerAuthenticationProvider
    {
        public static string OAuthSessionClaimKey = "OAuthSession";
        public static string OAuthClientCredentialsGrantKey = "OAuthClientCredentialsGrant";
        
        public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        {            
            bool validated = false;
            base.ValidateIdentity(context);
            ApplicationDbContext dbContext = context.OwinContext.Get<ApplicationDbContext>();
            ApplicationUserManager userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            
            if(context.Ticket!= null && context.Ticket.Identity != null)
            {                                
                if(context.Ticket.Identity.Claims.SingleOrDefault(c => c.Type == OAuthClientCredentialsGrantKey) != null)
                {
                    Guid clientId = new Guid(context.Ticket.Identity.Name);
                    if (dbContext.OAuthClients.SingleOrDefault(oac => oac.ClientId == clientId && oac.Enabled==true) != null)
                    {
                        validated = true;
                        context.Validated();
                    }
                }
                else {
                    Claim oauthSessionId = context.Ticket.Identity.Claims.SingleOrDefault(c => c.Type == OAuthSessionClaimKey);
                    if (oauthSessionId != null)
                    {                    
                        OAuthSession oauthSession = dbContext.OAuthSessions.SingleOrDefault(oas => oas.Id.ToString() == oauthSessionId.Value);
                        if (oauthSession != null)
                        {
                            validated = true;
                            context.Validated();
                        }
                    }
                }
            }            
            if (!validated)
            {
                context.SetError("Invalid Token", "The Access Token is invalid.");
                context.Rejected();
            }
            return Task.FromResult<object>(null);
        }

    }
}