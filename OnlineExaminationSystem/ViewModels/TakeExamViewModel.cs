namespace OnlineExaminationSystem.ViewModels
{
    public class TakeExamViewModel
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }

        public int AssignmentId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<TakeExamQuestionViewModel> Questions { get; set; }
    }
}
