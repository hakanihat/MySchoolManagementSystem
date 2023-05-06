using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineExaminationSystem.ViewModels
{
    public class StudentAssignmentViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int MaxPoints { get; set; }
        public int CourseId { get; set; }
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }
        public int ExamId { get; set; }
        [Display(Name = "Exam Name")]
        public string ExamName { get; set; }
    }
}
