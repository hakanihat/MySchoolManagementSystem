namespace OnlineExaminationSystem.ViewModels
{
    public class TakeExamAnswerViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string QuestionText { get; set; }
        public bool IsCorrect { get; set; }
    }

}
