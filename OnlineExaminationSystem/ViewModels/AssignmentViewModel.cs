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
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Required]
        [Display(Name = "Max Points")]
        public int MaxPoints { get; set; }

        [Required]
        [Display(Name = "Exam")]
        public int ExamId { get; set; }
        public IEnumerable<SelectListItem> Exams { get; set; }

        [Required]
        [Display(Name = "Assigned To")]
        public string AssignedToUserId { get; set; }
        public IEnumerable<StudentViewModel> Users { get; set; }
    }

}
