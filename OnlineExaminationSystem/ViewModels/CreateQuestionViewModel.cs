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
        [Display(Name = "Answers")]
        public string Answers { get; set; }

        [Required]
        [Display(Name = "Points")]
        public int Points { get; set; }

        public List<SelectListItem> QuestionTypes { get; set; }
    }


}
