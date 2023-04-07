using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
               : base(options)
        {
        }

        public DbSet<Answer> Answers { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<ChatConversation> ChatConversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<SchoolClass> SchoolClasses { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestAssignment> TestAssignments { get; set; }
        public DbSet<TestSubmission> TestSubmissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .Property(u => u.Role)
                .IsRequired();
           builder.Entity<IdentityRole>().HasData(
           new IdentityRole { Id = "1", Name = "admin", NormalizedName = "ADMIN" },
           new IdentityRole { Id = "2", Name = "student", NormalizedName = "STUDENT" },
           new IdentityRole { Id = "3", Name = "teacher", NormalizedName = "TEACHER" }
       );

         
        }
    }
}