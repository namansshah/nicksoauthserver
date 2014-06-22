using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NicksOAuthServer.Providers
{
    public enum OAuthGrant
    {
        AuthorizationCode=1,
        Implicit=2,
        ResourceOwnerPasswordCredentials=3,
        ClientCredentials=4
    }
}