namespace OnlineExaminationSystem.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        public string AssignedById { get; set; }
        public virtual ApplicationUser AssignedBy { get; set; }

        public string AssignedToId { get; set; }
        public virtual ApplicationUser AssignedTo { get; set; }

        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
