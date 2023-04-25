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
        public int Points { get; set; }
        public QuestionType Type { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<Choice> Choices { get; set; }
        public ICollection<Exam> Exams { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    
    }
}
