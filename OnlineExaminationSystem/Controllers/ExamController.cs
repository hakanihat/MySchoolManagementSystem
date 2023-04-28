using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
            viewModel.Courses = await GetCoursesAsync();
            return View("CreateExamViewModel", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExam(CreateExamViewModel viewModel, string SelectedQuestionIdsJson)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Questions = await GetQuestionsAsync();
                viewModel.Courses = await GetCoursesAsync();
                return View(viewModel);
            }

            var exam = new Exam
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                StartTime = viewModel.StartTime,
                EndTime = viewModel.EndTime,
                ApplicationUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                CourseId = viewModel.CourseId
            };

            if (!string.IsNullOrEmpty(SelectedQuestionIdsJson))
            {
                var selectedQuestionIds = JsonConvert.DeserializeObject<int[]>(SelectedQuestionIdsJson);
                foreach (var questionId in selectedQuestionIds)
                {
                    var question = _context.Questions.Find(questionId);
                    _context.ExamQuestions.Add(new ExamQuestion { Exam = exam, Question = question });
                }
            }

            _context.Exams.Add(exam);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public async Task<List<SelectListItem>> GetQuestionsAsync(int? courseId = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = _context.Questions
                .Where(q => q.ApplicationUserId == userId);

            if (courseId.HasValue)
            {
                query = query.Where(q => q.CourseId == courseId.Value);
            }

            var questions = await query
                .Select(q => new SelectListItem { Value = q.Id.ToString(), Text = q.Text })
                .ToListAsync();

            return questions;
        }

        public async Task<List<SelectListItem>> GetCoursesAsync()
        {
            var courses = await _context.Courses
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();

            return courses;
        }


        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> Detail()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IQueryable<Exam> examsQuery = _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Questions)
                    .ThenInclude(q => q.Choices);

            if (User.IsInRole("teacher"))
            {
                examsQuery = examsQuery.Where(e => e.ApplicationUserId == userId);
            }

            var exams = await examsQuery.ToListAsync();

            return View("SeeAllExams", exams);

        }





    }

}
