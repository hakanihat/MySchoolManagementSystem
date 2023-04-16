namespace OnlineExaminationSystem.Models
{
    public class ExamResult
    {
        public int Id { get; set; }
        public int Score { get; set; }

        public int ExamId { get; set; }
        public virtual Exam Exam { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
