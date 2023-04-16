namespace SchoolManagementSystem.Models
{
    public class Exam
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public virtual ICollection<ExamResult> ExamResults { get; set; }
    }


}
