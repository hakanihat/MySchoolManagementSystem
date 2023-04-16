namespace OnlineExaminationSystem.Models
{
    public class CourseEnrollment
    {
        public int Id { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
    }
}
