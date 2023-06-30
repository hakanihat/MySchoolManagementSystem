using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.Common;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.ViewModels;


namespace OnlineExaminationSystem.Controllers
{
    [Authorize]
    public class ExamResultController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SendGridEmailSender> _logger;

        public ExamResultController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<SendGridEmailSender> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                var examResult = await _context.ExamResults
                    .Include(er => er.Exam)
                        .ThenInclude(e => e.Questions)
                        .ThenInclude(q => q.Answers)
                    .Include(er => er.StudentAnswers)
                        .ThenInclude(sa => sa.Answer)
                            .ThenInclude(a => a.Question)
                    .FirstOrDefaultAsync(er => er.Id == id && er.ApplicationUserId == userId);

                if (examResult == null)
                {
                    return NotFound();
                }

                var studentAnswers = new Dictionary<int, List<string>>();

                foreach (var studentAnswer in examResult.StudentAnswers)
                {
                    try
                    {
                        if (studentAnswer.AnswerId.HasValue)
                        {
                            var answer = await _context.Answers.FindAsync(studentAnswer.AnswerId.Value);

                            if (answer != null)
                            {
                                if (studentAnswers.ContainsKey(answer.QuestionId))
                                {
                                    studentAnswers[answer.QuestionId].Add(answer.Text);
                                }
                                else
                                {
                                    studentAnswers.Add(answer.QuestionId, new List<string> { answer.Text });
                                }
                            }
                        }
                        else
                        {
                            if (studentAnswers.ContainsKey(studentAnswer.QuestionId))
                            {
                                studentAnswers[studentAnswer.QuestionId].Add(studentAnswer.Text);
                            }
                            else
                            {
                                studentAnswers.Add(studentAnswer.QuestionId, new List<string> { studentAnswer.Text });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while processing a student answer.");
                        return RedirectToAction("Index", "Error");
                    }
                }

                double totalPoints = examResult.Exam.Questions.Sum(q => q.Points);

                var viewModel = new ExamResultViewModel
                {
                    Id = examResult.Id,
                    Score = examResult?.Score ?? 0,
                    Comment = examResult?.Comment ?? "Final result",
                    TotalPoints = totalPoints,
                    ExamName = examResult.Exam.Name,
                    ShortAnsEssayAnswers = (examResult.Exam.Questions ?? Enumerable.Empty<Question>())
                        .Where(q => q.Type == QuestionType.ShortAnswer || q.Type == QuestionType.Essay)
                        .Select(q => new ShortAnsEssayViewModel
                        {
                            QuestionId = q.Id,
                            QuestionText = q.Text,
                            AnswerText = studentAnswers.ContainsKey(q.Id) ? studentAnswers[q.Id]?.FirstOrDefault() ?? "" : ""

                        }).ToList(),
                    SMTFAnswers = (examResult.Exam.Questions ?? Enumerable.Empty<Question>())
                        .Where(q => q.Type != QuestionType.ShortAnswer && q.Type != QuestionType.Essay)
                        .Select(q => new SMTFAnswersViewModel
                        {
                            QuestionId = q.Id,
                            QuestionText = q.Text,
                            AnswerId = examResult.StudentAnswers?.FirstOrDefault(sa => sa.QuestionId == q.Id)?.AnswerId ?? null,
                            CorrectAnswerText = q.Answers?.Where(a => a.IsCorrect).Select(a => a.Text).ToList(),
                            AnswerText = studentAnswers.ContainsKey(q.Id) ? studentAnswers[q.Id].ToList() : new List<string>(),
                            IsCorrect = examResult.StudentAnswers.Any(sa => sa.Answer != null && sa.Answer.QuestionId == q.Id)
                        }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the exam result details.");
                return RedirectToAction("Index", "Error");
            }
        }



        [Authorize(Roles = "admin,teacher")]
        public async Task<IActionResult> Edit(int submissionId)
        {
            try
            {
                var submission = await _context.Submissions
                   .Include(s => s.Assignment)
                        .ThenInclude(a => a.Exam)
                             .ThenInclude(e => e.Questions)
                   .Include(s => s.ExamResult)
                        .ThenInclude(er => er.StudentAnswers)
                             .ThenInclude(sa => sa.Answer)
                                    .ThenInclude(a => a.Question)
                   .FirstOrDefaultAsync(s => s.Id == submissionId);


                if (submission == null)
                {
                    return NotFound();
                }

                var studentAnswers = new Dictionary<int, List<string>>();

                foreach (var studentAnswer in submission.ExamResult.StudentAnswers)
                {
                    try
                    {
                        if (studentAnswer.AnswerId.HasValue)
                        {
                            var answer = await _context.Answers.FindAsync(studentAnswer.AnswerId.Value);

                            if (answer != null)
                            {
                                if (studentAnswers.ContainsKey(answer.QuestionId))
                                {
                                    studentAnswers[answer.QuestionId].Add(answer.Text);
                                }
                                else
                                {
                                    studentAnswers.Add(answer.QuestionId, new List<string> { answer.Text });
                                }
                            }
                        }
                        else
                        {
                            if (studentAnswers.ContainsKey(studentAnswer.QuestionId))
                            {
                                studentAnswers[studentAnswer.QuestionId].Add(studentAnswer.Text);
                            }
                            else
                            {
                                studentAnswers.Add(studentAnswer.QuestionId, new List<string> { studentAnswer.Text });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while processing a student answer.");
                        return RedirectToAction("Index", "Error");
                    }
                }

                double totalPoints = submission.ExamResult.Exam.Questions.Sum(q => q.Points);
                int correctAnswers = submission.ExamResult.StudentAnswers.Count(sa => sa.Answer != null && sa.Answer.IsCorrect);

                var viewModel = new ExamResultViewModel
                {
                    Id = submission.ExamResult.Id,
                    Score = submission.ExamResult?.Score ?? 0,
                    Comment = submission.ExamResult?.Comment ?? "Final result",
                    TotalPoints = totalPoints,
                    ExamName = submission.ExamResult.Exam.Name,
                    ShortAnsEssayAnswers = submission.ExamResult.Exam.Questions.Where(q => q.Type == QuestionType.ShortAnswer || q.Type == QuestionType.Essay)
                       .Select(q => new ShortAnsEssayViewModel
                       {
                           QuestionId = q.Id,
                           QuestionText = q.Text,
                           AnswerText = studentAnswers.ContainsKey(q.Id) ? studentAnswers[q.Id]?.FirstOrDefault() ?? "" : ""
                       }).ToList(),
                    SMTFAnswers = submission.ExamResult.Exam.Questions.Where(q => q.Type != QuestionType.ShortAnswer && q.Type != QuestionType.Essay)
                            .Select(q => new SMTFAnswersViewModel
                            {
                                QuestionId = q.Id,
                                QuestionText = q.Text,
                                AnswerId = submission.ExamResult.StudentAnswers.FirstOrDefault(sa => sa.Answer.QuestionId == q.Id)?.AnswerId,
                                CorrectAnswerText = q.Answers.Where(a => a.IsCorrect).Select(a => a.Text).ToList(),
                                AnswerText = studentAnswers.ContainsKey(q.Id) ? studentAnswers[q.Id].ToList() : new List<string>(),
                                IsCorrect = submission.ExamResult.StudentAnswers.Any(sa => sa.Answer.QuestionId == q.Id)
                            }).ToList()
                };

                TempData["SuccessMessage"] = ConstantStrings.ExamResultSent;
                return RedirectToAction("Index", "Submission", new { successMessage = TempData["SuccessMessage"] });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the submission details.");
                return RedirectToAction("Index", "Error");
            }
        }


        [Authorize(Roles = "admin,teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamResultViewModel viewModel)
        {
            try
            {
                if (id != viewModel.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var examResult = await _context.ExamResults.FindAsync(id);

                    if (examResult == null)
                    {
                        return NotFound();
                    }

                    examResult.Score = viewModel.Score;
                    examResult.Comment = viewModel.Comment;

                    _context.Update(examResult);
                    var submission = await _context.Submissions.FindAsync(examResult.SubmissionId);
                    submission.isResultChecked = true;
                    _context.Update(submission);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while editing the exam result.");
                return RedirectToAction("Index", "Error");
            }
        }

    }
}
