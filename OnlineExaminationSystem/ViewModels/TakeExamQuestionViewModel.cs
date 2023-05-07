using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem.ViewModels
{
    public class TakeExamQuestionViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Points { get; set; }
        public QuestionType QuestionType { get; set; }
        public List<TakeExamAnswerViewModel> Answers { get; set; }
    }

}
