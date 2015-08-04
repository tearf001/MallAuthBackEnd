using MallAuth.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MallAuth.DAL
{
    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;

        private Microsoft.AspNet.Identity.UserManager<ApplicationUser> _userManager;

        public AuthRepository()
        {            
            _ctx = new AuthContext();
            _userManager = new Microsoft.AspNet.Identity.UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_ctx));
        }

        public async Task<Microsoft.AspNet.Identity.IdentityResult> RegisterUser(UserModel userModel)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = userModel.UserName
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            return result;
        }

        public async Task<ApplicationUser> FindUser(string userName, string password)
        {
            ApplicationUser user = await _userManager.FindAsync(userName, password);

            return user;
        }

        public void Dispose()
        {
            //_userManager.GenerateUserTokenAsync
            _ctx.Dispose();
            _userManager.Dispose();

        }

        public Client FindClient(string ClientId)
        {
            //return _ctx.Clients.FirstOrDefault(s => s.Id == ClientId);
            var client = _ctx.Clients.Find(ClientId);
            return client;
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        public async Task<bool> AddRefreshToken(RefreshToken rt) {
            var existsToken = _ctx.RefreshTokens.Where(s => s.ClientId == rt.ClientId && s.Subject == rt.Subject).SingleOrDefault();
            if (null != existsToken) {
                var result = await RemoveRefreshToken(existsToken);
            }
            _ctx.RefreshTokens.Add(rt);
            return await _ctx.SaveChangesAsync()>0;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken existsToken)
        {
            _ctx.RefreshTokens.Remove(existsToken);
            return await _ctx.SaveChangesAsync() > 0;
        }
        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                _ctx.RefreshTokens.Remove(refreshToken);
                return await _ctx.SaveChangesAsync() > 0;
            }
            return false;
        }
        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }
        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _ctx.RefreshTokens.ToList();
        }
    }
}