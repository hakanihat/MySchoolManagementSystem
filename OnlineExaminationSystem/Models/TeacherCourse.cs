namespace OnlineExaminationSystem.Models
{
    public class TeacherCourse
    {
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
