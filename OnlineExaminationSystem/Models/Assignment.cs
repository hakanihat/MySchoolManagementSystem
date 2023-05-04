namespace OnlineExaminationSystem.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int MaxPoints { get; set; }

        public string AssignedByUserId { get; set; }
        public ApplicationUser AssignedBy { get; set; }

        public string AssignedToUserId { get; set; }
        public ApplicationUser AssignedTo { get; set; }

        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public virtual ICollection<Submission> Submissions { get; set; }
    }

}
