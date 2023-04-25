using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace OnlineExaminationSystem.ViewModels
{
    public class CreateQuestionViewModel
    {
        [Required]
        [Display(Name = "Question Text")]
        public string QuestionText { get; set; }

        [Required]
        [Display(Name = "Question Type")]
        public string QuestionType { get; set; }

        [Required]
        [Display(Name = "Points")]
        public int Points { get; set; }

        [Required]
        public int CourseId { get; set; }
        public List<SelectListItem> Courses { get; set; } = new List<SelectListItem>();
        public List<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();

        public List<SelectListItem> QuestionTypes { get; } = new List<SelectListItem>
    {
        new SelectListItem { Text = "Single Choice", Value = "SingleChoice" },
        new SelectListItem { Text = "Multiple Choice", Value = "MultipleChoice" },
        new SelectListItem { Text = "True/False", Value = "TrueFalse" },
        new SelectListItem { Text = "Short Answer", Value = "ShortAnswer" },
        new SelectListItem { Text = "Essay", Value = "Essay" }
    };
    }



}
