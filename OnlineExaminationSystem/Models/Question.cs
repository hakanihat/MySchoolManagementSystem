using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineExaminationSystem.Models
{
    public enum QuestionType
    {
        SingleChoice,
        MultipleChoice,
        TrueFalse,
        ShortAnswer,
        Essay
    }
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public double Points { get; set; }
        public QuestionType Type { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<StudentAnswer>? StudentAnswers { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    
    }
}
