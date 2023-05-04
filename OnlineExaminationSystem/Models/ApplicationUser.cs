﻿using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;

namespace OnlineExaminationSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public long? SchoolNumber { get; set; }
        public int? GroupId { get; set; }
        public Group Group { get; set; }
        public virtual ICollection<ExamResult> ExamResults { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<ChatRoomUser> ChatRoomUsers { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
    }
}
