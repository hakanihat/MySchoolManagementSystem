namespace OnlineExaminationSystem.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<ApplicationUser> Teachers { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
