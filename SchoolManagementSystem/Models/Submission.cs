namespace SchoolManagementSystem.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public string AnswerText { get; set; }
        public DateTime SubmissionTime { get; set; }

        public int AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }

}
