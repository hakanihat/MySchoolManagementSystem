using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.ViewModels;
using System.Security.Claims;

namespace OnlineExaminationSystem.Controllers
{
    [Authorize(Roles = "admin,teacher")]
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CreateQuestionAsync()
        {
            var viewModel = new QuestionViewModel();
            viewModel.Courses = await GetCoursesAsync();
            return View("CreateQuestionViewModel", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestionAsync(QuestionViewModel viewModel, string AnswersJson)
        {
            // Deserialize the JSON string into a list of AnswerViewModel objects
            var answers = JsonConvert.DeserializeObject<List<AnswerViewModel>>(AnswersJson);

            // Assign the answers to the view model's Answers property
            viewModel.Answers = answers;



            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var question = new Question
            {
                Text = viewModel.QuestionText,
                Points = viewModel.Points,
                Type = Enum.Parse<QuestionType>(viewModel.QuestionType),
                ApplicationUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                CourseId = viewModel.CourseId
            };

            _context.Questions.Add(question);
            _context.SaveChanges();
            CreateQuestionAnswers(viewModel.Answers, question);
            viewModel.Courses = await GetCoursesAsync();
            return View("CreateQuestionViewModel", viewModel);
        }

        private void CreateQuestionAnswers(List<AnswerViewModel> answers, Question question)
        {
            var ans = new List<Answer>();

            foreach (var answer in answers)
            {
                ans.Add(new Answer
                {
                    Text = answer.AnswerText,
                    IsCorrect = answer.IsCorrect,
                    QuestionId = question.Id
                });
            }
            _context.Answers.AddRange(ans);
            _context.SaveChanges();

        }

        // GET: Questions/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
           .Include(q => q.Answers) // eagerly load choices
           .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            var viewModel = new QuestionViewModel
            {
                QuestionId = question.Id,
                QuestionText = question.Text,
                QuestionType = question.Type.ToString(),
                Points = question.Points,
                CourseId = question.CourseId,
                Courses = await GetCoursesAsync(),
                Answers = question.Answers.Select(c => new AnswerViewModel
                {
                    AnswerText = c.Text,
                    IsCorrect = c.IsCorrect
                }).ToList()
            };
            return View("QuestionDetail", viewModel);

        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            var viewModel = new QuestionViewModel
            {
                QuestionText = question.Text,
                QuestionType = question.Type.ToString(),
                Points = question.Points,
                CourseId = question.CourseId,
                Courses = await GetCoursesAsync(),
                Answers = question.Answers.Select(c => new AnswerViewModel
                {
                    AnswerText = c.Text,
                    IsCorrect = c.IsCorrect
                }).ToList()
            };

            return View("EditQuestionViewModel", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, QuestionViewModel viewModel, string AnswersJson)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Deserialize the JSON string into a list of AnswerViewModel objects
            var answers = JsonConvert.DeserializeObject<List<AnswerViewModel>>(AnswersJson);

            // Assign the answers to the view model's Answers property
            viewModel.Answers = answers;

            if (!ModelState.IsValid)
            {
                viewModel.Courses = await GetCoursesAsync();
                return View("EditQuestionViewModel", viewModel);
            }

            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            question.Text = viewModel.QuestionText;
            question.Points = viewModel.Points;
            question.Type = Enum.Parse<QuestionType>(viewModel.QuestionType);
            question.ApplicationUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            question.CourseId = viewModel.CourseId;

            // Delete existing choices
            _context.Answers.RemoveRange(question.Answers);

            // Create new choices
            CreateQuestionAnswers(viewModel.Answers, question);

            try
            {
                _context.Update(question);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(question.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            viewModel.Courses = await GetCoursesAsync();
            return View("EditQuestionViewModel", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            // Delete associated records in ExamQuestions table
            var examQuestions = await _context.ExamQuestions.Where(eq => eq.QuestionId == id).ToListAsync();
            _context.ExamQuestions.RemoveRange(examQuestions);

            // Remove the question
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home"); // Redirect to the desired page after deletion
        }


        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }



        public async Task<List<SelectListItem>> GetCoursesAsync()
        {
            var courses = await _context.Courses
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();

            return courses;
        }



    }
}
