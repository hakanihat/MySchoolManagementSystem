using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineExaminationSystem.ViewModels
{
    public class AssignmentViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
  
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Required]
        [Display(Name = "Max Points")]
        [Range(1, int.MaxValue, ErrorMessage = "The {0} field must be greater than or equal to {1}.")]
        public int MaxPoints { get; set; }

        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }
        [BindNever]
        public IEnumerable<CourseViewModel> Courses { get; set; } = Enumerable.Empty<CourseViewModel>();

        [Required]
        [Display(Name = "Exam")]
        public int ExamId { get; set; }
        [BindNever]
        public IEnumerable<ExamViewModel> Exams { get; set; } = Enumerable.Empty<ExamViewModel>();

        [Required]
        [Display(Name = "Assigned To")]
        public List<string> AssignedToUserId { get; set; }
        [BindNever]
        public IEnumerable<StudentViewModel> Users { get; set; } = Enumerable.Empty<StudentViewModel>();
    }

}
