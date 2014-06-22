using NicksOAuthServer.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NicksOAuthServer.Models
{
    public class OAuthClient
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public Guid ClientId { get; set; }
        public string ClientSecretHash { get; set; }
        [Required]
        public OAuthGrant AllowedGrant {get; set;}
        public DateTimeOffset CreatedOn {get;set;}
        [Required]
        public bool Enabled { get; set; }
        public int OrganizationId { get; set; }
    }
}