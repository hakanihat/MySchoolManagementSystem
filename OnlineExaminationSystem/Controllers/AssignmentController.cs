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
        public async Task<IActionResult> Create(int examId)
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");
            var studentsWithProfile = _context.Users.Include(u => u.UserProfile).Where(u => students.Contains(u)).ToList();


            var viewModel = new AssignmentViewModel
            {
                ExamId = examId,
                Users = studentsWithProfile.Select(s => new SelectListItem
                {
                    Value = $"{s.SchoolNumber}",
                    Text = $"{s.UserProfile.FullName}"
                }).ToList()

            };

            return View("AssignmentViewModel",viewModel);
        }


        // POST: Assignment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssignmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var assignment = new Assignment
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    DueDate = viewModel.DueDate,
                    MaxPoints = viewModel.MaxPoints,
                    AssignedByUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    AssignedToUserId = viewModel.AssignedToUserId,
                    ExamId = viewModel.ExamId
                };

                _context.Add(assignment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var students = await _userManager.GetUsersInRoleAsync("Student");
            return View("AssignmentViewModel", viewModel);
        }
    }
}
