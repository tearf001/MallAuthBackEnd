using MallAuth.ServerCache;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace MallAuth.AuthProvider
{
    /// <summary>
    /// 自定义 ClaimsAuthorzation管理器,相对于 AuthorizationFilterAttribute,管理器提供更为丰富的功能!,但是微软asp.net没有提供其实现,仅LoadCustomConfiguration
    /// </summary>
    public class SimpleRestAuthorizationManager : System.Security.Claims.ClaimsAuthorizationManager
    {
        public override bool CheckAccess(System.Security.Claims.AuthorizationContext context)
        {
            var name = context.Principal.Identity.Name;
            var claims = context.Principal ;//as ClaimsPrincipal;
            

            var action = context.Action.First().Value.ToLower();
            var rsc = context.Resource.First().Value.ToLower();

            if (MallClaim.dict.ContainsKey(action) && claims != null && claims.HasClaim(MallClaim.dict[action], rsc))
            {
                return true;
            }
            return false;
        }        
    }



}

//以下是Thinktecture.IdentityModel提供的管理器
//namespace Thinktecture.IdentityModel.Authorization.WebApi
//{
//    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
//    {
//        private string _action;
//        private string[] _resources;

//        public ClaimsAuthorizeAttribute()
//        { }

//        public ClaimsAuthorizeAttribute(string action, params string[] resources)
//        {
//            _action = action;
//            _resources = resources;
//        }
//        /// <summary>
//        /// 检验是否通过鉴权
//        /// </summary>
//        /// <param name="actionContext"></param>
//        /// <returns></returns>
//        protected override bool IsAuthorized(System.Web.Http.Controllers.HttpActionContext actionContext)
//        {
//            if (!string.IsNullOrWhiteSpace(_action)) //如果定义了Action,那么...
//            {
//                return ClaimsAuthorization.CheckAccess(_action, _resources);
//            }
//            else
//            {
//                //否则只检查 Action + ControllerName的访问权
//                return CheckAccess(actionContext);
//            }
//        }
//        /// <summary>
//        /// 供扩展的方法,只实现了Action + ControllerName的访问权
//        /// </summary>
//        /// <param name="actionContext"></param>
//        /// <returns></returns>
//        protected virtual bool CheckAccess(System.Web.Http.Controllers.HttpActionContext actionContext)
//        {
//            var action = actionContext.ActionDescriptor.ActionName;
//            var resource = actionContext.ControllerContext.ControllerDescriptor.ControllerName;

//            return ClaimsAuthorization.CheckAccess(action,resource);
//        }
//    }
//}

//namespace Thinktecture.IdentityModel.Authorization
//{
//    /// <summary>
//    /// Provides direct access methods for evaluating authorization policy
//    /// </summary>
//    public static class ClaimsAuthorization
//    {
//        /// <summary>
//        /// Default action claim type.
//        /// </summary>
//        public const string ActionType = "http://application/claims/authorization/action";

//        /// <summary>
//        /// Default resource claim type
//        /// </summary>
//        public const string ResourceType = "http://application/claims/authorization/resource";

//        public static bool EnforceAuthorizationManagerImplementation { get; set; }

//        /// <summary>
//        /// Gets the registered authorization manager.
//        /// </summary>
//        public static ClaimsAuthorizationManager AuthorizationManager
//        {
//            get
//            {
//                return FederatedAuthentication.FederationConfiguration.IdentityConfiguration.ClaimsAuthorizationManager;
//            }
//        }

//        static ClaimsAuthorization()
//        {
//            EnforceAuthorizationManagerImplementation = true;
//        }

//        /// <summary>
//        /// Checks the authorization policy.
//        /// </summary>
//        /// <param name="resource">The resource.</param>
//        /// <param name="action">The action.</param>
//        /// <returns>true when authorized, otherwise false</returns>
//        public static bool CheckAccess(string action, params string[] resources)
//        {
//            Contract.Requires(!String.IsNullOrEmpty(action));


//            return CheckAccess(ClaimsPrincipal.Current, action, resources);
//        }

//        public static bool CheckAccess(ClaimsPrincipal principal, string action, params string[] resources)
//        {
//            var context = CreateAuthorizationContext(
//                principal,
//                action,
//                resources);

//            return CheckAccess(context);
//        }

//        /// <summary>
//        /// Checks the authorization policy.
//        /// </summary>
//        /// <param name="actions">The actions.</param>
//        /// <param name="resources">The resources.</param>
//        /// <returns>true when authorized, otherwise false</returns>
//        public static bool CheckAccess(Collection<Claim> actions, Collection<Claim> resources)
//        {
//            Contract.Requires(actions != null);
//            Contract.Requires(resources != null);


//            return CheckAccess(new AuthorizationContext(
//                ClaimsPrincipal.Current, resources, actions));
//        }

//        /// <summary>
//        /// Checks the authorization policy.
//        /// </summary>
//        /// <param name="action">The action.</param>
//        /// <param name="resources">The resources.</param>
//        /// <returns>true when authorized, otherwise false</returns>
//        public static bool CheckAccess(string action, params Claim[] resources)
//        {
//            Contract.Requires(action != null);
//            Contract.Requires(resources != null);

//            var actionCollection = new Collection<Claim>();
//            actionCollection.Add(new Claim(ActionType, action));
//            var resourceCollection = new Collection<Claim>();
//            foreach (var resource in resources) resourceCollection.Add(resource);

//            return CheckAccess(new AuthorizationContext(
//                ClaimsPrincipal.Current, resourceCollection, actionCollection));
//        }

//        /// <summary>
//        /// Checks the authorization policy.
//        /// </summary>
//        /// <param name="action">The action.</param>
//        /// <param name="resource">The resource name.</param>
//        /// <param name="resources">The resources.</param>
//        /// <returns>true when authorized, otherwise false</returns>
//        public static bool CheckAccess(string action, string resource, params Claim[] resources)
//        {
//            Contract.Requires(action != null);
//            Contract.Requires(resource != null);

//            var resourceList = resources.ToList();
//            resourceList.Add(new Claim(ResourceType, resource));
//            return CheckAccess(action, resourceList.ToArray());
//        }

//        /// <summary>
//        /// Checks the authorization policy.
//        /// </summary>
//        /// <param name="context">The authorization context.</param>
//        /// <returns>true when authorized, otherwise false</returns>
//        public static bool CheckAccess(AuthorizationContext context)
//        {
//            Contract.Requires(context != null);


//            if (EnforceAuthorizationManagerImplementation)
//            {
//                var authZtype = AuthorizationManager.GetType().FullName;
//                if (authZtype.Equals("System.Security.Claims.ClaimsAuthorizationManager"))
//                {
//                    throw new InvalidOperationException("No ClaimsAuthorizationManager implementation configured.");
//                }
//            }

//            return AuthorizationManager.CheckAccess(context);
//        }

//        public static AuthorizationContext CreateAuthorizationContext(ClaimsPrincipal principal, string action, params string[] resources)
//        {
//            var actionClaims = new Collection<Claim>
//            {
//                new Claim(ActionType, action)
//            };

//            var resourceClaims = new Collection<Claim>();

//            if (resources != null && resources.Length > 0)
//            {
//                resources.ToList().ForEach(ar => resourceClaims.Add(new Claim(ResourceType, ar)));
//            }

//            return new AuthorizationContext(
//                principal,
//                resourceClaims,
//                actionClaims);
//        }
//    }
//}
//namespace Thinktecture.IdentityModel.Authorization
//{
//    /// <summary>
//    /// Provides direct access methods for evaluating authorization policy
//    /// </summary>
//    public static class ClaimsAuthorization
//    {
//        /// <summary>
//        /// Default action claim type.
//        /// </summary>
//        public const string ActionType = "http://application/claims/authorization/action";

//        /// <summary>
//        /// Default resource claim type
//        /// </summary>
//        public const string ResourceType = "http://application/claims/authorization/resource";

//        public static bool EnforceAuthorizationManagerImplementation { get; set; }

//        /// <summary>
//        /// Gets the registered authorization manager.
//        /// </summary>
//        public static System.Security.Claims.ClaimsAuthorizationManager AuthorizationManager
//        {
//            get
//            {
//                return System.IdentityModel.Services.FederatedAuthentication.FederationConfiguration.IdentityConfiguration.ClaimsAuthorizationManager;
//            }
//        }

//        static ClaimsAuthorization()
//        {
//            EnforceAuthorizationManagerImplementation = true;
//        }

//        /// <summary>
//        /// Checks the authorization policy.
//        /// </summary>
//        /// <param name="resource">The resource.</param>
//        /// <param name="action">The action.</param>
//        /// <returns>true when authorized, otherwise false</returns>
//        public static bool CheckAccess(string action, params string[] resources)
//        {
//            System.Diagnostics.Contracts.Contract.Requires(!String.IsNullOrEmpty(action));


//            return CheckAccess(System.Security.Claims.ClaimsPrincipal.Current, action, resources);
//        }

//        public static bool CheckAccess(System.Security.Claims.ClaimsPrincipal principal, string action, params string[] resources)
//        {
//            var context = CreateAuthorizationContext(
//                principal,
//                action,
//                resources);

//            return CheckAccess(context);
//        }

//        /// <summary>
//        /// Checks the authorization policy.
//        /// </summary>
//        /// <param name="actions">The actions.</param>
//        /// <param name="resources">The resources.</param>
//        /// <returns>true when authorized, otherwise false</returns>
//        public static bool CheckAccess(System.Collections.ObjectModel.Collection<Claim> actions, System.Collections.ObjectModel.Collection<Claim> resources)
//        {
//            System.Diagnostics.Contracts.Contract.Requires(actions != null);
//            System.Diagnostics.Contracts.Contract.Requires(resources != null);


//            return CheckAccess(new AuthorizationContext(
//                ClaimsPrincipal.Current, resources, actions));
//        }

//        /// <summary>
//        /// Checks the authorization policy.
//        /// </summary>
//        /// <param name="action">The action.</param>
//        /// <param name="resources">The resources.</param>
//        /// <returns>true when authorized, otherwise false</returns>
//        public static bool CheckAccess(string action, params Claim[] resources)
//        {
//            System.Diagnostics.Contracts.Contract.Requires(action != null);
//            System.Diagnostics.Contracts.Contract.Requires(resources != null);

//            var actionCollection = new System.Collections.ObjectModel.Collection<Claim>();
//            actionCollection.Add(new Claim(ActionType, action));
//            var resourceCollection = new System.Collections.ObjectModel.Collection<Claim>();
//            foreach (var resource in resources) resourceCollection.Add(resource);

//            return CheckAccess(new AuthorizationContext(
//                ClaimsPrincipal.Current, resourceCollection, actionCollection));
//        }

//        /// <summary>
//        /// Checks the authorization policy.
//        /// </summary>
//        /// <param name="action">The action.</param>
//        /// <param name="resource">The resource name.</param>
//        /// <param name="resources">The resources.</param>
//        /// <returns>true when authorized, otherwise false</returns>
//        public static bool CheckAccess(string action, string resource, params Claim[] resources)
//        {
//            System.Diagnostics.Contracts.Contract.Requires(action != null);
//            System.Diagnostics.Contracts.Contract.Requires(resource != null);

//            var resourceList = resources.ToList();
//            resourceList.Add(new Claim(ResourceType, resource));
//            return CheckAccess(action, resourceList.ToArray());
//        }

//        /// <summary>
//        /// Checks the authorization policy.
//        /// </summary>
//        /// <param name="context">The authorization context.</param>
//        /// <returns>true when authorized, otherwise false</returns>
//        public static bool CheckAccess(AuthorizationContext context)
//        {
//            System.Diagnostics.Contracts.Contract.Requires(context != null);


//            if (EnforceAuthorizationManagerImplementation)
//            {
//                var authZtype = AuthorizationManager.GetType().FullName;
//                if (authZtype.Equals("System.Security.Claims.ClaimsAuthorizationManager"))
//                {
//                    throw new InvalidOperationException("No ClaimsAuthorizationManager implementation configured.");
//                }
//            }

//            return AuthorizationManager.CheckAccess(context);
//        }

//        public static AuthorizationContext CreateAuthorizationContext(ClaimsPrincipal principal, string action, params string[] resources)
//        {
//            var actionClaims = new System.Collections.ObjectModel.Collection<Claim>
//            {
//                new Claim(ActionType, action)
//            };

//            var resourceClaims = new System.Collections.ObjectModel.Collection<Claim>();

//            if (resources != null && resources.Length > 0)
//            {
//                resources.ToList().ForEach(ar => resourceClaims.Add(new Claim(ResourceType, ar)));
//            }

//            return new AuthorizationContext(
//                principal,
//                resourceClaims,
//                actionClaims);
//        }
//    }
//}