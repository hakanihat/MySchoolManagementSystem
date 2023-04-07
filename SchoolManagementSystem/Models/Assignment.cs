using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.Security.Claims;

namespace SchoolManagementSystem.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int ClassId { get; set; }
        public string FilePath { get; set; }

        public virtual SchoolClass SchoolClass { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
    }
}
