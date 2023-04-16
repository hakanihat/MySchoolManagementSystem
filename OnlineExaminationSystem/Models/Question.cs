namespace OnlineExaminationSystem.Models
{
    public enum QuestionType
    {
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
        public ICollection<Choice> Choices { get; set; }
        public ICollection<Exam> Exams { get; set; }
    }
}
