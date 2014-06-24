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

        //Validates Identity if the framework was able to populate the user's identity from the Access Token AND the sessions table indicate that the user has not logged out
        public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            bool validated = false;
            base.ValidateIdentity(context);
            ApplicationDbContext dbContext = context.OwinContext.Get<ApplicationDbContext>();
            ApplicationUserManager userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            
            if(context.Ticket!= null && context.Ticket.Identity != null)
            {
                Claim oauthSessionId = context.Ticket.Identity.Claims.First(c => c.Type == OAuthSessionClaimKey);
                if (oauthSessionId != null)
                {
                    
                    OAuthSession oauthSession = dbContext.OAuthSessions.SingleOrDefault(oas => oas.Id.ToString() == oauthSessionId.Value);
                    if (oauthSession != null)
                    {
                        validated = true;
                    }
                }
            }
            if (!validated)
            {
                context.SetError("Invalid Token", "The Access Token is invalid.");
            }
            return Task.FromResult<object>(null);
        }

    }
}