using MallAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MallAuth.DAL
{
    public class AuthContext : Microsoft.AspNet.Identity.EntityFramework.IdentityDbContext<ApplicationUser>
    {
        public AuthContext()
            : base("LocalAuth")
        {

        }

        public static List<Client> test = new List<Client> {        
               new Client
                {
                    Id = "MallConsole",
                    Secret = Helper.GetHash("abc@123"),
                    Name = "AngularJS front-end Application",
                    ApplicationType =ApplicationTypes.JavaScript,
                    Active = true,
                    RefreshTokenLifeTime = 7200,
                    AllowedOrigin = "*",//或者www.URL,*,如果置空，由SimpleOathAthencitactionServerProvider处理
                },
             new Client
                {
                    Id = "consoleApp",
                    Secret = Helper.GetHash("123@abc"),
                    Name = "Console Application",
                    ApplicationType = ApplicationTypes.NativeConfidential,
                    Active = true,
                    RefreshTokenLifeTime = 14400,
                    AllowedOrigin = "*"
                }
        };
        //public List<MallAuth.Models.Client> Clients { get { return test; } }
        public System.Data.Entity.DbSet<Client> Clients { get; set; }
        public System.Data.Entity.DbSet<RefreshToken> RefreshTokens { get; set; }
        public System.Data.Entity.DbSet<CustomUserClaim> CustomUserClaims { get; set; }
        public System.Data.Entity.DbSet<ContentManagerSystem> CMS { get; set; }//Directory
        public System.Data.Entity.DbSet<Directory> Directories { get; set; }//Directory
    }

    public class Helper
    {
        public static string GetHash(string input)
        {
            System.Security.Cryptography.HashAlgorithm hashAlgorithm = new System.Security.Cryptography.SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
        public static int AuthorEditReputation = 10;
        public static int PublicEditReputaion = 40;
        public static int PublicProtectReputation = 100;
        public static int PublicCloseReputation = 1000;
    }
}