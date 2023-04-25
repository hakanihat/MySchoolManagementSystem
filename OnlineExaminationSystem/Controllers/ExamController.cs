using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.ViewModels;
using System.Data;
using System.Security.Claims;

namespace OnlineExaminationSystem.Controllers
{
    [Authorize(Roles = "admin")]
    public class ExamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateExam()
        {
        

            var viewModel = new CreateExamViewModel();
            viewModel.Questions = await GetQuestionsAsync();
            return View("CreateExamViewModel", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExam(CreateExamViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Questions = await GetQuestionsAsync();
                return View(viewModel);
            }

            var exam = new Exam
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                StartTime = viewModel.StartTime,
                EndTime = viewModel.EndTime,
                ApplicationUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value
            };


            foreach (var questionId in viewModel.SelectedQuestionIds)
            {
                var question = _context.Questions.Find(questionId);
                exam.Questions.Add(question);
            }

            _context.Exams.Add(exam);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }


        public async Task<List<SelectListItem>> GetQuestionsAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var questions = await _context.Questions
                .Where(q => q.ApplicationUserId == userId)
                .Select(q => new SelectListItem { Value = q.Id.ToString(), Text = q.Text })
                .ToListAsync();

            return questions;
        }

    }

}
