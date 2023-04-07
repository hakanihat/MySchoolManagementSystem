namespace SchoolManagementSystem.Models
{
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ClassId { get; set; }
        public DateTime DueDate { get; set; }
        public virtual SchoolClass Class { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Grade> Grades { get; set; }
    }
}
