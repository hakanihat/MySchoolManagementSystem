namespace OnlineExaminationSystem.Models
{
    public class StudentAnswer
    {
        public int Id { get; set; }
        public string? Text { get; set; } // to store the student's answer
        public int? AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
        public int SubmissionId { get; set; }
        public virtual Submission Submission { get; set; }
    }




}
