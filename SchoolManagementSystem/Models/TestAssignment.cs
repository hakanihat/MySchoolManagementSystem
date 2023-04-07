using System.Security.Claims;

namespace SchoolManagementSystem.Models
{
    public class TestAssignment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int ClassId { get; set; }
        public int TestId { get; set; }

        public virtual SchoolClass SchoolClass { get; set; }
        public virtual Test Test { get; set; }
        public virtual ICollection<TestSubmission> TestSubmissions { get; set; }
    }
}
