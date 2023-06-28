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
        private readonly ILogger<SendGridEmailSender> _logger;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CreateQuestionAsync()
        {
            var viewModel = new QuestionViewModel();
            viewModel.Courses = await GetCoursesAsync(User);
            return View("CreateQuestionViewModel", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestionAsync(QuestionViewModel viewModel, string AnswersJson)
        {
            try
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
                viewModel.Courses = await GetCoursesAsync(User);
                return View("CreateQuestionViewModel", viewModel);
            }
            catch (JsonException ex)
            {
                // Handle JSON deserialization exception
                // Log the exception
                _logger.LogError(ex, "An error occurred while deserializing JSON.");

                // Handle the exception or return an error view
                return RedirectToAction("Error", "Home");
            }
            catch (DbUpdateException ex)
            {
                // Handle database update exception
                // Log the exception
                _logger.LogError(ex, "An error occurred while updating the database.");

                // Handle the exception or return an error view
                return RedirectToAction("Error", "Home");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                // Log the exception
                _logger.LogError(ex, "An error occurred while creating the question.");

                // Handle the exception or return an error view
                return RedirectToAction("Error", "Home");
            }
        }


        private void CreateQuestionAnswers(List<AnswerViewModel> answers, Question question)
        {
            try
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
            catch (DbUpdateException ex)
            {
             
                _logger.LogError(ex, "An error occurred while updating the database.");
                throw; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the question answers.");
                throw; 
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
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
                    Courses = await GetCoursesAsync(User),
                    Answers = question.Answers.Select(c => new AnswerViewModel
                    {
                        AnswerText = c.Text,
                        IsCorrect = c.IsCorrect
                    }).ToList()
                };
                return View("QuestionDetail", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the question details.");
                return RedirectToAction("Index", "Error");
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            try
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
                    Courses = await GetCoursesAsync(User),
                    Answers = question.Answers.Select(c => new AnswerViewModel
                    {
                        AnswerText = c.Text,
                        IsCorrect = c.IsCorrect
                    }).ToList()
                };

                return View("EditQuestionViewModel", viewModel);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the question for editing.");
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the question for editing.");
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, QuestionViewModel viewModel, string AnswersJson)
        {
            try
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
                    viewModel.Courses = await GetCoursesAsync(User);
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

                _context.Update(question);
                await _context.SaveChangesAsync();

                viewModel.Courses = await GetCoursesAsync(User);
                return View("EditQuestionViewModel", viewModel);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle the DbUpdateConcurrencyException
                // Log the exception
                _logger.LogError(ex, "An error occurred while updating the question.");
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                // Handle other specific exception types if needed
                // Log the exception
                _logger.LogError(ex, "An error occurred while updating the question.");
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
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
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while deleting the question.");
                return RedirectToAction("Index", "Error");
            }
        }

        public async Task<List<SelectListItem>> GetCoursesAsync(ClaimsPrincipal user)
        {
            try
            {
                List<SelectListItem> courses;

                if (user.IsInRole("admin"))
                {
                    courses = await _context.Courses
                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                        .ToListAsync();
                }
                else
                {
                    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                    courses = await _context.TeacherCourses
                        .Where(tc => tc.ApplicationUserId == userId)
                        .Select(uc => new SelectListItem { Value = uc.Course.Id.ToString(), Text = uc.Course.Name })
                        .ToListAsync();
                }

                return courses;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while retrieving the courses.");

                // Handle the exception or return an empty list
                return new List<SelectListItem>(); // Return an empty list or handle the exception based on your requirement
            }
        }



        public async Task<IActionResult> Index()
        {
            if (_context.Questions != null)
            {
                return View(await _context.Questions.ToListAsync());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
