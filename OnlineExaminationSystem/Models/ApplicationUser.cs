using Microsoft.AspNetCore.Identity;

namespace OnlineExaminationSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<ExamResult> ExamResults { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<ChatRoomUser> ChatRoomUsers { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
