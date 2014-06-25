using NicksOAuthServer.Models;
using NicksOAuthServer.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
namespace NicksOAuthServer.Controllers
{
    [Authorize]
    public class ValuesController : ApiController
    {
        public static IDictionary<int, string> values = new Dictionary<int, string>() { {3,"Value 3!"},{12, "Value 12!"}};

        [OAuthScopeAuthorizationFilter(NicksOAuthConstants.ValuesReadScope)]
        // GET api/values
        public IHttpActionResult Get()
        {
            return Ok<IEnumerable<string>>(values.Keys.Select(v => String.Format("{0}: {1}", v, values[v])));
        }

        [OAuthScopeAuthorizationFilter(NicksOAuthConstants.ValuesReadScope)]
        // GET api/values/5
        public IHttpActionResult Get(int id)
        {
            if (values.Keys.Contains(id))
            {
                return Ok(values[id]);
            }
            return NotFound();
        }

        [OAuthScopeAuthorizationFilter(NicksOAuthConstants.ValuesModifyScope)]
        // PUT api/values/5
        public IHttpActionResult Put(int id, PutValueModel model)
        {
            if (!String.IsNullOrWhiteSpace(model.value))
            {
                values[id] = model.value;
                return Ok();
            }
            return BadRequest();
        }

        [OAuthScopeAuthorizationFilter(NicksOAuthConstants.ValuesModifyScope)]
        // DELETE api/values/5
        public IHttpActionResult Delete(int id)
        {
            if(values.Keys.Contains(id)){
                values.Remove(id);
                return Ok();
            }
            else {
                return StatusCode(HttpStatusCode.NoContent);
            }            
        }

        public class PutValueModel
        {
            [Required]
            [Display(Name = "The Value")]
            public string value { get; set; }
        }
    }
}
