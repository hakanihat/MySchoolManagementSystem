using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.ViewModels;


namespace OnlineExaminationSystem.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private Course course;

        public AssignmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Assignment
        public async Task<IActionResult> Index()
        {
            var assignments = await _context.Assignments
                .Include(a => a.Exam)
                .ToListAsync();

            return View(assignments);
        }

        // GET: Assignment/Create
        // GET: Assignment/Create
        public async Task<IActionResult> Create(int? examId)
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");
            var studentsWithProfile = _context.Users.Include(u => u.UserProfile).Include(g => g.Group).Where(u => students.Contains(u)).ToList();
            var exams = await _context.Exams.ToListAsync();
            course = await _context.Courses
        .Where(c => c.Exams.Any(e => e.Id == examId))
        .FirstOrDefaultAsync(); // check why is null
            var viewModel = new AssignmentViewModel
            {
                ExamId = examId.HasValue ? (int)examId : 0,
                Exams = exams.Select(e => new ExamViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    CourseId = e.CourseId
                }).ToList(),
                Users = studentsWithProfile.Select(s => new StudentViewModel
                { Id = s.Id,
                    SchoolNumber = s.SchoolNumber ?? 0,
                    FullName = $"{s.UserProfile.FullName}",
                    GroupName = $"{s.Group.Name}"
                }).ToList(),
                CourseId = course.Id

            };

            return View("AssignmentViewModel",viewModel);
        }


        // POST: Assignment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssignmentViewModel viewModel)
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");
            var studentsWithProfile = _context.Users.Include(u => u.UserProfile).Include(g => g.Group).Where(u => students.Contains(u)).ToList();
            var exams = await _context.Exams.ToListAsync();
            
            viewModel.Exams = exams.Select(e => new ExamViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                CourseId = course.Id
            }).ToList();

            viewModel.Users = studentsWithProfile.Select(s => new StudentViewModel
            {
                Id =s.Id,
                SchoolNumber = s.SchoolNumber ?? 0,
                FullName = $"{s.UserProfile.FullName}",
                GroupName = $"{s.Group.Name}"
            }).ToList();

            if (ModelState.IsValid)
            {
                foreach (var userId in viewModel.AssignedToUserId)
                {
                    var assignment = new Assignment
                    {
                        Title = viewModel.Title,
                        Description = viewModel.Description,
                        DueDate = viewModel.DueDate,
                        MaxPoints = viewModel.MaxPoints,
                        AssignedByUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                        AssignedToUserId = userId,
                        ExamId = viewModel.ExamId,
                        CourseId = course.Id
                    };

                    Console.WriteLine(assignment.AssignedByUserId);

                    _context.Add(assignment);
                }

                await _context.SaveChangesAsync();

                return View("AssignmentViewModel", viewModel);
            }

       

            return View("AssignmentViewModel", viewModel);
        }


    }
}
