using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<SendGridEmailSender> _logger;

        private Course course;

        public AssignmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<SendGridEmailSender> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
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
            try
            {
                List<Course> courses;
                List<Exam> exams;

                var currentUser = await _userManager.GetUserAsync(User);
                var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
                var students = await _userManager.GetUsersInRoleAsync("Student");
                var studentsWithProfile = _context.Users.Include(u => u.UserProfile).Include(g => g.Group).Where(u => students.Contains(u)).ToList();
                double totalPoints = 0;

                if (isAdmin)
                {
                    courses = await _context.Courses.ToListAsync();
                    exams = await _context.Exams.ToListAsync();
                }
                else
                {
                    courses = await _context.TeacherCourses
                     .Where(tc => tc.ApplicationUserId == currentUser.Id) // Assuming there is a UserId property in the UserCourse entity representing the user
                      .Select(uc => uc.Course)
                      .ToListAsync();
                    exams = await _context.Exams
                       .Where(e => e.ApplicationUserId == currentUser.Id)
                          .ToListAsync();
                }

                course = await _context.Courses
                  .Where(c => c.Exams.Any(e => e.Id == examId))
                    .FirstOrDefaultAsync() ?? new Course();


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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the assignment.");
                return RedirectToAction("Index", "Error");

            }
        }


        // POST: Assignment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssignmentViewModel viewModel)
        {
            try
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
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while saving the assignment.");

                if (ex.InnerException is SqlException sqlException)
                {
                    _logger.LogError(sqlException, "An SQL exception occurred while saving the assignment.");
                    return RedirectToAction("Index", "Error");
                }
                else
                {
                    _logger.LogError(ex.InnerException, "An exception occurred while saving the assignment.");
                    return RedirectToAction("Index", "Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the assignment.");
                return RedirectToAction("Index", "Error");
            }
        }


        public async Task<IActionResult> GroupedAssignments()
        {
            try
            {
                // Retrieve the assignments from the database
                var assignments = await _context.Assignments.ToListAsync();

                // Group the assignments by name and due date
                var groupedAssignments = assignments.GroupBy(a => new { a.Title, a.DueDate });

                // Create a list of grouped assignment view models
                var viewModel = groupedAssignments.Select(g => new GroupedAssignmentViewModel
                {
                    Title = g.First().Title,
                    Description = g.First().Description,
                    DueDate = g.Key.DueDate,
                    MaxPoints = g.First().MaxPoints,
                    CourseId = g.First().CourseId,
                    ExamId = g.First().ExamId
                }).ToList();

                // Retrieve the courses and exams associated with the assignments
                var courses = await _context.Courses.ToListAsync();
                var exams = await _context.Exams.ToListAsync();

                // Map the courses and exams to the view model
                foreach (var assignment in viewModel)
                {
                    assignment.Courses = courses.Select(c => new CourseViewModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList();

                    assignment.Exams = exams.Select(e => new ExamViewModel
                    {
                        Id = e.Id,
                        Name = e.Name
                    }).ToList();
                }

                return View("GroupedAssignmentViewModel", viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while retrieving grouped assignments.");

                // You can redirect the user to an error page here
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string title, DateTime dueDate)
        {
            try
            {
                // Get the assignments with the specified title and due date
                var assignmentsToDelete = await _context.Assignments
                    .Include(a => a.Submissions)
                    .Where(a => a.Title == title && a.DueDate == dueDate && !a.Submissions.Any())
                    .ToListAsync();

                if (assignmentsToDelete.Count == 0)
                {
                    // No assignments found or they have submissions, return an error or redirect to an appropriate page
                    return NotFound();
                }

                // Remove the assignments from the context and save the changes
                _context.Assignments.RemoveRange(assignmentsToDelete);
                await _context.SaveChangesAsync();

                // Redirect to the appropriate page
                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateException ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while deleting assignments. Database update error.");

                // You can redirect the user to an error page here
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while deleting assignments.");

                // You can redirect the user to an error page here
                return RedirectToAction("Index", "Error");
            }
        }




        // GET: Assignment/StudentAssignments
        public async Task<IActionResult> StudentAssignments()
        {
            try
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
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while retrieving student assignments.");

                // You can redirect the user to an error page here
                return RedirectToAction("Index", "Error");
            }
        }


        public async Task<IActionResult> Details(int id)
        {
            try
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
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while retrieving assignment details.");

                // You can redirect the user to an error page here
                return RedirectToAction("Index", "Error");
            }
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
