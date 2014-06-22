using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NicksOAuthServer.Models
{
    public class OAuthSession
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid RefreshToken { get; set; }
        public bool RefreshTokenEnabled { get; set; }
        public int OrganizationId { get; set; }
        public int ClientId { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

        public OAuthSession()
        {
            CreatedOn = DateTimeOffset.Now;
            RefreshTokenEnabled = false;
        }

        public bool IsRefreshTokenValid(Guid refreshToken, TimeSpan validTimeSpanWindow) {
            TimeSpan result = DateTimeOffset.Now - CreatedOn;
            if (RefreshTokenEnabled && refreshToken.Equals(RefreshToken) && DateTimeOffset.Now - CreatedOn < validTimeSpanWindow)
            {
                return true;
            }
            return false;            
        }
    }
}