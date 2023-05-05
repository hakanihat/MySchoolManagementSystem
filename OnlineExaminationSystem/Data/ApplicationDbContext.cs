﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.ViewModels;
using System.Reflection.Emit;

namespace OnlineExaminationSystem.Data
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
               :  base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Course> Courses { get; set; }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatRoomUser> ChatRoomUsers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


           

            builder.Entity<Question>()
                .HasMany(e => e.Exams)
                .WithMany(q => q.Questions)
                .UsingEntity<ExamQuestion>(
                    eq => eq.HasOne(prop => prop.Exam).WithMany().HasForeignKey(prop => prop.ExamId),
                    eq => eq.HasOne(prop => prop.Question).WithMany().HasForeignKey(prop => prop.QuestionId),
                    eq =>
                    {
                        eq.HasKey(prop => new {prop.ExamId, prop.QuestionId});
                    }
                );
            builder.Entity<Assignment>()
                .HasOne(a => a.AssignedBy)
                .WithMany(u => u.AssignmentsBy)
                .HasForeignKey(a => a.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Assignment>()
                .HasOne(a => a.AssignedTo)
                .WithMany(u => u.AssignmentsTo)
                .HasForeignKey(a => a.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);



            builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "1", Name = "admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = "2", Name = "student", NormalizedName = "STUDENT" },
            new IdentityRole { Id = "3", Name = "teacher", NormalizedName = "TEACHER" }
        );


        }

        public DbSet<OnlineExaminationSystem.ViewModels.CreateCourseViewModel>? CreateCourseViewModel { get; set; }
    }
}