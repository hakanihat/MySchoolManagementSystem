﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineExaminationSystem.ViewModels
{
    public class GroupedAssignmentViewModel
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
        public double MaxPoints { get; set; }

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


    }
}
