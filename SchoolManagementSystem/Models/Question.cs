namespace SchoolManagementSystem.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public virtual ICollection<Option> Options { get; set; }
        public int CorrectOptionId { get; set; }

        public virtual Option CorrectOption { get; set; }
    }
}
