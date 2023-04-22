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

    }

}
