using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineExaminationSystem.ViewModels
{
    public class CourseViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

  
    }

}
