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
            var courses = await _context.Courses.ToListAsync();
            course = await _context.Courses
              .Where(c => c.Exams.Any(e => e.Id == examId))
                .FirstOrDefaultAsync(); // check why is null
            double totalPoints = 0;
            if (examId != null)
            {
                var exam = await _context.Exams
                    .Include(e => e.Questions)
                      .FirstOrDefaultAsync(e => e.Id == examId);
                totalPoints = exam.Questions.Sum(q => q.Points);
            }


       
            var viewModel = new AssignmentViewModel
            {
                ExamId = examId.HasValue ? (int)examId : 0,
                Exams = exams.Select(e => new ExamViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    ExamDuration = e.ExamDuration,
                    CourseId = e.CourseId
                }).ToList(),
                
                Users = studentsWithProfile.Select(s => new StudentViewModel
                {
                    Id = s.Id,
                    SchoolNumber = s.SchoolNumber ?? 0,
                    FullName = $"{s.UserProfile.FullName}",
                    GroupName = $"{s.Group.Name}"
                }).ToList(),

                MaxPoints = totalPoints,
                CourseId = course?.Id ?? 0,
                Courses = courses.Select(c => new CourseViewModel // map courses to CourseViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                }).ToList()
            };

            return View("AssignmentViewModel", viewModel);
        }


        // POST: Assignment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssignmentViewModel viewModel)
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");
            var studentsWithProfile = _context.Users.Include(u => u.UserProfile).Include(g => g.Group).Where(u => students.Contains(u)).ToList();
            var exams = await _context.Exams.ToListAsync();
            var courses = await _context.Courses.ToListAsync();

            viewModel.Exams = exams.Select(e => new ExamViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                ExamDuration = e.ExamDuration,
                CourseId = e?.CourseId ?? 0
            }).ToList();

            viewModel.Users = studentsWithProfile.Select(s => new StudentViewModel
            {
                Id = s.Id,
                SchoolNumber = s.SchoolNumber ?? 0,
                FullName = $"{s.UserProfile.FullName}",
                GroupName = $"{s.Group.Name}"
            }).ToList();

            viewModel.Courses = courses.Select(c => new CourseViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();

            if (ModelState.IsValid)
            {
                if (HttpContext.User.Identity.IsAuthenticated)
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
                            CourseId = viewModel.CourseId
                        };

                        Console.WriteLine(assignment.AssignedByUserId);
                        _context.Add(assignment);
                    }

                }
                await _context.SaveChangesAsync();

                return View("AssignmentViewModel", viewModel);
            }

            return View("AssignmentViewModel", viewModel);
        }

        // GET: Assignment/StudentAssignments
        public async Task<IActionResult> StudentAssignments()
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(User);

            // Get all the assignments assigned to the current user
            var assignments = await _context.Assignments
                .Include(a => a.AssignedBy)
                .Include(a => a.AssignedTo)
                .Include(a => a.Course)
                .Include(a => a.Exam)
                .Where(a => a.AssignedToUserId == currentUser.Id)
                .ToListAsync();

            // Get the submissions and associated exam results for the current user
            var submissions = await _context.Submissions
                .Include(s => s.Assignment)
                .Include(s => s.ExamResult)
                .Where(s => s.ApplicationUserId == currentUser.Id && assignments.Contains(s.Assignment))
                .ToListAsync();

            // Map the assignments to a view model and include the associated submissions and exam results
            var viewModel = assignments.Select(a => new StudentAssignmentViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                DueDate = a.DueDate,
                MaxPoints = a.MaxPoints,
                CourseId = a.CourseId,
                CourseName = a.Course.Name,
                ExamId = a.ExamId,
                ExamName = a.Exam.Name,
                ExamResultId = submissions.FirstOrDefault(s => s.AssignmentId == a.Id)?.ExamResult?.Id ?? null
            }).ToList();

            return View("StudentAssignmentViewModel", viewModel);
        }






        public async Task<IActionResult> Details(int id)
        {
            // Retrieve the assignment with the specified id from the database
            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .Include(a => a.Exam)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null)
            {
                // If the assignment is not found, return a 404 error
                return NotFound();
            }

            // Create a new instance of the AssignmentViewModel and set its properties
            var viewModel = new StudentAssignmentViewModel
            {
                Id = assignment.Id,
                Title = assignment.Title,
                Description = assignment.Description,
                DueDate = assignment.DueDate,
                MaxPoints = assignment.MaxPoints,
                CourseId = assignment.CourseId,
                CourseName = assignment.Course.Name,
                ExamId = assignment.ExamId,
                ExamName = assignment.Exam.Name
            };

            // Pass the view model to the Details view
            return View("StudentAssignmentDetailsViewModel", viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> GetExamTotalPoints(int examId)
        {
            // Retrieve the exam with the given examId from your data source
            var exam = await _context.Exams
                           .Include(e => e.Questions)
                             .FirstOrDefaultAsync(e => e.Id == examId);
            var totalPoints = exam.Questions.Sum(q => q.Points);
        

            // Return the total points as a JSON response
            return Json(totalPoints);
        }



    }
}
