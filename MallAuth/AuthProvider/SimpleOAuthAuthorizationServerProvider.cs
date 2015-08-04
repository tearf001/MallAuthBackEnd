using MallAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace MallAuth.Provider
{
    public class SimpleOAuthAuthorizationServerProvider : Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerProvider
    {
        private DAL.AuthRepository authRepository;
        private bool userClientAuth;
        private string AnoymouseAllowedOrigins;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="authRepository"></param>
        public SimpleOAuthAuthorizationServerProvider(DAL.AuthRepository authRepository)
        {
            // TODO: Complete member initialization
            this.authRepository = authRepository;
            this.userClientAuth = Convert.ToBoolean(ConfigurationManager.AppSettings["useClientAuthentication"]);
            this.AnoymouseAllowedOrigins = ConfigurationManager.AppSettings["AnoymouseAllowedOrigins"];
            if (string.IsNullOrWhiteSpace(AnoymouseAllowedOrigins)) this.AnoymouseAllowedOrigins = "*";
        }
        /// <summary>
        /// 客户端解包,鉴权
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async System.Threading.Tasks.Task ValidateClientAuthentication(Microsoft.Owin.Security.OAuth.OAuthValidateClientAuthenticationContext context)
        {
            ////如果不做客户端鉴权,请取消注释以下2行代码
            //context.Validated();
            //await Task.FromResult<object>(null);
            ////如果不做客户端鉴权

            var clientId = default(string);
            var clientSecret = default(string);
            var client = default(Client);
            /// base64 encode in the Authorization header.
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                ///x-www-form-urlencoded
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {

                // Remove the comments from the below line context.SetError, and invalidate context 
                // if you want to force sending clientId/secrects once obtain access tokens.                 
                if (!userClientAuth)
                    context.Validated();
                else
                    context.SetError("invalid_clientId", "ClientId 必须"); 

                await Task.FromResult<object>(null);
            }

            client = authRepository.FindClient(context.ClientId);

            if (client == null)
            {
                context.SetError("invalid_clientId", string.Format("Client '{0}' 不允许!,不在服务端注册列表中", context.ClientId));

                await Task.FromResult<object>(null);
            }
            #region 原生应用-（非JS应用，续校验CLIENT_id私密信息）已注释
            ////使用本地凭证的应用
            //if (client.ApplicationType == Models.ApplicationTypes.NativeConfidential)
            //{
            //    if (string.IsNullOrWhiteSpace(clientSecret))
            //    {
            //        context.SetError("invalid_clientId", "Client secret should be sent.");

            //        await Task.FromResult<object>(null);
            //    }
            //    else
            //    {
            //        if (client.Secret != MallAuth.DAL.Helper.GetHash(clientSecret))
            //        {
            //            context.SetError("invalid_clientId", "Client secret is invalid.");

            //            await Task.FromResult<object>(null);
            //        }
            //    }
            //} 
            #endregion

            if (!client.Active)
            {
                context.SetError("invalid_clientId", "Client is inactive.");

                await Task.FromResult<object>(null);
            }
            //从服务器配置的 刷新令牌时间，及 来源（AllowedOrigin)
            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();

            await Task.FromResult<object>(null);
        }
        /// <summary>
        /// 发放。授权资源访问凭证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async System.Threading.Tasks.Task GrantResourceOwnerCredentials(Microsoft.Owin.Security.OAuth.OAuthGrantResourceOwnerCredentialsContext context)
        {
            //return base.GrantResourceOwnerCredentials(context);
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            
            //鉴定ClientID之后。授权来源
            if (allowedOrigin == null)                
                allowedOrigin = this.userClientAuth? "*" : this.AnoymouseAllowedOrigins;
            /////ngauthenticationweb Access-Control-Allow-Origin //来源鉴定
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", allowedOrigin.Split(','));
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET", "POST", "PUT", "DELETE" });
            
    
            Microsoft.AspNet.Identity.EntityFramework.IdentityUser user =
                await authRepository.FindUser(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "用户名,密码不正确");
                return;
            }
            //claim based 认证
            var identity = new System.Security.Claims.ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, context.UserName));
            identity.AddClaim(new System.Security.Claims.Claim("sub", context.UserName));
            identity.AddClaim(new System.Security.Claims.Claim("role", "user"));
            //identity.AddClaim(new System.Security.Claims.Claim("test", "test"));
            var claims = MallAuth.ServerCache.GlobalCache.getInstance().getUserClaims(context.UserName);
            foreach (var item in claims)
            {
                identity.AddClaim(new System.Security.Claims.Claim(item.Type,item.Value ));
            }
            ///额外的响应参数.注意这个和Claim不同
            var props = new Microsoft.Owin.Security.AuthenticationProperties(new Dictionary<string, string>
                {
                    { 
                        "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    { 
                        "userName", context.UserName
                    }
                });

            var ticket = new Microsoft.Owin.Security.AuthenticationTicket(identity, props);
            context.Validated(ticket);

            //context.Validated(identity);
        }
        public override Task TokenEndpoint(Microsoft.Owin.Security.OAuth.OAuthTokenEndpointContext context)
        {
            //return base.TokenEndpoint(context);
            foreach (var property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }

    }
}
