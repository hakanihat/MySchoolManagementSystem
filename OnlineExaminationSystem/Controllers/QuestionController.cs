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
    [Authorize(Roles = "admin")]
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CreateQuestionAsync()
        {
            var viewModel = new CreateQuestionViewModel();
            viewModel.Courses = await GetCoursesAsync();
            return View("CreateQuestionViewModel",viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
             public async Task<IActionResult> CreateQuestionAsync(CreateQuestionViewModel viewModel, string AnswersJson)
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
            CreateQuestionAnswers(viewModel.Answers,question);
            viewModel.Courses = await GetCoursesAsync();
            return View("CreateQuestionViewModel", viewModel);
        }

        private void CreateQuestionAnswers(List<AnswerViewModel> answers,Question question)
        {
            var choices = new List<Choice>();

            foreach (var answer in answers)
            {
                choices.Add(new Choice
                {
                    Text = answer.AnswerText,
                    IsCorrect = answer.IsCorrect,
                    QuestionId = question.Id
                });
            }
            _context.Choices.AddRange(choices);
            _context.SaveChanges();

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