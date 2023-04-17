using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.ViewModels;

namespace OnlineExaminationSystem.Controllers
{
    [Authorize(Roles = "admin")]
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult CreateQuestion()
        {
            var viewModel = new CreateQuestionViewModel();
            return View("CreateQuestionViewModel",viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateQuestion(CreateQuestionViewModel viewModel, List<string> Answers)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var question = new Question
            {
                Text = viewModel.QuestionText,
                Points = viewModel.Points,
                Type = Enum.Parse<QuestionType>(viewModel.QuestionType),
                Choices = ParseChoices(Answers)
            };

            _context.Questions.Add(question);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        private List<Choice> ParseChoices(List<string> answers)
        {
            var choices = new List<Choice>();

            foreach (var answer in answers)
            {
                choices.Add(new Choice
                {
                    Text = answer,
                    IsCorrect = false
                });
            }

            return choices;
        }


    }
}
