using OnlineExaminationSystem.Models;

namespace OnlineExaminationSystem.ViewModels
{
    public class SubmissionViewModel
    {
        public int SubmissionId { get; set; }
        public long UserSchoolNumber { get; set; }
        public string CourseName { get; set; }
        public string GroupName { get; set; }
        public string AssignmentName { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int ExamResultId { get; set; }
    }


}
