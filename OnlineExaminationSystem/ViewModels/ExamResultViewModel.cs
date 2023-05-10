namespace OnlineExaminationSystem.ViewModels
{
    public class ExamResultViewModel
    {
        public int Id { get; set; }
        public double Score { get; set; }
        public string Comment { get; set; }
        public double TotalPoints { get; set; }
        public int CorrectAnswers { get; set; }
        public string ExamName { get; set; }
        public List<TakeExamAnswerViewModel> StudentAnswers { get; set; }
    }
}
