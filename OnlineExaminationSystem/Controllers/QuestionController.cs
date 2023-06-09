﻿using Microsoft.AspNetCore.Authorization;
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
                var answers = JsonConvert.DeserializeObject<List<AnswerViewModel>>(AnswersJson);
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
                TempData["SuccessMessage"] = "Question created successfully!";
                return RedirectToAction("CreateQuestion", "Question", new { successMessage = TempData["SuccessMessage"] });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "An error occurred while deserializing JSON.");
                return RedirectToAction("Error", "Home");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the database.");
                return RedirectToAction("Error", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the question.");
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
                    .Include(q => q.Answers) 
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

                var answers = JsonConvert.DeserializeObject<List<AnswerViewModel>>(AnswersJson);
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

                _context.Answers.RemoveRange(question.Answers);

                CreateQuestionAnswers(viewModel.Answers, question);

                _context.Update(question);
                await _context.SaveChangesAsync();

                viewModel.Courses = await GetCoursesAsync(User);
                TempData["SuccessMessage"] = "Question edited successfully!";
                return RedirectToAction("Index", "Question", new { successMessage = TempData["SuccessMessage"] });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the question.");
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
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

                var examQuestions = await _context.ExamQuestions.Where(eq => eq.QuestionId == id).ToListAsync();
                _context.ExamQuestions.RemoveRange(examQuestions);
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Question deleted successfully!";
                return RedirectToAction("Index", "Question", new { successMessage = TempData["SuccessMessage"] });
            }
            catch (Exception ex)
            {
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
                _logger.LogError(ex, "An error occurred while retrieving the courses.");
                return new List<SelectListItem>(); 
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
