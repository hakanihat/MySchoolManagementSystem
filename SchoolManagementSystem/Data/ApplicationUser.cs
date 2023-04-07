using Microsoft.AspNetCore.Identity;
using SchoolManagementSystem.Models;


namespace SchoolManagementSystem.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Role { get; set; }
        public virtual ICollection<SchoolClass> Classes { get; set; }
    }

}
