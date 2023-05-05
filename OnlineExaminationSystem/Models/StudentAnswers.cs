namespace OnlineExaminationSystem.Models
{
    public class StudentAnswer
    {
        public int Id { get; set; }
        public int AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
        public int ExamResultId { get; set; }
        public virtual ExamResult ExamResult { get; set; }
    }
}
