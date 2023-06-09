﻿using System.ComponentModel;

namespace OnlineExaminationSystem.Models
{
    public class Submission
    {
        public int Id { get; set; }

        public DateTime SubmissionTime { get; set; }
        [DefaultValue(false)]
        public bool isResultChecked { get; set; }
        public int AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
        public virtual ExamResult ExamResult { get; set; }
    }

}
