using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MallAuth.AuthProvider
{

    public class MyClaimAuthorizationFilter : AuthorizationFilterAttribute
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public bool ControllerAction { get { return string.IsNullOrWhiteSpace(this.ClaimType) || string.IsNullOrWhiteSpace(this.ClaimValue); } }

        public MyClaimAuthorizationFilter(string ClaimType = null, string ClaimValue = null)
        {
            this.ClaimType = ClaimType;
            this.ClaimValue = ClaimValue;
        }
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {
            if (ControllerAction)
            {
                var action = actionContext.ActionDescriptor.ActionName.ToLower();
                var resource = actionContext.ControllerContext.ControllerDescriptor.ControllerName.ToLower();
                if(!MallAuth.ServerCache.MallClaim.dict.ContainsKey(action)){
                  return NotAllow(actionContext);
                }
                ClaimType = MallAuth.ServerCache.MallClaim.dict[action];
                ClaimValue = resource;
            }

            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (!principal.Identity.IsAuthenticated)
            {
                return NotAllow(actionContext);

            }

            if (!(principal.HasClaim(x => x.Type == ClaimType && x.Value == ClaimValue)))
            {
                return NotAllow(actionContext);
            }

            //User is Authorized, complete execution
            return Task.FromResult<object>(null);

        }

        private static Task NotAllow(HttpActionContext actionContext)
        {
            var notAllowed = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response = notAllowed;
            return Task.FromResult<object>(null);
        }
    }
}