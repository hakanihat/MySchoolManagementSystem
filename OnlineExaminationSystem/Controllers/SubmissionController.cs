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

    public SubmissionController(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var submissions = await _context.Submissions
            .Include(s => s.ApplicationUser)
            .Include(s => s.Assignment)
                .ThenInclude(a => a.Course)
            .Include(s => s.ApplicationUser)
                .ThenInclude(u => u.Group)
            .Include(s => s.ExamResult)
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


}
