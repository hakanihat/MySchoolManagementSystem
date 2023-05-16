using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineExaminationSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineExaminationSystem.ViewModels
{
    public class ExamViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The exam duration must be at least 1.")]
        public int ExamDuration { get; set; }

        [Required]
        public int CourseId { get; set; }
        public List<SelectListItem> Courses { get; set; } = new List<SelectListItem>();

        [BindNever]
        public List<int> SelectedQuestionIds { get; set; } = new List<int>();
        [BindNever]
        public List<SelectListItem> Questions { get; set; } = new List<SelectListItem>();
        public bool IsCheatSecured { get; set; }

    }


}
