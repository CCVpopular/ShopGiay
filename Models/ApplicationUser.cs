using Microsoft.AspNetCore.Identity;

namespace ShopGiay.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime? Dob {  get; set; }
    }
}
