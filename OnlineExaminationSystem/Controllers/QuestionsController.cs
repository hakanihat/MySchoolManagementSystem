using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.ViewModels;

namespace OnlineExaminationSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            var viewModel = new CreateQuestionViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateQuestionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.QuestionTypes = GetQuestionTypes();
                return View("Create", viewModel);
            }

            var question = new Question
            {
                Text = viewModel.QuestionText,
                Points = viewModel.Points,
                Type = Enum.Parse<QuestionType>(viewModel.QuestionType),
                Choices = ParseChoices(viewModel.Answers)
            };

            _context.Questions.Add(question);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        private List<SelectListItem> GetQuestionTypes()
        {
            return Enum.GetValues(typeof(QuestionType))
                .Cast<QuestionType>()
                .Select(x => new SelectListItem
                {
                    Text = x.ToString(),
                    Value = x.ToString()
                })
                .ToList();
        }


        private List<Choice> ParseChoices(string answers)
        {
            var choices = new List<Choice>();
            var answersArray = answers.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var answer in answersArray)
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
