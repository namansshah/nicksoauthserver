using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace NicksOAuthServer.Models
{
    public static class NicksOAuthConstants
    {
        public const string RoleClaimType = ClaimsIdentity.DefaultRoleClaimType;
        public const string IssuerClaimType = ClaimsIdentity.DefaultIssuer;
        public const string ScopeClaimType = "urn:oauth:scope";

        public const string ValuesReadScope = "values-read";
        public const string ValuesModifyScope = "values-modify";
        public const string ValuesAvailableScope = "values-available";

        public const string UserRole = "User";
        public const string ClientRole = "Client";

    }
}