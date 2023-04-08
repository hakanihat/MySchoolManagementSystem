using System.Security.Claims;
using SchoolManagementSystem.Data;

namespace SchoolManagementSystem.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TeacherId { get; set; }

        public virtual ApplicationUser Teacher { get; set; }
        public virtual ICollection<SchoolClass> SchoolClasses { get; set; }
    }


}
