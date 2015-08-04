using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace MallAuth.ServerCache
{

    public static class MallClaim
    {
        public const string get = "http://mallconsoleapp/crud/get";
        public const string post = "http://mallconsoleapp/crud/post";
        public const string put = "http://mallconsoleapp/crud/put";
        public const string delete = "http://mallconsoleapp/crud/delete";
        public const string resource = "http://mallconsoleapp/resource";
        public static readonly Dictionary<string, string> dict = new Dictionary<string, string>
        {
            {"get",get},{"post",post},{"put",put},{"delete",delete},{"role",ClaimTypes.Role}
        };
    }
    public class GlobalCache
    {
        private static GlobalCache instance;

        private GlobalCache()
        {
            InitUserClaims();

        }
        /// <summary>
        /// 初始化用户申明
        /// </summary>
        private void InitUserClaims()
        {
            UserClaims_ = new Dictionary<string, List<Claim>>();
            using (var db = new MallAuth.DAL.AuthContext())
            {
                foreach (var uc in db.CustomUserClaims.Where(s => s.valid == true))
                {
                    if (MallClaim.dict.ContainsKey(uc.ClaimType))
                    {
                        var newClaim = new Claim(MallClaim.dict[uc.ClaimType], uc.ClaimValue);
                        if (!UserClaims_.ContainsKey(uc.UserReferId))
                            UserClaims_.Add(uc.UserReferId, new List<Claim> { newClaim });
                        else
                            UserClaims_[uc.UserReferId].Add(newClaim);
                    }
                }
            }
        }

        public static void Clean()
        {
            instance = new GlobalCache();
        }
        public static GlobalCache getInstance()
        {
            if (instance == null)
                instance = new GlobalCache();
            return instance;
        }



        private Dictionary<string, List<Claim>> UserClaims_;
        public List<Claim> getUserClaims(string username)
        {
            if (UserClaims_.ContainsKey(username))
                return UserClaims_[username];
            return null;

        }


    }
}