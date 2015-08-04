using MallAuth.DAL;
using MallAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MallAuth.Provider
{
    public class SimpleFreshTokenProvider :Microsoft.Owin.Security.Infrastructure.IAuthenticationTokenProvider
    {

        public void Create(Microsoft.Owin.Security.Infrastructure.AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public async System.Threading.Tasks.Task CreateAsync(Microsoft.Owin.Security.Infrastructure.AuthenticationTokenCreateContext context)
        {
            var clientId = context.Ticket.Properties.Dictionary["as:client_id"];
            if (string.IsNullOrEmpty(clientId)) return;
            var refreshTokenId = Guid.NewGuid().ToString("n");
            using (var _repo = new AuthRepository())
            {
                var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");
                var token = new RefreshToken
                {
                    Id = Helper.GetHash(refreshTokenId),
                    ClientId = clientId,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime)),
                    IssuedUtc = DateTime.UtcNow,
                    Subject = context.Ticket.Identity.Name
                };
                context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
                context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;
                token.ProtectedTicket = context.SerializeTicket();
                var result = await _repo.AddRefreshToken(token);
                if (result)
                {
                    context.SetToken(refreshTokenId);
                }
            }
        }

        public void Receive(Microsoft.Owin.Security.Infrastructure.AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public async System.Threading.Tasks.Task ReceiveAsync(Microsoft.Owin.Security.Infrastructure.AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            string hashedTokenId = Helper.GetHash(context.Token);

            using (AuthRepository _repo = new AuthRepository())
            {
                var refreshToken = await _repo.FindRefreshToken(hashedTokenId);

                if (refreshToken != null)
                {
                    //Get protectedTicket from refreshToken class
                    context.DeserializeTicket(refreshToken.ProtectedTicket);
                    var result = await _repo.RemoveRefreshToken(hashedTokenId);
                }
            }
        }
    }
}