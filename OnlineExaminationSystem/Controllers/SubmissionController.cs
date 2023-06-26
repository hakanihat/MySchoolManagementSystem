using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.ViewModels;

[Authorize(Roles = "admin,teacher")]
public class SubmissionController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SendGridEmailSender> _logger;

    public SubmissionController(ApplicationDbContext context, ILogger<SendGridEmailSender> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<IActionResult> Index()
    {
        try
        {
            var submissions = await _context.Submissions
                .Include(s => s.ApplicationUser)
                .Include(s => s.Assignment)
                    .ThenInclude(a => a.Course)
                .Include(s => s.ApplicationUser)
                    .ThenInclude(u => u.Group)
                .Include(s => s.ExamResult)
                .OrderBy(s => s.SubmissionTime)
                .ToListAsync();

            var viewModel = new List<SubmissionViewModel>();

            foreach (var submission in submissions)
            {
                var model = new SubmissionViewModel
                {
                    SubmissionId = submission.Id,
                    UserSchoolNumber = submission.ApplicationUser.SchoolNumber ?? 0,
                    IsResultChecked = submission.isResultChecked,
                    CourseName = submission.Assignment.Course.Name,
                    GroupName = submission.ApplicationUser.Group.Name,
                    AssignmentName = submission.Assignment.Title,
                    SubmissionDate = submission.SubmissionTime,
                    ExamResultId = submission.ExamResult?.Id ?? 0
                };

                viewModel.Add(model);
            }

            return View(viewModel);
        }
        catch (Exception ex)
        {
            // Log the exception and return an appropriate error response
            _logger.LogError(ex, "An error occurred while retrieving submissions.");
            return RedirectToAction("Index", "Error");
        }
    }



}
