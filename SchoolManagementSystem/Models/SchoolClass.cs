using Humanizer.Localisation;
using SchoolManagementSystem.Data;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace SchoolManagementSystem.Models
{
    public class SchoolClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CourseId { get; set; }

        public virtual Course Course { get; set; }

        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }

        public ICollection<ApplicationUser> Students
        {
            get
            {
                return Users.Where(u => u.Role == "Student").ToList();
            }
        }

        // This property is not mapped to the database
        [NotMapped]
        public ICollection<ApplicationUser> Users { get; set; }
    }
}

