using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineExaminationSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineExaminationSystem.ViewModels
{
    public class CreateExamViewModel
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }

        [Required]
        public int CourseId { get; set; }
        public List<SelectListItem> Courses { get; set; } = new List<SelectListItem>();
        [BindNever]
        public List<int> SelectedQuestionIds { get; set; } = new List<int>();
        public List<SelectListItem> Questions { get; set; } = new List<SelectListItem>();
    }

}
