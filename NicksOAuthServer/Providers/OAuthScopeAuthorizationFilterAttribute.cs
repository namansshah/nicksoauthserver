using NicksOAuthServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace NicksOAuthServer.Providers
{
    public class OAuthScopeAuthorizationFilterAttribute : AuthorizationFilterAttribute
    {
        private string _requiredScope;
        
        public OAuthScopeAuthorizationFilterAttribute(string requiredScope)
        {
            _requiredScope = requiredScope;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if(actionContext.RequestContext.Principal is IPrincipal && actionContext.RequestContext.Principal.Identity is ClaimsIdentity) {
                ClaimsIdentity identity = actionContext.RequestContext.Principal.Identity as ClaimsIdentity;
                Claim scopeClaim = identity.Claims.SingleOrDefault(c => c.Type == NicksOAuthConstants.ScopeClaimType);
                if (scopeClaim != null)
                {
                    string[] assignedScopes = scopeClaim.Value.Split(' ');
                    if (assignedScopes.Contains(_requiredScope))
                    {
                        return;
                    }
                }
            }
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
        }                
    }
}