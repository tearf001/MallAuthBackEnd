using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MallAuth.Models
{
    public class ApplicationUser : Microsoft.AspNet.Identity.EntityFramework.IdentityUser
    {
        //    public string Roles { get; set; }
        public string RealName { get; set; }
        public int? DeptId { get; set; }


        //public async Task<System.Security.Claims.ClaimsIdentity>  
        //    GenerateUserIdentityAsync(Microsoft.AspNet.Identity.UserManager<ApplicationUser, string> manager)
        //{
        //    var userIdentity = await manager.CreateIdentityAsync(this,
        //        Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalBearer);
        //    return userIdentity;
        //}
        //public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
    }

    public class CustomUserClaim
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }
        public string UserReferId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public bool valid { get; set; }
    }

}