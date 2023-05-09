﻿using Microsoft.AspNetCore.Authorization;
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

            // Calculate the total points and number of correct answers
            int totalPoints = examResult.Exam.Questions.Sum(q => q.Points);
            int correctAnswers = examResult.StudentAnswers.Count(sa => sa.Answer.IsCorrect);

            // Create the view model
            var viewModel = new ExamResultViewModel
            {
                Id = examResult.Id,
                Score = examResult?.Score ?? 0,
                Comment = examResult?.Comment ?? "Final result",
                TotalPoints = totalPoints,
                CorrectAnswers = correctAnswers,
                ExamName = examResult.Exam.Name,
                StudentAnswers = examResult.StudentAnswers.Select(sa => new TakeExamAnswerViewModel
                {
                    Text = sa.Answer.Text,
                    IsCorrect = sa.Answer.IsCorrect,
                    QuestionText = sa.Answer.Question.Text
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
