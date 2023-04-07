using SchoolManagementSystem.Data;

namespace SchoolManagementSystem.Models
{
    public class TestSubmission
    {
        public int Id { get; set; }
        public int TestAssignmentId { get; set; }
        public string StudentId { get; set; }
        public DateTime SubmissionTime { get; set; }

        public virtual TestAssignment TestAssignment { get; set; }
        public virtual ApplicationUser Student { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
    }
}
