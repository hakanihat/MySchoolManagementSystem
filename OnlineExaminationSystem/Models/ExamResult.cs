namespace OnlineExaminationSystem.Models
{
    public class ExamResult
    {
        public int Id { get; set; }
        public int? Score { get; set; }
        public string? Comment { get; set; }
        public int ExamId { get; set; }
        public virtual Exam Exam { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int SubmissionId { get; set; }
        public virtual Submission Submission { get; set; }

        public virtual ICollection<StudentAnswer> StudentAnswers { get; set; }
    }
}
