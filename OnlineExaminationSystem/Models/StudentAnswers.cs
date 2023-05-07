namespace OnlineExaminationSystem.Models
{
    public class StudentAnswer
    {
        public int Id { get; set; }
        public int AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
        public int SubmissionId { get; set; }
        public virtual Submission Submission { get; set; }
    }

}
