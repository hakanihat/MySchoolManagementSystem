using SchoolManagementSystem.Data;

namespace SchoolManagementSystem.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public string StudentId { get; set; }
        public double Score { get; set; }

        public virtual Assignment Assignment { get; set; }
        public virtual ApplicationUser Student { get; set; }
    }
}
