using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExaminationSystem.Controllers
{
    [Authorize]
    public class ExamResultController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExamResultController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Detail(int id)
        {
            // Get the current user's ID
            var userId = _userManager.GetUserId(User);

            // Find the exam result with the specified ID that belongs to the current user
            var examResult = await _context.ExamResults
                .Include(er => er.Exam)
                    .ThenInclude(e => e.Questions)
                .Include(er => er.StudentAnswers)
                    .ThenInclude(sa => sa.Answer)
                        .ThenInclude(a => a.Question)
                .FirstOrDefaultAsync(er => er.Id == id && er.ApplicationUserId == userId);

            if (examResult == null)
            {
                return NotFound();
            }

            // Create a dictionary to hold the student's answers
            var studentAnswers = new Dictionary<int, string>();

            // Loop through the student's answers and add them to the dictionary
            foreach (var studentAnswer in examResult.StudentAnswers)
            {
                if (studentAnswer.AnswerId.HasValue)
                {
                    var answer = await _context.Answers.FindAsync(studentAnswer.AnswerId.Value);

                    if (answer != null)
                    {
                        studentAnswers[answer.QuestionId] = answer.Text;
                    }
                }
                else
                {
                    studentAnswers[studentAnswer.QuestionId] = studentAnswer.Text;
                }
            }

            // Calculate the total points and number of correct answers
            double totalPoints = examResult.Exam.Questions.Sum(q => q.Points);
            int correctAnswers = examResult.StudentAnswers.Count(sa => sa.Answer != null && sa.Answer.IsCorrect);


            // Create the view model
            var viewModel = new ExamResultViewModel
            {
                Id = examResult.Id,
                Score = examResult?.Score ?? 0,
                Comment = examResult?.Comment ?? "Final result",
                TotalPoints = totalPoints,
                CorrectAnswers = correctAnswers,
                ExamName = examResult.Exam.Name,
                StudentAnswers = examResult.Exam.Questions.Select(q => new TakeExamAnswerViewModel
                {
                    Text = studentAnswers.ContainsKey(q.Id) ? studentAnswers[q.Id] : "",
                    IsCorrect = q.Answers.Any(a => a.IsCorrect),
                    QuestionText = q.Text
                }).ToList()
            };

            return View(viewModel);
        }


        public async Task<IActionResult> Edit(int submissionId)
        {
            var submission = await _context.Submissions
                .Include(s => s.Assignment)
                .ThenInclude(a => a.Exam)
                .Include(s => s.ExamResult)
                .FirstOrDefaultAsync(s => s.Id == submissionId);

            if (submission == null)
            {
                return NotFound();
            }

            var viewModel = new ExamResultViewModel
            {
                Id = submission.Id,
                Score = submission.ExamResult?.Score ?? 0,
                Comment = submission.ExamResult.Comment,
                TotalPoints = submission.Assignment.Exam.Questions.Sum(q => q.Points),
                CorrectAnswers = submission.ExamResult.StudentAnswers.Count(a => a.Answer.IsCorrect),
                ExamName = submission.Assignment.Exam.Name,
                StudentAnswers = submission.ExamResult.StudentAnswers.Select(a => new TakeExamAnswerViewModel
                {
                    Id = a.Id,
                    QuestionText = a.Answer.Question.Text,
                    Text = a.Answer.Text,
                    IsCorrect = a.Answer.IsCorrect,
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamResultViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            var submission = await _context.Submissions
                .Include(s => s.StudentAnswers)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null)
            {
                return NotFound();
            }

            submission.ExamResult.Score = viewModel.Score;
            submission.ExamResult.Comment = viewModel.Comment;

            //foreach (var answerViewModel in viewModel.StudentAnswers)
            //{
            //    var answer = submission.StudentAnswers.FirstOrDefault(a => a.Id == answerViewModel.Id);
            //    if (answer != null)
            //    {
            //        answer.Points = answerViewModel.Points;
            //    }
            //}

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "Home");
        }

   
    }
}
