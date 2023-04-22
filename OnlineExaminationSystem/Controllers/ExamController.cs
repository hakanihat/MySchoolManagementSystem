//using Microsoft.AspNetCore.Mvc;
//using OnlineExaminationSystem.Data;
//using OnlineExaminationSystem.Models;
//using OnlineExaminationSystem.ViewModels;

//namespace OnlineExaminationSystem.Controllers
//{
//    public class ExamController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public ExamController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }

//        public IActionResult CreateExam()
//        {
//            var viewModel = new CreateExamViewModel();
//            viewModel.Questions = _context.Questions.ToList();
//            return View(viewModel);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult CreateExam(CreateExamViewModel viewModel)
//        {
//            if (!ModelState.IsValid)
//            {
//                viewModel.Questions = _context.Questions.ToList();
//                return View(viewModel);
//            }

//            var exam = new Exam
//            {
//                Name = viewModel.Name,
//                Description = viewModel.Description,
//                StartTime = viewModel.StartTime,
//                EndTime = viewModel.EndTime,
//                ApplicationUser = _context.Users.Find(User.Identity.GetUserId())
//            };

//            _context.Exams.Add(exam);
//            _context.SaveChanges();

//            foreach (var questionId in viewModel.SelectedQuestionIds)
//            {
//                var examQuestion = new ExamQuestion
//                {
//                    Exam = exam,
//                    Question = _context.Questions.Find(questionId)
//                };
//                _context.ExamQuestions.Add(examQuestion);
//            }

//            _context.SaveChanges();
//            return RedirectToAction("Index", "Home");
//        }
//    }

//}
