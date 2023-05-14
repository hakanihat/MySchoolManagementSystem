namespace OnlineExaminationSystem.ViewModels
{
    public class SMTFAnswersViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int? AnswerId { get; set; }
        public List<string>? CorrectAnswerText { get; set; }
        public List<string>? AnswerText { get; set; }
        public bool? IsCorrect { get; set; }
    }
}
