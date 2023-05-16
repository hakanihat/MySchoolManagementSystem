using System.ComponentModel.DataAnnotations;

namespace OnlineExaminationSystem.Models
{
    public class Exam
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ExamDuration { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public bool IsCheatSecured { get; set; }
        public virtual ICollection<Question> Questions { get; set; }

        public virtual ICollection<ExamResult> ExamResults { get; set; }
    }

}
