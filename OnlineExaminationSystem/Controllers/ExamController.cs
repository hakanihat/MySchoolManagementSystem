﻿using Microsoft.AspNetCore.Authorization;
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
        

            var viewModel = new ExamViewModel();
            viewModel.Questions = await GetQuestionsAsync();
            viewModel.Courses = await GetCoursesAsync();
            return View("CreateExamViewModel", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExam(ExamViewModel viewModel, string SelectedQuestionIdsJson)
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

         
        public async Task<IActionResult> Edit(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (exam == null)
            {
                return NotFound();
            }

            var viewModel = new ExamViewModel
            {
                Name = exam.Name,
                Description = exam.Description,
                StartTime = exam.StartTime,
                EndTime = exam.EndTime,
                CourseId = exam.CourseId,
                Courses = await GetCoursesAsync(),
                SelectedQuestionIds = exam.Questions.Select(q => q.Id).ToList(),
                Questions = await GetQuestionsAsync()
            };

            return View("EditExam",viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamViewModel viewModel, string SelectedQuestionIdsJson)
        {
            if (ModelState.IsValid)
            {
                var exam = await _context.Exams
                    .Include(e => e.Course)
                    .Include(e => e.Questions)
                    .FirstOrDefaultAsync(e => e.Id == id);

                exam.Name = viewModel.Name;
                exam.Description = viewModel.Description;
                exam.StartTime = viewModel.StartTime;
                exam.EndTime = viewModel.EndTime;
                exam.CourseId = viewModel.CourseId;

                var selectedQuestionIds = JsonConvert.DeserializeObject<int[]>(SelectedQuestionIdsJson);
  
                var currentQuestionIds = exam.Questions.Select(q => q.Id).ToList();
                var addedQuestionIds = selectedQuestionIds.Except(currentQuestionIds).ToList();
                var removedQuestionIds = currentQuestionIds.Except(selectedQuestionIds).ToList();

                // Add new selected questions to ExamQuestion
                foreach (var questionId in addedQuestionIds)
                {
                    var examQuestion = new ExamQuestion { ExamId = exam.Id, QuestionId = questionId };
                    _context.ExamQuestions.Add(examQuestion);
                }

                // Remove unselected questions from ExamQuestion
                foreach (var questionId in removedQuestionIds)
                {
                    var examQuestion = await _context.ExamQuestions
                        .FirstOrDefaultAsync(eq => eq.ExamId == exam.Id && eq.QuestionId == questionId);
                    if (examQuestion != null)
                    {
                        _context.ExamQuestions.Remove(examQuestion);
                    }
                }

                try
                {
                    _context.Update(exam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamExists(exam.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return View("EditExam", viewModel);
            }

            viewModel.Courses = await GetCoursesAsync();
            viewModel.Questions = await GetQuestionsAsync();
            return View("EditExam", viewModel);
        }



        private bool ExamExists(int id)
        {
            return _context.Exams.Any(e => e.Id == id);
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound();
            }

            // Delete related records in ExamQuestion table
            var relatedExamQuestions = await _context.ExamQuestions.Where(eq => eq.ExamId == id).ToListAsync();
            _context.ExamQuestions.RemoveRange(relatedExamQuestions);

            // Remove exam and save changes
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }





    }

}
