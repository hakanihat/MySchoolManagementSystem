using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OnlineExaminationSystem.Common;
using OnlineExaminationSystem.Data;
using OnlineExaminationSystem.Models;
using OnlineExaminationSystem.ViewModels;
using System.Data;
using System.Security.Claims;

namespace OnlineExaminationSystem.Controllers
{
    
    public class ExamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SendGridEmailSender> _logger;

        public ExamController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger<SendGridEmailSender> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize(Roles = "admin,teacher")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin,teacher")]
        public async Task<IActionResult> CreateExam()
        {
            var viewModel = new ExamViewModel();
            viewModel.Questions = await GetQuestionsAsync();
            viewModel.Courses = await GetCoursesAsync();
            return View("CreateExamViewModel", viewModel);
        }

        [Authorize(Roles = "admin,teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExam(ExamViewModel viewModel, string SelectedQuestionIdsJson)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    viewModel.Questions = await GetQuestionsAsync();
                    viewModel.Courses = await GetCoursesAsync();
                    return View("CreateExamViewModel", viewModel);
                }

                var exam = new Exam
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    ExamDuration = viewModel.ExamDuration,
                    IsCheatSecured = viewModel.IsCheatSecured,
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

                TempData["SuccessMessage"] = ConstantStrings.ExamCreationSuccess;
                return RedirectToAction("Detail", "Exam", new { successMessage = TempData["SuccessMessage"] });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the exam.");
                return RedirectToAction("Index", "Error");
            }
        }


        public async Task<List<SelectListItem>> GetQuestionsAsync(int? courseId = null)
        {
            try
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
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "An InvalidOperationException occurred while retrieving the questions.");
                return new List<SelectListItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the questions.");
                throw; 
            }
        }


        public async Task<List<SelectListItem>> GetCoursesAsync()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
                List<Course> courses;

                if (isAdmin)
                {
                    courses = await _context.Courses.ToListAsync();
                }
                else
                {
                    courses = await _context.TeacherCourses
                        .Where(tc => tc.ApplicationUserId == currentUser.Id)
                        .Select(uc => uc.Course)
                        .ToListAsync();
                }

                var courseItems = courses.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

                return courseItems;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "An error occurred due to an invalid operation.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the courses.");
                throw;
            }
        }


        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> Detail()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                IQueryable<Exam> examsQuery = _context.Exams
                    .Include(e => e.Course)
                    .Include(e => e.Questions)
                        .ThenInclude(q => q.Answers);

                if (User.IsInRole("teacher"))
                {
                    examsQuery = examsQuery.Where(e => e.ApplicationUserId == userId);
                }

                var exams = await examsQuery.ToListAsync();

                return View("SeeAllExams", exams);
            }
            catch (InvalidOperationException ex)
            {
                // Handle InvalidOperationException
                _logger.LogError(ex, "An error occurred due to an invalid operation.");
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                // Handle other specific exceptions or provide a generic fallback
                _logger.LogError(ex, "An error occurred while retrieving the exams.");
                return RedirectToAction("Index", "Error");
            }
        }


        [Authorize(Roles = "admin,teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            try
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
                    ExamDuration = exam.ExamDuration,
                    IsCheatSecured = exam.IsCheatSecured,
                    CourseId = exam.CourseId,
                    Courses = await GetCoursesAsync(),
                    SelectedQuestionIds = exam.Questions.Select(q => q.Id).ToList(),
                    Questions = await GetQuestionsAsync()
                };

                return View("EditExam", viewModel);
            }
            catch (InvalidOperationException ex)
            {
                // Handle InvalidOperationException
                _logger.LogError(ex, "An error occurred due to an invalid operation.");
                throw;
            }
            catch (Exception ex)
            {
                // Handle other specific exceptions or provide a generic fallback
                _logger.LogError(ex, "An error occurred while retrieving or preparing the exam for editing.");
                throw;
            }
        }

        [Authorize(Roles = "admin,teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExamViewModel viewModel, string SelectedQuestionIdsJson)
        {
            try
            {
                if (ModelState.ErrorCount <= 1)
                {
                    var exam = await _context.Exams
                        .Include(e => e.Course)
                        .Include(e => e.Questions)
                        .FirstOrDefaultAsync(e => e.Id == id);

                    exam.Name = viewModel.Name;
                    exam.Description = viewModel.Description;
                    exam.ExamDuration = viewModel.ExamDuration;
                    exam.IsCheatSecured = viewModel.IsCheatSecured;
                    exam.CourseId = viewModel.CourseId;

                    if (SelectedQuestionIdsJson != null)
                    {
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
                            return RedirectToAction("Index", "Error");
                        }
                    }

                    TempData["SuccessMessage"] = ConstantStrings.ExamEditSuccess;
                    return RedirectToAction("Detail", "Exam", new { successMessage = TempData["SuccessMessage"] });
                }

                viewModel.Courses = await GetCoursesAsync();
                viewModel.Questions = await GetQuestionsAsync();
                return View("EditExam", viewModel);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the exam.");
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the exam editing process.");
                return RedirectToAction("Index", "Error");
            }
        }

        private bool ExamExists(int id)
        {
            try
            {
                return _context.Exams.Any(e => e.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if the exam exists.");
                throw;
            }
        }

        [Authorize(Roles = "admin,teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var exam = await _context.Exams.FindAsync(id);
                if (exam == null)
                {
                    return NotFound();
                }

                var relatedExamQuestions = await _context.ExamQuestions.Where(eq => eq.ExamId == id).ToListAsync();
                _context.ExamQuestions.RemoveRange(relatedExamQuestions);
                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = ConstantStrings.ExamDeleteSuccess;
                return RedirectToAction("Detail", "Exam", new { successMessage = TempData["SuccessMessage"] });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the exam.");
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the exam.");
                return RedirectToAction("Index", "Error");
            }
        }



        public async Task<IActionResult> TakeExam(int examId, int assignmentId)
        {
            try
            {
                var assignment = await _context.Assignments.FindAsync(assignmentId);

                if (assignment == null)
                {
                    return NotFound();
                }

                if (assignment.IsSubmitted)
                {
                    TempData["SuccessMessage"] = ConstantStrings.ExamRepeatError;
                    return RedirectToAction("StudentHomePage", "Home", new { successMessage = TempData["SuccessMessage"] });
                
                }

                assignment.IsSubmitted = true;

                await _context.SaveChangesAsync();
                var exam = await _context.Exams
                    .Include(e => e.Questions)
                    .ThenInclude(q => q.Answers)
                    .FirstOrDefaultAsync(e => e.Id == examId);

                if (exam == null)
                {
                    return NotFound();
                }

                var viewModel = new TakeExamViewModel
                {
                    ExamId = exam.Id,
                    ExamName = exam.Name,
                    IsSecure = exam.IsCheatSecured,
                    AssignmentId = assignmentId,
                    ExamDuration = exam.ExamDuration,
                    Questions = exam.Questions.Select(q => new TakeExamQuestionViewModel
                    {
                        Id = q.Id,
                        Text = q.Text,
                        QuestionType = q.Type, 
                        Points = q.Points,
                        Answers = q.Answers.Select(o => new TakeExamAnswerViewModel
                        {
                            Id = o.Id,
                            Text = o.Text
                        }).ToList()
                    }).ToList()
                };

                ShuffleQuestions(viewModel.Questions);

                return View(viewModel);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "A database error occurred while retrieving the exam for taking.");
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the exam for taking.");
                return RedirectToAction("Index", "Error");
            }
        }




        [HttpPost]
        public async Task<IActionResult> TakeExam(TakeExamViewModel model, string answersJson, string textAnswersJson)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var assignment = await _context.Assignments.FindAsync(model.AssignmentId);

            if (assignment == null)
            {
                return NotFound();
            }

            assignment.IsSubmitted = true;

            var submission = new Submission
            {
                ApplicationUserId = userId,
                AssignmentId = model.AssignmentId,
                SubmissionTime = DateTime.Now
            };
           
            Dictionary<int,List<int>> answers = new Dictionary<int, List<int>>();
            Dictionary<int, string> textAnswers =  new Dictionary<int, string>();
            if (answersJson != null) {
                answers=JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(answersJson);

            }
            if (textAnswersJson != null) {
                textAnswers = JsonConvert.DeserializeObject<Dictionary<int, string>>(textAnswersJson);
            }
            foreach (var answerId in answers)
            {
                var quest = answerId.Key;
                var anss = answerId.Value;
          
                foreach (var ans in anss) { 
                        if(ans == -1)
                    {
                        var studentAnswer = new StudentAnswer
                        {
                            QuestionId = quest,
                            SubmissionId = submission.Id
                        };
                        submission.StudentAnswers.Add(studentAnswer);
                    }
                    else { 
                        var studentAnswer = new StudentAnswer
                        {

                            AnswerId = ans,
                            QuestionId = quest,
                            SubmissionId = submission.Id
                        };
                        submission.StudentAnswers.Add(studentAnswer);
                    }             

                }
            }
            foreach (var textAnswer in textAnswers)
            {
                var questionId = textAnswer.Key;
                var answerText = textAnswer.Value;

                var studentAnswer = new StudentAnswer
                {
                    Text = answerText,
                    SubmissionId = submission.Id,
                    QuestionId = questionId
                };
                submission.StudentAnswers.Add(studentAnswer);
            }
          

            try
            {
                _context.Submissions.Add(submission);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction("Index", "Error");
            }
            var examResult = await CreateExamResultAsync(model.ExamId, userId, answers, textAnswers, submission);
            submission.ExamResult = examResult;
            return RedirectToAction("Detail", "ExamResult", new { id = examResult.Id });
        }

        public async Task<ExamResult> CreateExamResultAsync(int examId, string userId, Dictionary<int, List<int>> answerIds, Dictionary<int, string> textAnswers, Submission sub)
        {
            try
            {
                var exam = await _context.Exams
                    .Include(e => e.Questions)
                    .ThenInclude(q => q.Answers)
                    .FirstOrDefaultAsync(e => e.Id == examId);

                if (exam == null)
                {
                    throw new ArgumentException($"No exam found with ID {examId}");
                }
                var score = 0.0;
                foreach (var question in exam.Questions)
                {
                    if (question.Type == QuestionType.ShortAnswer || question.Type == QuestionType.Essay)
                    {
                        continue;
                    }

                    var questionId = question.Id;
                    var questionAnswerIds = answerIds.ContainsKey(questionId) ? answerIds[questionId] : new List<int>();

                    var correctAnswerIds = question.Answers.Where(a => a.IsCorrect).Select(a => a.Id).ToList();
                    var selectedCorrectAnswers = questionAnswerIds.Intersect(correctAnswerIds).ToList();

                    var incorrectAnswerIds = question.Answers.Where(a => !a.IsCorrect).Select(a => a.Id).ToList();
                    var selectedIncorrectAnswers = questionAnswerIds.Intersect(incorrectAnswerIds).ToList();

                    if (question.Type == QuestionType.MultipleChoice)
                    {
                        if (selectedIncorrectAnswers.Any() || !selectedCorrectAnswers.Any())
                        {
                            continue;
                        }
                        else
                        {
                            score += question.Points;
                        }
                    }
                    else if (question.Type == QuestionType.TrueFalse || question.Type == QuestionType.SingleChoice)
                    {
                        if (selectedCorrectAnswers.Any())
                        {
                            score += question.Points;
                        }
                    }
                }

                string comment = "Final result";
                if (textAnswers != null && textAnswers.Count > 0)
                {
                    comment = "Wait for teacher's points!";
                }

                var result = new ExamResult
                {
                    ExamId = examId,
                    Score = score,
                    Comment = comment,
                    ApplicationUserId = userId,
                    StudentAnswers = sub.StudentAnswers,
                    SubmissionId = sub.Id
                };

                _context.ExamResults.Add(result);
                await _context.SaveChangesAsync();

                return result;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Argument exception occurred while creating an exam result.");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "A database error occurred while creating an exam result.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an exam result.");
                throw;
            }
        }

        private void ShuffleQuestions(List<TakeExamQuestionViewModel> questions)
        {
            var rng = new Random();
            int n = questions.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var question = questions[k];
                questions[k] = questions[n];
                questions[n] = question;
            }
        }

        [HttpGet]
        public IActionResult GetExams(int? courseId)
        {
            try
            {
                if (courseId.HasValue)
                {
                    var filteredExams = _context.Exams
                        .Where(e => e.CourseId == courseId)
                        .Select(e => new { value = e.Id, text = $"{e.Name} - {e.Description}" })
                        .ToList();

                    return Json(filteredExams);
                }

                var exams = _context.Exams
                    .Select(e => new { value = e.Id, text = $"{e.Name} - {e.Description}" })
                    .ToList();

                return Json(exams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving exams.");
                return RedirectToAction("Index", "Error");
            }
        }


    }

}
