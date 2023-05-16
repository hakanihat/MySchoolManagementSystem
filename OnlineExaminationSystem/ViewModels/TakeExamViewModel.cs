namespace OnlineExaminationSystem.ViewModels
{
    public class TakeExamViewModel
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public bool IsSecure { get; set; }

        public int AssignmentId { get; set; }
        public int ExamDuration { get; set; }
        public List<TakeExamQuestionViewModel> Questions { get; set; }
    }
}
